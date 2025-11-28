using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// [설치] 적 루트 오브젝트(캡슐 등)에 부착
/// [핵심] EnemySight를 사용해 플레이어를 보면 추적, 근접 시 공격.
///        Health.OnDamaged를 구독해 '맞았을 때 잠깐 멈칫(경직)' 처리.
/// [필수 연결] NavMeshAgent, EnemySight, (옵션) 플레이어 Health
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
public class EnemyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySight sight;           // 시야 판정 담당
    [SerializeField] private NavMeshAgent agent;         // 길찾기 이동 담당
    [SerializeField] private Transform modelRoot;        // 회전(바라보기) 기준(없으면 자기 자신)
    [SerializeField] private Health playerHealth;        // 공격 대상의 Health(플레이어)

    [Header("Chase / Attack")]
    [SerializeField] private float attackRange = 2.0f;        // 공격 거리(미터)
    [SerializeField] private float attackIntervalSec = 1.0f;  // 공격 주기(초)
    [SerializeField] private float faceTurnSpeedDeg = 360.0f; // 바라보기 회전 속도(도/초)
    [SerializeField] private float damagePerHit = 10.0f;      // 한 번 공격 시 주는 피해량

    [Header("Stagger(경직)")]
    [SerializeField] private float staggerTime = 0.25f;  // 맞은 직후 멈칫 시간(초)

    // 내부 상태
    private float attackCooldown = 0.0f; // 남은 공격 대기 시간(초)
    private float staggerTimer = 0.0f;   // 남은 경직 시간(초)
    private Health selfHealth;           // 자기 Health(이벤트 구독용)

    private void Reset()
    {
        agent = GetComponent<NavMeshAgent>();
        selfHealth = GetComponent<Health>();
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

        if (selfHealth == null)
        {
            selfHealth = GetComponent<Health>();
        }

        // '맞았을 때' 이벤트 구독 -> 경직 처리
        if (selfHealth != null)
        {
            selfHealth.OnDamaged += HandleDamaged;
        }

        // 에이전트 회전은 직접 제어(더 자연스러운 바라보기)
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

    private void OnDestroy()
    {
        if (selfHealth != null)
        {
            selfHealth.OnDamaged -= HandleDamaged;
        }
    }

    private void Update()
    {
        // 쿨다운/타이머 업데이트
        if (attackCooldown > 0.0f)
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown < 0.0f)
            {
                attackCooldown = 0.0f;
            }
        }

        if (staggerTimer > 0.0f)
        {
            staggerTimer -= Time.deltaTime;
            if (staggerTimer < 0.0f)
            {
                staggerTimer = 0.0f;
            }
        }

        // 경직 중에는 멈춰 서서 아무 것도 안 함
        if (staggerTimer > 0.0f)
        {
            agent.isStopped = true;
            return;
        }

        bool canSee = false;

        if (sight != null)
        {
            canSee = sight.CanSeeTarget();
        }

        if (canSee == true)
        {
            ChaseAndFight();
            return;
        }

        // 보이진 않지만 최근에 본 위치가 있다면 그곳까지 이동(탐색)
        if (sight != null && sight.HasRecentSighting() == true)
        {
            Vector3 lastPos = sight.GetLastSeenPosition();
            MoveTo(lastPos);
            return;
        }

        // 아무것도 못 보면 정지
        agent.isStopped = true;
    }

    /// <summary>
    /// 플레이어가 보일 때: 접근하다가 사거리 안이면 공격.
    /// </summary>
    private void ChaseAndFight()
    {
        // 목표 위치는 시야가 보고 있는 대상의 현재 위치
        Vector3 targetPos = sight.GetLastSeenPosition();
        float distance = Vector3.Distance(transform.position, targetPos);

        // 사거리 밖이면 계속 이동
        if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(targetPos);
            FaceTarget(targetPos);
            return;
        }

        // 사거리 안이면 멈추고 공격 시도
        agent.isStopped = true;
        FaceTarget(targetPos);

        if (attackCooldown <= 0.0f)
        {
            DoAttack(targetPos);
            attackCooldown = attackIntervalSec;
        }
    }

    /// <summary>
    /// 특정 지점으로 이동하고 바라보기 회전을 보정.
    /// </summary>
    private void MoveTo(Vector3 worldPos)
    {
        agent.isStopped = false;
        agent.SetDestination(worldPos);
        FaceTarget(worldPos);
    }

    /// <summary>
    /// 모델을 목표 지점으로 부드럽게 회전시킨다.
    /// </summary>
    private void FaceTarget(Vector3 worldPos)
    {
        Vector3 to = worldPos - modelRoot.position; // 모델에서 목표까지 벡터
        to.y = 0.0f;                                 // 바닥면에서만 회전

        if (to.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        Quaternion targetRot = Quaternion.LookRotation(to, Vector3.up);
        float t = Mathf.Min(1.0f, (faceTurnSpeedDeg * Mathf.Deg2Rad) * Time.deltaTime);
        modelRoot.rotation = Quaternion.Slerp(modelRoot.rotation, targetRot, t);
    }

    /// <summary>
    /// 아주 단순한 근접 공격: 플레이어 Health에 즉시 피해를 준다.
    /// 실제 게임에서는 애니메이션 이벤트/히트박스/쿨타임 등으로 정교화.
    /// </summary>
    private void DoAttack(Vector3 targetPos)
    {
        if (playerHealth != null)
        {
            Vector3 hitPoint = targetPos; // 대략 플레이어 위치
            Vector3 hitNormal = (transform.position - targetPos).normalized; // 적 -> 플레이어 반대 방향

            // DamageInfo를 사용하면 이벤트 흐름(Hitmarker 등)에 자연스럽게 연결됨
            DamageInfo info = new DamageInfo(gameObject, damagePerHit, hitPoint, hitNormal, false);
            playerHealth.ApplyDamage(info);
        }

        Debug.Log("[Enemy] Attack!");
    }

    /// <summary>
    /// 적이 맞았을 때 호출되어 '경직'을 건다.
    /// </summary>
    private void HandleDamaged(DamageInfo info, float oldHp, float newHp)
    {
        staggerTimer = staggerTime;
    }
}
