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