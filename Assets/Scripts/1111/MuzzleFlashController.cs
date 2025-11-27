using UnityEngine;

/// <summary>
/// [설치] firePoint(총구) 오브젝트
/// [핵심] 발사 시 파티클 재생(+선택: 라이트를 아주 짧게 켰다가 끔)
/// [필수] ParticleSystem(권장), Light(선택)
/// </summary>
public class MuzzleFlashController : MonoBehaviour
{
    [SerializeField] private ParticleSystem muzzleParticle;
    [SerializeField] private Light muzzleLight;      // 선택
    [SerializeField] private float lightTime = 0.03f;

    private float lightTimer;

    private void Update()
    {
        if (muzzleLight != null)
        {
            if (lightTimer > 0.0f)
            {
                lightTimer -= Time.deltaTime;
                if (lightTimer <= 0.0f)
                {
                    muzzleLight.enabled = false;
                }
            }
        }
    }

    /// <summary>
    /// 발사 시 호출: 파티클 재생, 라이트 잠깐 켜기.
    /// </summary>
    public void PlayFlash()
    {
        if (muzzleParticle != null)
        {
            // 재생 중에도 확실히 튀게 하려면 Stop/Play 패턴
            muzzleParticle.Stop();
            muzzleParticle.Play();
        }

        if (muzzleLight != null)
        {
            muzzleLight.enabled = true;
            lightTimer = lightTime;
        }
    }
}
