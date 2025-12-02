using UnityEngine;

/// <summary>
/// [상태] 추적: 보이는 동안 대상에게 접근.
/// </summary>
public class ChaseState : BaseEnemyState
{
    public override void Enter(EnemyContext ctx)
    {
        EnemySight sight = ctx.GetSight();
        if (sight != null)
        {
            ctx.MoveTo(sight.GetLastSeenPosition());
        }
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
        if (canSee == false)
        {
            // 보이지 않으면 의심으로 하향
            bool hasNoise = ctx.GetHearing() != null && ctx.GetHearing().HasRecentNoise() == true;
            bool hasLastSeen = sight.HasRecentSighting() == true;
            if (hasNoise == true || hasLastSeen == true)
            {
                fsm.ChangeState(new SuspiciousState());
                return;
            }

            fsm.ChangeState(new PatrolState());
            return;
        }

        Vector3 pos = sight.GetLastSeenPosition();
        float dist = Vector3.Distance(ctx.transform.position, pos);

        if (dist <= ctx.GetAttackRange())
        {
            fsm.ChangeState(new AttackState());
            return;
        }

        ctx.MoveTo(pos);
    }
}
