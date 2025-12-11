using UnityEngine;

public class WeaponControllerPlus : MonoBehaviour
{
    [Header("데이터 자산")]
    [SerializeField] private WeaponData data;                 // 무기 수치 자산

    [Header("입력 키")]
    [SerializeField] private KeyCode reloadKey = KeyCode.R;   // 재장전 키

    [Header("참조")]
    [SerializeField] private Camera viewCamera;               // 판정의 기준이 되는 카메라

    [Header("사건 수신자  선택 사항")]
    [SerializeField] private MonoBehaviour[] eventTargets;    // 사건을 받을 대상들  인터페이스 구현을 찾아 사용함

    // 런타임 상태
    private int mag;                                          // 현재 탄창의 탄 수
    private int reserve;                                      // 현재 예비 탄약 수
    private bool isReloading;                                 // 재장전 진행 중 여부
    private float nextFireTime;                                // 다음 발사가 가능한 시각
    private float reloadFinishTime;                            // 재장전이 완료되는 시각

    private void Reset()
    {
        if (viewCamera == null)
        {
            viewCamera = Camera.main;                         // 카메라 자동 보정
        }
    }

    private void Awake()
    {
        // 시작 시 런타임 상태 초기화
        if (data != null)
        {
            mag = data.magCapacity;                           // 최초 탄창 채우기
            reserve = data.startReserve;                      // 예비 탄약 설정
        }
        isReloading = false;                                  // 재장전 상태 초기화
        nextFireTime = 0.0f;                                  // 즉시 발사 가능
        reloadFinishTime = 0.0f;                              // 재장전 예약 없음
    }

    private void Update()
    {
        // 재장전 진행 상태를 먼저 확인하여 완료 시점을 처리함
        if (isReloading == true)
        {
            if (Time.time >= reloadFinishTime)
            {
                FinishReload();                               // 재장전 완료 처리
            }
            // 재장전 중에는 발사를 허용하지 않음
        }

        // 발사 입력 처리
        bool wantFire = false;                                // 발사 의지 플래그
        if (data != null)
        {
            if (data.fireMode == FireMode.Semi)
            {
                wantFire = Input.GetMouseButtonDown(0) == true;   // 단발은 눌림 한 번
            }
            else
            {
                wantFire = Input.GetMouseButton(0) == true;       // 연사는 누르는 동안
            }
        }

        if (wantFire == true)
        {
            TryFire();                                        // 발사 시도
        }

        // 재장전 입력 처리
        bool wantReload = Input.GetKeyDown(reloadKey) == true;
        if (wantReload == true)
        {
            TryStartReload();                                 // 수동 재장전 시도
        }
    }

    private void TryFire()
    {
        if (data == null || viewCamera == null)
        {
            return;
        }

        // 재장전 중이면 발사 불가
        if (isReloading == true)
        {
            return;
        }

        // 발사 간격 검사
        if (Time.time < nextFireTime)
        {
            return;
        }

        // 탄약 검사
        if (mag <= 0)
        {
            TryStartReload();                                 // 탄이 없으면 재장전 시도
            return;
        }

        // 발사 실행
        FireOnce();

        // 다음 발사 가능 시각 계산
        nextFireTime = Time.time + data.fireInterval;
    }

    private void FireOnce()
    {
        // 탄 소모
        mag -= 1;

        // 레이 판정  카메라 정면으로 발사
        Ray ray = new Ray(viewCamera.transform.position, viewCamera.transform.forward);
        RaycastHit hit;
        bool didHit = Physics.Raycast(ray, out hit, data.maxDistance, data.hitMask, QueryTriggerInteraction.Ignore);

        if (didHit == true)
        {
            // 오늘은 간단히 로그로만 확인
            Debug.Log("발사 성공  대상 적중");
        }
        else
        {
            Debug.Log("발사 성공  허공");
        }

        // 사건 통지
        NotifyFired();

        // 탄이 바닥나면 자동 재장전 시도
        if (mag <= 0)
        {
            TryStartReload();
        }
    }

    private void TryStartReload()
    {
        if (data == null)
        {
            return;
        }

        // 이미 재장전 중이면 무시
        if (isReloading == true)
        {
            return;
        }

        // 예비 탄약이 없으면 재장전 불가
        if (reserve <= 0)
        {
            return;
        }

        // 탄창이 이미 가득 차 있으면 재장전 불필요
        if (mag >= data.magCapacity)
        {
            return;
        }

        // 재장전 시작
        isReloading = true;
        reloadFinishTime = Time.time + data.reloadTime;

        // 사건 통지
        NotifyReloadStarted();
    }

    private void FinishReload()
    {
        if (data == null)
        {
            return;
        }

        isReloading = false;

        // 채워야 하는 탄 수 계산
        int need = data.magCapacity - mag;
        if (need > reserve)
        {
            need = reserve;
        }

        // 예비에서 탄창으로 이동
        mag += need;
        reserve -= need;

        // 사건 통지
        NotifyReloadFinished();
    }

    private void NotifyFired()
    {
        if (eventTargets == null)
        {
            return;
        }

        for (int i = 0; i < eventTargets.Length; ++i)
        {
            MonoBehaviour mb = eventTargets[i];
            if (mb == null)
            {
                continue;
            }
            IWeaponEvents recv = mb as IWeaponEvents;
            if (recv != null)
            {
                recv.OnFired();
            }
        }
    }

    private void NotifyReloadStarted()
    {
        if (eventTargets == null)
        {
            return;
        }

        for (int i = 0; i < eventTargets.Length; ++i)
        {
            MonoBehaviour mb = eventTargets[i];
            if (mb == null)
            {
                continue;
            }
            IWeaponEvents recv = mb as IWeaponEvents;
            if (recv != null)
            {
                recv.OnReloadStarted();
            }
        }
    }

    private void NotifyReloadFinished()
    {
        if (eventTargets == null)
        {
            return;
        }

        for (int i = 0; i < eventTargets.Length; ++i)
        {
            MonoBehaviour mb = eventTargets[i];
            if (mb == null)
            {
                continue;
            }
            IWeaponEvents recv = mb as IWeaponEvents;
            if (recv != null)
            {
                recv.OnReloadFinished(mag, reserve);
            }
        }
    }

    // 현재 탄창과 예비 탄약을 외부에서 읽을 수 있도록 제공
    public int GetMag() { return mag; }
    public int GetReserve() { return reserve; }
    public bool GetIsReloading() { return isReloading; }
}
