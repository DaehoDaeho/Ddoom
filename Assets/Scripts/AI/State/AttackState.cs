using UnityEngine;

/// <summary>
/// [상태] 공격: 사거리 안에서 주기적으로 피해를 준다.
/// </summary>
public class AttackState : BaseEnemyState
{
    private bool requestedThisCycle = false; // 중복 요청 방지
    
    public override void Enter(EnemyContext ctx)
    {
        // 첫 프레임에 바라보기 보정
        EnemySight sight = ctx.GetSight();
        if (sight != null)
        {
            ctx.FaceTarget(sight.GetLastSeenPosition());
        }

        requestedThisCycle = false;
    }

    public override void Tick(EnemyContext ctx, EnemyStateMachine fsm)
    {
        ctx.TickCommon(Time.deltaTime);

        EnemySight sight = ctx.GetSight();
        if (sight == null)
        {
            fsm.ChangeState(new PatrolState());
            return;
        }

        bool canSee = sight.CanSeeTarget();
        Vector3 pos = sight.GetLastSeenPosition();
        float dist = Vector3.Distance(ctx.transform.position, pos);

        if (canSee == false)
        {
            // 시야를 잃으면 의심으로 하향
            fsm.ChangeState(new SuspiciousState());

            requestedThisCycle = false;

            return;
        }

        if (dist > ctx.GetAttackRange())
        {
            // 사거리 밖이면 추적
            fsm.ChangeState(new ChaseState());

            requestedThisCycle = false;

            return;
        }

        // 사거리 안: 바라보고 공격 시도
        ctx.FaceTarget(pos);
        ctx.TryAttack(pos);

        // 공격 쿨다운은 EnemyContext.TryAttack가 관리했지만,
        // 오늘은 '애니메이션 -> 이벤트' 흐름이므로 애니메이션만 요청.
        if (requestedThisCycle == false)
        {
            EnemyAttackSwitcher sw = ctx.GetComponent<EnemyAttackSwitcher>();
            if (sw != null)
            {
                sw.RequestAttackAnimation();
            }

            //requestedThisCycle = true;
        }
    }
}