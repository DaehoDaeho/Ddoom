using UnityEngine;
using System;

/// <summary>
/// [설치] 씬에 빈 오브젝트 하나에 부착
/// [핵심] 소리 이벤트를 전역으로 방송한다. (좌표 + 크기(loudness))
/// [필수 연결] 없음(발신자와 수신자가 이 인스턴스를 사용)
/// </summary>
public class NoiseEventBus : MonoBehaviour
{
    public static NoiseEventBus Instance; // 싱글턴 인스턴스

    /// <summary>
    /// 소리가 발생했을 때 방송되는 이벤트.
    /// position=소리 좌표, loudness=소리 크기(1.0 기본, 총성처럼 크면 2~3)
    /// </summary>
    public event Action<Vector3, float> OnNoise;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 소리 발생을 전파한다.
    /// </summary>
    public void RaiseNoise(Vector3 position, float loudness)
    {
        if (OnNoise != null)
        {
            OnNoise.Invoke(position, loudness);
        }
    }
}
