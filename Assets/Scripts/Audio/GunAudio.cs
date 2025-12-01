using UnityEngine;

/// <summary>
/// [설치] WeaponAnchor 하위 무기 오브젝트
/// [핵심] 발사/재장전 소리를 재생. 피치/볼륨에 미세 랜덤으로 생동감 추가.
/// [필수] AudioSource, 발사/재장전 AudioClip
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class GunAudio : MonoBehaviour
{
    [SerializeField] private AudioClip fireClip;
    [SerializeField] private AudioClip reloadClip;

    [SerializeField] private float volume = 1.0f;           // 기본 볼륨
    [SerializeField] private float pitchRandomRange = 0.05f; // ±범위(작게)

    //===========================================================
    [SerializeField] private float gunshotLoudness = 2.5f;   // 총성 크기(발소리보다 큼)
    //===========================================================

    [SerializeField]
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        // 3D 느낌을 원하면 spatialBlend를 1.0f로, 일단은 기본값 사용
    }

    /// <summary>
    /// 발사 소리 재생.
    /// </summary>
    public void PlayFire()
    {
        if (fireClip == null)
        {
            return;
        }

        audioSource.pitch = 1.0f + Random.Range(-pitchRandomRange, pitchRandomRange);
        audioSource.PlayOneShot(fireClip, volume);

        //========================================================
        if (NoiseEventBus.Instance != null)
        {
            Vector3 pos = transform.position;
            NoiseEventBus.Instance.RaiseNoise(pos, gunshotLoudness);
        }
        //========================================================
    }

    /// <summary>
    /// 재장전 소리 재생.
    /// </summary>
    public void PlayReload()
    {
        if (reloadClip == null)
        {
            return;
        }

        audioSource.pitch = 1.0f + Random.Range(-pitchRandomRange, pitchRandomRange);
        audioSource.PlayOneShot(reloadClip, volume);
    }
}
