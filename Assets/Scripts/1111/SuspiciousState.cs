using UnityEngine;

/// <summary>
/// [상태] 의심: 최근 들은 소리/마지막 본 위치로 이동하며 확인.
/// </summary>
public class SuspiciousState : BaseEnemyState
{
    private float timer = 0.0f;

    public override void Enter(EnemyContext ctx)
    {
        timer = ctx.GetSuspicionTime();
        MoveToGoal(ctx);
    }

    public override void Tick(EnemyContext ctx, EnemyStateMachine fsm)
    {
        ctx.TickCommon(Time.deltaTime);

        bool changed = TryImmediateChaseOrAttack(ctx, fsm);
        if (changed == true)
        {
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0.0f)
        {
            fsm.ChangeState(new PatrolState());
            return;
        }

        Vector3 goal = GetGoal(ctx);
        if (ctx.IsReached(goal) == true)
        {
            ctx.Stop(); // 도착해서 주변 살피기
            return;
        }

        ctx.MoveTo(goal);
    }

    private void MoveToGoal(EnemyContext ctx)
    {
        Vector3 goal = GetGoal(ctx);
        ctx.MoveTo(goal);
    }

    private Vector3 GetGoal(EnemyContext ctx)
    {
        bool hasNoise = ctx.GetHearing() != null && ctx.GetHearing().HasRecentNoise() == true;
        if (hasNoise == true)
        {
            return ctx.GetHearing().GetLastHeardPosition();
        }

        EnemySight sight = ctx.GetSight();
        if (sight != null)
        {
            return sight.GetLastSeenPosition();
        }

        return ctx.transform.position;
    }
}