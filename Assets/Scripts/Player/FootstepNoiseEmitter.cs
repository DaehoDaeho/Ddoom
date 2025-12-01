using UnityEngine;

/// <summary>
/// [설치] Player 루트(또는 이동 스크립트 있는 오브젝트)
/// [핵심] 플레이어가 일정 속도 이상으로 움직일 때 일정 간격으로 '발소리' 소리 이벤트를 낸다.
/// [필수 연결] PlayerMotor(수평 속도 읽기), NoiseEventBus(씬에 1개)
/// </summary>
public class FootstepNoiseEmitter : MonoBehaviour
{
    [SerializeField] private PlayerMotor playerMotor; // 수평 속도 제공자
    [SerializeField] private float speedThreshold = 0.5f; // 발소리 낼 최소 속도(m/s)
    [SerializeField] private float stepInterval = 0.45f;  // 걸음 간격(초)
    [SerializeField] private float loudness = 1.0f;       // 발소리 크기(기본 1.0)

    private float stepTimer = 0.0f; // 다음 발소리까지 남은 시간(초)

    private void Reset()
    {
        playerMotor = GetComponent<PlayerMotor>();
    }

    private void Update()
    {
        if (playerMotor == null || NoiseEventBus.Instance == null)
        {
            return;
        }

        float speed = playerMotor.GetHorizontalSpeed(); // 현재 수평 속도(m/s)

        if (speed >= speedThreshold)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0.0f)
            {
                // 현재 위치를 소리 좌표로 사용
                Vector3 pos = transform.position;
                NoiseEventBus.Instance.RaiseNoise(pos, loudness);

                // 속도가 빠르면 간격을 조금 줄여 자연스럽게
                float speedFactor = Mathf.Clamp(speed / 5.0f, 0.5f, 1.5f); // 0.5~1.5
                float next = stepInterval / speedFactor;

                stepTimer = next;
            }
        }
        else
        {
            // 멈추면 즉시 타이머 초기화
            stepTimer = 0.0f;
        }
    }
}
