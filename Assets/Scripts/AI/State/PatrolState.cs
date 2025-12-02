using UnityEngine;

/// <summary>
/// [상태] 순찰: 웨이포인트를 순환.
/// </summary>
public class PatrolState : BaseEnemyState
{
    public override void Enter(EnemyContext ctx)
    {
        // 즉시 이동 시작(경로가 없다면 정지)
        PatrolRoute route = ctx.GetPatrol();
        if (route == null || route.HasPoints() == false)
        {
            ctx.Stop();
            return;
        }

        Transform p = route.GetCurrent();
        if (p != null)
        {
            ctx.MoveTo(p.position);
        }
    }

    public override void Tick(EnemyContext ctx, EnemyStateMachine fsm)
    {
        // 공통 타이머
        ctx.TickCommon(Time.deltaTime);

        // 보이면 즉시 승격
        bool changed = TryImmediateChaseOrAttack(ctx, fsm);
        if (changed == true)
        {
            return;
        }

        // 소리나 마지막 시야 기억이 있으면 Suspicious
        bool hasNoise = ctx.GetHearing() != null && ctx.GetHearing().HasRecentNoise() == true;
        bool hasLastSeen = ctx.GetSight() != null && ctx.GetSight().HasRecentSighting() == true;

        if (hasNoise == true || hasLastSeen == true)
        {
            fsm.ChangeState(new SuspiciousState());
            return;
        }

        // 웨이포인트 이동
        PatrolRoute route = ctx.GetPatrol();
        if (route == null || route.HasPoints() == false)
        {
            ctx.Stop();
            return;
        }

        Transform p = route.GetCurrent();
        if (p == null)
        {
            ctx.Stop();
            return;
        }

        if (ctx.IsReached(p.position) == true)
        {
            route.MoveNext();
            Transform next = route.GetCurrent();
            if (next != null)
            {
                ctx.MoveTo(next.position);
            }
        }
        else
        {
            ctx.MoveTo(p.position);
        }
    }
}