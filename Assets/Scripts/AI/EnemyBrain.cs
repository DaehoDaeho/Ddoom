using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// [설치] 적 루트 오브젝트(캡슐/모델)에 부착
/// [핵심] 시야와 청각을 함께 보고, '순찰 -> 의심(수색) -> 추적/공격' 상태를 전환한다.
/// [필수 연결] NavMeshAgent, EnemySight, EnemyHearing, PatrolRoute, 플레이어 Health
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBrain : MonoBehaviour
{
    // 상태 정의
    private enum State
    {
        Patrol,       // 웨이포인트 순찰
        Suspicious,   // 소리나 마지막 시야 위치로 가서 확인
        Chase,        // 대상 보이며 추격
        Attack        // 사거리 내 공격
    }

    [Header("References")]
    [SerializeField] private EnemySight sight;         // 시야(보이는지 판단)
    [SerializeField] private EnemyHearing hearing;     // 청각(소리 기억)
    [SerializeField] private PatrolRoute patrol;       // 순찰 경로
    [SerializeField] private NavMeshAgent agent;       // 이동 담당
    [SerializeField] private Transform modelRoot;      // 회전 기준
    [SerializeField] private Health playerHealth;      // 플레이어 Health

    [Header("Chase / Attack")]
    [SerializeField] private float attackRange = 2.0f;        // 공격 거리
    [SerializeField] private float attackIntervalSec = 1.0f;  // 공격 간격
    [SerializeField] private float damagePerHit = 10.0f;      // 데미지
    [SerializeField] private float faceTurnSpeedDeg = 360.0f; // 바라보기 회전 속도

    [Header("Suspicious")]
    [SerializeField] private float reachThreshold = 0.8f;     // 수색 지점 도달 판정 거리
    [SerializeField] private float suspicionTime = 3.0f;      // 수색 상태 유지 시간

    private State state = State.Patrol;       // 현재 상태
    private float attackCooldown = 0.0f;      // 공격 대기
    private float suspiciousTimer = 0.0f;     // 의심 상태 남은 시간

    private void Reset()
    {
        agent = GetComponent<NavMeshAgent>();
        if (modelRoot == null)
        {
            modelRoot = transform;
        }
    }

    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        agent.updateRotation = false;

        if (modelRoot == null)
        {
            modelRoot = transform;
        }

        if(playerHealth == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            if(go != null)
            {
                playerHealth = go.GetComponent<Health>();
            }
        }
    }

    private void Update()
    {
        // 공통 타이머 갱신
        if (attackCooldown > 0.0f)
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown < 0.0f)
            {
                attackCooldown = 0.0f;
            }
        }

        // 전이 체크: 보이면 Chase/Attack으로 단번에 승격
        bool canSee = sight != null && sight.CanSeeTarget() == true;

        if (canSee == true)
        {
            Vector3 targetPos = sight.GetLastSeenPosition();
            float distance = Vector3.Distance(transform.position, targetPos);

            if (distance <= attackRange)
            {
                state = State.Attack;
                DoAttack(targetPos);
            }
            else
            {
                state = State.Chase;
                DoChase(targetPos);
            }

            return;
        }

        // 보이진 않지만 소리를 최근에 들었거나(청각), 마지막 시야 기억이 남았다면(시야 메모리) -> 의심
        bool hasNoise = hearing != null && hearing.HasRecentNoise() == true;
        bool hasLastSeen = sight != null && sight.HasRecentSighting() == true;

        if (hasNoise == true || hasLastSeen == true)
        {
            if (state != State.Suspicious)
            {
                state = State.Suspicious;
                suspiciousTimer = suspicionTime;
            }

            Vector3 goal = hasNoise == true ? hearing.GetLastHeardPosition() : sight.GetLastSeenPosition();
            DoSuspicious(goal);
            return;
        }

        // 아무 단서도 없으면 순찰
        state = State.Patrol;
        DoPatrol();
    }

    // --- 상태 동작 구현 ---

    private void DoPatrol()
    {
        if (patrol != null && patrol.HasPoints() == true)
        {
            Transform target = patrol.GetCurrent();
            if (target != null)
            {
                float dist = Vector3.Distance(transform.position, target.position);

                if (dist <= reachThreshold)
                {
                    patrol.MoveNext();
                }
                else
                {
                    MoveTo(target.position);
                }
            }
        }
        else
        {
            // 순찰 경로가 없으면 제자리 대기
            agent.isStopped = true;
        }
    }

    private void DoSuspicious(Vector3 goal)
    {
        suspiciousTimer -= Time.deltaTime;
        if (suspiciousTimer <= 0.0f)
        {
            // 수색 시간이 끝나면 순찰로 복귀
            state = State.Patrol;
            return;
        }

        float dist = Vector3.Distance(transform.position, goal);

        if (dist <= reachThreshold)
        {
            // 도착했다면 주변을 잠깐 둘러보듯 대기
            agent.isStopped = true;
        }
        else
        {
            MoveTo(goal);
        }
    }

    private void DoChase(Vector3 targetPos)
    {
        MoveTo(targetPos);
    }

    private void DoAttack(Vector3 targetPos)
    {
        // 사거리 안에서만 호출됨. 바라보기 보정.
        FaceTarget(targetPos);

        if (attackCooldown <= 0.0f)
        {
            if (playerHealth != null)
            {
                Vector3 hitPoint = targetPos; // 대략 플레이어 위치
                Vector3 hitNormal = (transform.position - targetPos).normalized; // 반대 방향

                DamageInfo info = new DamageInfo(gameObject, damagePerHit, hitPoint, hitNormal, false);
                playerHealth.ApplyDamage(info);
            }

            attackCooldown = attackIntervalSec;
        }
    }

    // --- 공통 유틸 ---

    private void MoveTo(Vector3 worldPos)
    {
        agent.isStopped = false;
        agent.SetDestination(worldPos);
        FaceTarget(worldPos);
    }

    private void FaceTarget(Vector3 worldPos)
    {
        Vector3 to = worldPos - modelRoot.position; // 모델 -> 목표 벡터
        to.y = 0.0f;

        if (to.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        Quaternion targetRot = Quaternion.LookRotation(to, Vector3.up);

        // 도/초를 보간 비율로 변환(부드럽게 회전)
        float t = Mathf.Min(1.0f, (faceTurnSpeedDeg * Mathf.Deg2Rad) * Time.deltaTime);
        modelRoot.rotation = Quaternion.Slerp(modelRoot.rotation, targetRot, t);
    }
}
