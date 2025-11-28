using UnityEngine;

/// <summary>
/// [설치] 적 루트 또는 '눈' 역할 오브젝트에 부착(권장: 적 루트에 부착 후 eyeTransform 지정)
/// [핵심] 플레이어가 시야 범위(거리·각도) 안에 있고, 사이에 벽이 없으면 '보인다'로 판정.
///        최근에 본 위치를 기억하여 잠시 그 위치까지 추적할 수 있게 한다.
/// [필수 연결] target(플레이어 트랜스폼), eyeTransform(레이 시작 위치), obstructionMask(벽/지형 레이어)
/// </summary>
public class EnemySight : MonoBehaviour
{
    [SerializeField] private Transform target;        // 쫓아갈 대상(플레이어)
    [SerializeField] private Transform eyeTransform;  // 시야 레이의 시작 위치(적 눈/머리)
    [SerializeField] private float viewRadius = 15.0f;     // 볼 수 있는 최대 거리(미터)
    [SerializeField] private float viewAngleDeg = 100.0f;  // 좌우 총 시야각(도)
    [SerializeField] private LayerMask obstructionMask;     // 벽/지형 등 가리는 레이어

    // 최근에 플레이어를 본 위치와 타이머(기억 유지 시간)
    [SerializeField] private float memoryTime = 2.0f; // 마지막 시야 보존 시간(초)
    private Vector3 lastSeenPosition;                 // 마지막으로 본 월드 좌표
    private float memoryTimer = 0.0f;                 // 남은 기억 시간(초)

    /// <summary>
    /// 매 프레임 호출하여 대상이 '보이는지' 판정한다.
    /// 보이면 lastSeenPosition을 갱신하고 memoryTimer를 리셋한다.
    /// </summary>
    public bool CanSeeTarget()
    {
        if (target == null || eyeTransform == null)
        {
            return false;
        }

        // 로컬 변수: 대상까지의 벡터와 거리
        Vector3 toTarget = target.position - eyeTransform.position; // 적 눈 -> 플레이어
        float distance = toTarget.magnitude;                         // 두 점 사이 거리

        // 1) 거리 조건
        if (distance > viewRadius)
        {
            // 너무 멀면 보이지 않음
            return false;
        }

        // 2) 각도 조건(시야 원뿔)
        Vector3 forward = eyeTransform.forward; // 눈의 정면 방향
        Vector3 dir = toTarget.normalized;      // 정규화된 방향
        float dot = Vector3.Dot(forward, dir);  // 코사인 값
        float cosHalf = Mathf.Cos((viewAngleDeg * 0.5f) * Mathf.Deg2Rad); // 절반 각의 코사인

        if (dot < cosHalf)
        {
            // 원뿔 밖이면 보이지 않음
            return false;
        }

        // 3) 시야선(사이에 벽이 있는지) — 벽이 없을 때만 보임
        Ray ray = new Ray(eyeTransform.position, dir);
        RaycastHit hit;
        bool blocked = Physics.Raycast(ray, out hit, distance, obstructionMask, QueryTriggerInteraction.Ignore);

        if (blocked == true)
        {
            // 벽/지형 등에 가려졌다면 보이지 않음
            return false;
        }

        // 여기까지 통과 -> 본 것으로 처리
        lastSeenPosition = target.position;
        memoryTimer = memoryTime;
        return true;
    }

    /// <summary>
    /// 최근에 본 위치를 '아직 기억 중'인지 반환한다.
    /// </summary>
    public bool HasRecentSighting()
    {
        if (memoryTimer > 0.0f)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 최근에 본 위치를 반환한다(기억이 없으면 현재 위치 반환).
    /// </summary>
    public Vector3 GetLastSeenPosition()
    {
        if (HasRecentSighting() == true)
        {
            return lastSeenPosition;
        }

        // 기억이 만료되면 대상을 못 보므로 대략 현재 target 위치를 준다(널 가드)
        if (target != null)
        {
            return target.position;
        }

        return transform.position;
    }

    private void Update()
    {
        // 기억 시간 감소(프레임 독립)
        if (memoryTimer > 0.0f)
        {
            memoryTimer -= Time.deltaTime;
            if (memoryTimer < 0.0f)
            {
                memoryTimer = 0.0f;
            }
        }
    }

    // 디버그: 씬 뷰에서 시야 원뿔/반경을 보기 쉽게 그려줌
    private void OnDrawGizmosSelected()
    {
        if (eyeTransform == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(eyeTransform.position, viewRadius);

        // 시야 각 표시
        Vector3 forward = eyeTransform.forward;
        float half = viewAngleDeg * 0.5f;
        Quaternion leftRot = Quaternion.AngleAxis(-half, Vector3.up);
        Quaternion rightRot = Quaternion.AngleAxis(half, Vector3.up);

        Vector3 leftDir = leftRot * forward;
        Vector3 rightDir = rightRot * forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(eyeTransform.position, leftDir * viewRadius);
        Gizmos.DrawRay(eyeTransform.position, rightDir * viewRadius);
    }
}
