using UnityEngine;

public class HitscanWeapon : WeaponBase
{
    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private Transform firePoint;

    [SerializeField]
    private GameObject owner;               // 가해자 루트(히트마커 소스)
    
    [SerializeField]
    private PlayerMotor playerMotor;        // 이동 속도 보정용
    
    [SerializeField]
    private WeaponInputReader inputReader;  // 조준 여부 참조
    
    [SerializeField]
    private PlayerViewKick viewKick;        // 뷰 킥 트리거

    [SerializeField]
    private float damage = 20.0f;

    [SerializeField]
    private float maxDistance = 100.0f;

    [SerializeField]
    private float spreadAngleDeg = 0.0f;    // 퍼짐 각도

    [SerializeField]
    private LayerMask hitMask;

    [SerializeField]
    private LayerMask ignoreMask = 0;

    [SerializeField]
    private ImpactVfxSpawner impactVfx;

    [SerializeField]
    private ParticleSystem muzzleFlash;

    [Header("Spread (degrees)")]
    [SerializeField]
    private float baseSpreadHip = 1.5f;     // 비조준 기본 퍼짐
    
    [SerializeField]
    private float baseSpreadAds = 0.4f;     // 조준 기본 퍼짐
    
    [SerializeField]
    private float spreadPerShot = 0.3f;     // 발사 시 누적되는 퍼짐
    
    [SerializeField]
    private float spreadDecayPerSec = 2.5f; // 초당 감소(가만히 있을 때)
    
    [SerializeField]
    private float moveSpreadScale = 1.0f;   // 이동 속도에 따른 가중치(정규화된 속도 × 이 값)

    [Header("View Kick (degrees)")]
    [SerializeField]
    private float pitchKickPerShotHip = 1.0f; // 비조준 뷰 킥 크기
    
    [SerializeField]
    private float pitchKickPerShotAds = 0.4f; // 조준 뷰 킥 크기
    
    [SerializeField]
    private float yawKickRandom = 0.3f;       // 좌우로 약간 랜덤 킥

    // 내부 상태
    private float currentSpread = 0.0f;        // 누적 퍼짐(발사로 증가, 시간으로 감소)
    private float lastEffectiveSpread = 0.0f;  // UI 등을 위한 최근 퍼짐 기록

    [SerializeField] private GunAudio gunAudio;
    [SerializeField] private MuzzleFlashController muzzleFlashController;
    [SerializeField] private BulletTracerSpawner tracerSpawner;
    [SerializeField] private RecoilPatternCurve recoilPattern;

    // 내부 상태: 몇 번째 탄을 쐈는지(연사 중 패턴 샘플용)
    private int shotsFiredInBurst = 0;

    private void Update()
    {
        // 퍼짐 감쇠(가만히 있을수록 차분해짐)
        if (currentSpread > 0.0f)
        {
            currentSpread -= spreadDecayPerSec * Time.deltaTime;
            if (currentSpread < 0.0f)
            {
                currentSpread = 0.0f;
            }
        }
    }

    void PlayMuzzleflash()
    {
        if(muzzleFlash != null)
        {
            if(muzzleFlash.isPlaying == false)
            {
                muzzleFlash.Play();
            }
            else
            {
                muzzleFlash.Stop();
                muzzleFlash.Play();
            }
        }
    }

    Vector3 ApplySpread(Vector3 dir, float angleDeg)
    {
        if (angleDeg <= 0.0f)
        {
            return dir.normalized;
        }

        float yaw = Random.Range(-angleDeg, angleDeg);
        float pitch = Random.Range(-angleDeg, angleDeg);

        Quaternion spreadRot = Quaternion.Euler(pitch, yaw, 0.0f);
        Vector3 spreadDir = spreadRot * dir;
        return spreadDir.normalized;
    }

