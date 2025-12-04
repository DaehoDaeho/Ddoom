using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 개별 적이 '공격 허가'를 요청/유지/반납한다.
/// 허가되면 slotWorldPos로 자리 잡고 공격, 아니면 대기/빙글빙글.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyEngageAgent : MonoBehaviour
{
    [SerializeField] private int agentId = 0;  // 고유 식별자(없으면 런타임에 생성)
    [SerializeField] private float releaseDistance = 6.0f; // 플레이어와 너무 멀면 반납
    [SerializeField] private float orbitDistance = 3.5f;   // 대기 시 원형 거리
    [SerializeField] private float orbitSpeed = 1.2f;      // 대기 시 회전 속도

    private bool hasId = false;
    private bool hasEngagePermission = false;
    private Vector3 slotWorldPos;
    private NavMeshAgent agent;

    public bool HasPermission()
    {
        return hasEngagePermission == true;
    }

    public Vector3 GetSlotWorldPos()
    {
        return slotWorldPos;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agentId != 0)
        {
            hasId = true;
        }
        else
        {
            agentId = GetInstanceID();
            hasId = true;
        }
    }

    private void OnDisable()
    {
        if (EngageCoordinator.Instance != null && hasId == true)
        {
            EngageCoordinator.Instance.Release(agentId);
        }
    }

    private void Update()
    {
        if (EngageCoordinator.Instance == null)
        {
            hasEngagePermission = false;
            return;
        }

        Transform target = EngageCoordinator.Instance.transform;
        if (EngageCoordinator.Instance != null && EngageCoordinator.Instance.gameObject != null)
        {
            // target은 Coordinator의 target 필드를 그대로 사용해야 정확함
            // 여기서는 거리 체크만 간단히 수행
        }

        int slotIndex;
        Vector3 pos;

        bool ok = EngageCoordinator.Instance.RequestOrUpdate(agentId, transform.position, out slotIndex, out pos);
        hasEngagePermission = ok == true;
        slotWorldPos = pos;

        // 플레이어와 너무 멀어지면 반납
        if (hasEngagePermission == true && EngageCoordinator.Instance != null)
        {
            Transform player = null;
            if (EngageCoordinator.Instance != null)
            {
                // Coordinator 내부의 target을 직접 노출하지 않았으니, 간단히 자기 자리와의 거리로 대체
                float near = Vector3.Distance(transform.position, slotWorldPos);
                if (near > releaseDistance)
                {
                    EngageCoordinator.Instance.Release(agentId);
                    hasEngagePermission = false;
                }
            }
        }
    }

    /// <summary>
    /// 대기 상태에서 플레이어 주위를 원으로 천천히 돌며 빈 자리를 기다릴 때 호출.
    /// 간단한 원 궤도 좌표를 반환한다.
    /// </summary>
    public Vector3 GetOrbitPointAround(Vector3 center, float t)
    {
        float angle = t * orbitSpeed * 360.0f;
        Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0.0f, Mathf.Sin(angle * Mathf.Deg2Rad)) * orbitDistance;
        return center + offset;
    }
}
