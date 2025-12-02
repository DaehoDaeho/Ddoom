using UnityEngine;

/// <summary>
/// 공통 유틸을 담은 기본 상태 베이스. 필요에 따라 상속해 사용.
/// </summary>
public abstract class BaseEnemyState : IEnemyState
{
    public virtual void Enter(EnemyContext ctx)
    {
    }

    public virtual void Tick(EnemyContext ctx, EnemyStateMachine fsm)
    {
    }

    public virtual void Exit(EnemyContext ctx)
    {
    }

    /// <summary>
    /// 시야 결과에 따라 즉시 Chase/Attack으로 승격할지 판단한다.
    /// true를 반환하면 호출한 쪽은 더 진행하지 않는 것이 좋다.
    /// </summary>
    protected bool TryImmediateChaseOrAttack(EnemyContext ctx, EnemyStateMachine fsm)
    {
        EnemySight sight = ctx.GetSight();
        if (sight == null)
        {
            return false;
        }

        bool canSee = sight.CanSeeTarget();
        if (canSee == true)
        {
            Vector3 targetPos = sight.GetLastSeenPosition();
            float dist = Vector3.Distance(ctx.transform.position, targetPos);

            if (dist <= ctx.GetAttackRange())
            {
                fsm.ChangeState(new AttackState());
                return true;
            }
            else
            {
                fsm.ChangeState(new ChaseState());
                return true;
            }
        }

        return false;
    }
}