    protected override void Fire()
    {
        if(cameraTransform == null)
        {
            return;
        }

        // 1) 조준 여부
        bool aiming = inputReader != null && inputReader.IsAimPressed == true;

        // 2) 기본 퍼짐(조준이면 더 작게)
        float baseSpread = aiming == true ? baseSpreadAds : baseSpreadHip;

        // 3) 이동 보정(수평 속도를 0~1로 정규화해서 가중)
        float moveFactor = 0.0f; // 정규화된 이동 척도
        if (playerMotor != null)
        {
            float speed = playerMotor.GetHorizontalSpeed();        // m/s
            float normalizeBy = 5.0f;                              // 5 m/s 기준 정규화
            moveFactor = Mathf.Clamp01(speed / normalizeBy);       // 0~1
        }

        float moveSpread = moveFactor * moveSpreadScale;           // 이동으로 늘어나는 퍼짐

        // 4) 총 퍼짐 계산 = 기본 + 누적 + 이동
        float effectiveSpread = baseSpread + currentSpread + moveSpread;

        Vector3 origin = cameraTransform.position;
        Vector3 direction = cameraTransform.forward;

        if(spreadAngleDeg > 0.0f)
        {
            direction = ApplySpread(direction, spreadAngleDeg);
        }

        Ray ray = new Ray(origin, direction);
        RaycastHit hit;

        bool didHit = Physics.Raycast(ray, out hit, maxDistance, hitMask, QueryTriggerInteraction.Ignore);

        PlayMuzzleflash();        

        if (didHit == true)
        {
            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
            if(damageable != null)
            {
                damageable.TakeDamage(owner, damage, hit.point, hit.normal);
            }

            if(impactVfx != null)
            {
                impactVfx.SpawnImpact(hit.point, hit.normal);
            }

            Debug.DrawLine(origin, hit.point, Color.red, 0.2f, false);
        }

        // 1) 총소리
        if (gunAudio != null)
        {
            gunAudio.PlayFire();
        }

        if(muzzleFlashController != null)
        {
            muzzleFlashController.PlayFlash();
        }

        // 2) 트레이서(카메라 원점 -> 히트 지점 또는 사거리 끝)
        Vector3 tracerStart = firePoint != null ? firePoint.position : cameraTransform.position;
        Vector3 tracerEnd;

        if (didHit == true)
        {
            tracerEnd = hit.point;
        }
        else
        {
            tracerEnd = origin + direction.normalized * maxDistance;
        }

        if (tracerSpawner != null)
        {
            tracerSpawner.SpawnTracer(tracerStart, tracerEnd);
        }

        // 8) 발사 후 누적 퍼짐 증가(연속으로 쏘면 점점 퍼짐)
        currentSpread += spreadPerShot;

        // 9) 뷰 킥(조준 중이면 더 작게)
        //if (viewKick != null)
        //{
        //    float pitch = aiming == true ? pitchKickPerShotAds : pitchKickPerShotHip; // 위로 톡
        //    float yaw = Random.Range(-yawKickRandom, yawKickRandom);                   // 좌우로 아주 조금
        //    viewKick.AddKick(yaw, pitch);
        //}

        // 반동 패턴(뷰 킥에 덧셈)
        // 발사 카운트 증가(연사 중이면 누적)
        shotsFiredInBurst += 1;

        if (recoilPattern != null && viewKick != null)
        {
            Vector2 kick = recoilPattern.SampleKickForShot(shotsFiredInBurst);
            // 패턴 값은 아주 작게. 필요하면 스케일 상수 곱하기.
            float yawKick = kick.x;
            float pitchKick = kick.y;

            viewKick.AddKick(yawKick, pitchKick);
        }

        // UI용 기록
        lastEffectiveSpread = effectiveSpread;
    }

    /// <summary>
    /// 현재 퍼짐(도)을 외부(UI)에서 읽을 수 있게 공개.
    /// </summary>
    public float GetEffectiveSpreadDeg()
    {
        return lastEffectiveSpread;
    }

    public void ResetBurstCount()
    {
        shotsFiredInBurst = 0;
    }
}
