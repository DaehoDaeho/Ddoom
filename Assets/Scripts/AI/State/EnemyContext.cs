using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// [설치] 적 루트 오브젝트에 부착
/// [핵심] 적 AI 상태들이 공유하는 모든 참조와 수치를 제공하고,
///       이동/회전/목적지 보정(금지 영역 회피 + NavMesh 샘플)을 도와준다.
/// [필수 연결] NavMeshAgent, EnemySight, EnemyHearing, playerHealth, patrol 등
/// </summary>
public class EnemyContext : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private EnemySight sight;
    [SerializeField] private EnemyHearing hearing;
    [SerializeField] private PatrolRoute patrol;
    [SerializeField] private Transform modelRoot;
    [SerializeField] private Health playerHealth;

    [Header("Chase / Attack")]
    [SerializeField] private float attackRange = 2.0f;
    [SerializeField] private float attackIntervalSec = 1.0f;
    [SerializeField] private float damagePerHit = 10.0f;
    [SerializeField] private float faceTurnSpeedDeg = 360.0f;

    [Header("Suspicious")]
    [SerializeField] private float reachThreshold = 0.8f;
    [SerializeField] private float suspicionTime = 3.0f;

    // 내부 상태 공유용
    private float attackCooldown = 0.0f;

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

    /// <summary>
    /// 매 프레임 쿨다운 등 공통 타이머를 갱신한다.
    /// </summary>
    public void TickCommon(float deltaTime)
    {
        if (attackCooldown > 0.0f)
        {
            attackCooldown -= deltaTime;
            if (attackCooldown < 0.0f)
            {
                attackCooldown = 0.0f;
            }
        }
    }

    /// <summary>NavMeshAgent 반환.</summary>
    public NavMeshAgent GetAgent()
    {
        return agent;
    }

    /// <summary>시야 컴포넌트.</summary>
    public EnemySight GetSight()
    {
        return sight;
    }

    /// <summary>청각 컴포넌트.</summary>
    public EnemyHearing GetHearing()
    {
        return hearing;
    }

    /// <summary>순찰 경로.</summary>
    public PatrolRoute GetPatrol()
    {
        return patrol;
    }

    /// <summary>플레이어 체력.</summary>
    public Health GetPlayerHealth()
    {
        return playerHealth;
    }

    /// <summary>공격 사거리.</summary>
    public float GetAttackRange()
    {
        return attackRange;
    }

    /// <summary>의심 상태 유지 시간.</summary>
    public float GetSuspicionTime()
    {
        return suspicionTime;
    }

    /// <summary>의심 도달 판정 거리.</summary>
    public float GetReachThreshold()
    {
        return reachThreshold;
    }

    /// <summary>바라보기 회전 속도(도/초).</summary>
    public float GetFaceTurnSpeedDeg()
    {
        return faceTurnSpeedDeg;
    }

    /// <summary>
    /// 지정 지점으로 이동 시작(금지 영역/네비 보정 포함) + 바라보기 보정.
    /// </summary>
    public void MoveTo(Vector3 worldPos)
    {
        if (agent == null)
        {
            return;
        }

        Vector3 clamped = ClampToAllowed(worldPos, transform.position);
        agent.isStopped = false;
        agent.SetDestination(clamped);
        FaceTarget(clamped);
    }

    /// <summary>
    /// 목적지에 도달했는지(수평 거리 기준) 반환.
    /// </summary>
    public bool IsReached(Vector3 worldPos)
    {
        float dist = Vector3.Distance(transform.position, worldPos);
        if (dist <= reachThreshold)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 에이전트를 멈춘다.
    /// </summary>
    public void Stop()
    {
        if (agent != null)
        {
            agent.isStopped = true;
        }
    }

    /// <summary>
    /// 주어진 위치를 향해 모델을 부드럽게 회전시킨다.
    /// </summary>
    public void FaceTarget(Vector3 worldPos)
    {
        if (modelRoot == null)
        {
            return;
        }

        Vector3 to = worldPos - modelRoot.position;
        to.y = 0.0f;

        if (to.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        Quaternion targetRot = Quaternion.LookRotation(to, Vector3.up);
        float t = Mathf.Min(1.0f, (faceTurnSpeedDeg * Mathf.Deg2Rad) * Time.deltaTime);
        modelRoot.rotation = Quaternion.Slerp(modelRoot.rotation, targetRot, t);
    }

    /// <summary>
    /// 플레이어에게 근접 공격을 시도한다(쿨다운 체크 포함).
    /// 성공 시 데미지를 주고 쿨다운을 초기화한다.
    /// </summary>
    public void TryAttack(Vector3 targetPos)
    {
        if (attackCooldown > 0.0f)
        {
            return;
        }

        if (playerHealth != null)
        {
            Vector3 hitPoint = targetPos;
            Vector3 hitNormal = (transform.position - targetPos).normalized;
            DamageInfo info = new DamageInfo(gameObject, damagePerHit, hitPoint, hitNormal, false);
            playerHealth.ApplyDamage(info);
        }

        attackCooldown = attackIntervalSec;
    }

    /// <summary>
    /// 목적지를 NavMesh에 스냅하고, 금지 영역이면 경계선으로 끌어당겨 반환한다.
    /// 성능: 1회 NavMesh.SamplePosition + 간단한 캡슐 거리 계산.
    /// </summary>
    public Vector3 ClampToAllowed(Vector3 desired, Vector3 from)
    {
        // 1) NavMesh 스냅
        NavMeshHit hit;
        Vector3 nav = desired;
        if (NavMesh.SamplePosition(desired, out hit, 2.0f, NavMesh.AllAreas) == true)
        {
            nav = hit.position;
        }

        // 2) 금지 영역 회피
        Vector3 safe = NavNoEntryVolume.ClampOutside(nav, from);
        return safe;
    }
}
