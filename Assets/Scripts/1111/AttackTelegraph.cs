using UnityEngine;

/// <summary>
/// 공격 직전의 '예고' 연출을 관리한다.
/// - BeginTelegraph: duration 동안 라이트/파티클/사운드 재생
/// - IsCompleted: 시간이 끝났는가
/// - Cancel: 도중 취소
/// </summary>
public class AttackTelegraph : MonoBehaviour
{
    [SerializeField] private Light telegraphLight;           // 번쩍 라이트(선택)
    [SerializeField] private ParticleSystem telegraphFx;     // 파티클(선택)
    [SerializeField] private AudioSource telegraphAudio;     // 사운드(선택)
    [SerializeField] private float defaultDuration = 0.3f;   // 기본 예고 시간(초)
    [SerializeField] private float maxLightIntensity = 4.0f; // 라이트 최대 세기

    private float timer = 0.0f;        // 남은 시간(초)
    private float duration = 0.0f;     // 총 예고 시간(초)
    private bool playing = false;      // 예고 재생 중 여부
    private float baseLightIntensity = 0.0f; // 라이트 원래 세기 저장

    private void Awake()
    {
        if (telegraphLight != null)
        {
            baseLightIntensity = telegraphLight.intensity;
            telegraphLight.enabled = false;
        }
    }

    /// <summary>
    /// 예고를 시작한다. duration이 0이면 defaultDuration을 사용.
    /// </summary>
    public void BeginTelegraph(float customDuration)
    {
        duration = customDuration > 0.0f ? customDuration : defaultDuration;
        timer = duration;
        playing = true;

        if (telegraphFx != null)
        {
            telegraphFx.Stop();
            telegraphFx.Play();
        }

        if (telegraphAudio != null)
        {
            telegraphAudio.Stop();
            telegraphAudio.Play();
        }

        if (telegraphLight != null)
        {
            telegraphLight.enabled = true;
            telegraphLight.intensity = baseLightIntensity;
        }
    }

    /// <summary>
    /// 예고 중인지.
    /// </summary>
    public bool IsPlaying()
    {
        return playing == true;
    }

    /// <summary>
    /// 예고가 끝났는지(진짜 공격 가능).
    /// </summary>
    public bool IsCompleted()
    {
        if (playing == false && timer <= 0.0f)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 예고를 즉시 취소한다.
    /// </summary>
    public void Cancel()
    {
        StopAll();
        timer = 0.0f;
        playing = false;
    }

    private void Update()
    {
        if (playing == false)
        {
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0.0f)
        {
            timer = 0.0f;
            playing = false;
            StopAll(); // 라이트/사운드/파티클 끄기
            return;
        }

        // 라이트를 살짝 펄스 느낌으로(0 -> 최대 -> 0)
        if (telegraphLight != null)
        {
            float u = 1.0f - (timer / duration); // 0 -> 1 진행도
            float pulse = Mathf.Sin(u * Mathf.PI); // 0 -> 1 -> 0
            telegraphLight.intensity = Mathf.Lerp(baseLightIntensity, maxLightIntensity, pulse);
        }
    }

    private void StopAll()
    {
        if (telegraphFx != null)
        {
            telegraphFx.Stop();
        }

        if (telegraphAudio != null)
        {
            telegraphAudio.Stop();
        }

        if (telegraphLight != null)
        {
            telegraphLight.intensity = baseLightIntensity;
            telegraphLight.enabled = false;
        }
    }
}
