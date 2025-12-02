using UnityEngine;

/// <summary>
/// 상태가 지켜야 할 기본 규약.
/// </summary>
public interface IEnemyState
{
    /// <summary>상태에 진입할 때 한 번 호출.</summary>
    void Enter(EnemyContext ctx);

    /// <summary>매 프레임 호출. 필요시 상태 전환을 요청할 수 있다.</summary>
    void Tick(EnemyContext ctx, EnemyStateMachine fsm);

    /// <summary>상태에서 나갈 때 한 번 호출.</summary>
    void Exit(EnemyContext ctx);
}

/// <summary>
/// 공통 유틸을 담은 기본 상태 베이스. 필요에 따라 상속해 사용.
/// </summary>
public abstract class BaseEnemyState : IEnemyState
{
    public virtual void Enter(EnemyContext ctx) { }
    public virtual void Tick(EnemyContext ctx, EnemyStateMachine fsm) { }
    public virtual void Exit(EnemyContext ctx) { }

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
