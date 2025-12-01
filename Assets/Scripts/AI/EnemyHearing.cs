using UnityEngine;

/// <summary>
/// [설치] 적 루트 오브젝트에 부착
/// [핵심] NoiseEventBus를 구독해 소리를 들으면 '마지막 들은 위치'와 '기억 시간'을 갱신.
/// [필수 연결] NoiseEventBus(씬에 1개)
/// </summary>
public class EnemyHearing : MonoBehaviour
{
    [SerializeField] private float hearingRadius = 10.0f;  // 기본 청취 반경(발소리 기준)
    [SerializeField] private float memoryTime = 3.0f;      // 들은 뒤 유지 시간(초)
    [SerializeField] private LayerMask obstructionMask;     // 매우 두꺼운 벽 등 '완전 차단' 체크(선택)

    private Vector3 lastHeardPosition; // 마지막 들은 소리 좌표
    private float memoryTimer = 0.0f;  // 남은 기억 시간(초)

    private void OnEnable()
    {
        if (NoiseEventBus.Instance != null)
        {
            NoiseEventBus.Instance.OnNoise += HandleNoise;
        }
    }

    private void OnDisable()
    {
        if (NoiseEventBus.Instance != null)
        {
            NoiseEventBus.Instance.OnNoise -= HandleNoise;
        }
    }

    private void Update()
    {
        if (memoryTimer > 0.0f)
        {
            memoryTimer -= Time.deltaTime;
            if (memoryTimer < 0.0f)
            {
                memoryTimer = 0.0f;
            }
        }
    }

    private void HandleNoise(Vector3 pos, float loudness)
    {
        // 로컬 변수: 소리까지의 거리
        float distance = Vector3.Distance(transform.position, pos);

        // 유효 반경 = 기본 반경 × 소리 크기
        float effective = hearingRadius * Mathf.Max(0.1f, loudness);

        if (distance <= effective)
        {
            // (선택) 두꺼운 벽에 완전히 막히면 무시
            if (obstructionMask.value != 0)
            {
                Vector3 dir = (pos - transform.position).normalized;
                Ray ray = new Ray(transform.position, dir);
                RaycastHit hit;

                bool blocked = Physics.Raycast(ray, out hit, distance, obstructionMask, QueryTriggerInteraction.Ignore);
                if (blocked == true)
                {
                    return;
                }
            }

            lastHeardPosition = pos;
            memoryTimer = memoryTime;
        }
    }

    /// <summary>
    /// 최근에 들은 소리가 아직 유효한지.
    /// </summary>
    public bool HasRecentNoise()
    {
        return memoryTimer > 0.0f;
    }

    /// <summary>
    /// 마지막으로 들은 소리 위치.
    /// </summary>
    public Vector3 GetLastHeardPosition()
    {
        return lastHeardPosition;
    }
}
