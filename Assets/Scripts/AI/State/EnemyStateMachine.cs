using UnityEngine;

/// <summary>
/// [설치] 적 루트 오브젝트에 부착
/// [핵심] 현재 상태를 보관하고 Enter/Tick/Exit를 호출하며, 상태 전환을 담당한다.
/// [필수 연결] EnemyContext
/// </summary>
[RequireComponent(typeof(EnemyContext))]
public class EnemyStateMachine : MonoBehaviour
{
    [SerializeField] private EnemyContext context;

    private IEnemyState currentState;
    [SerializeField] private string strCurrentState;

    private void Reset()
    {
        context = GetComponent<EnemyContext>();
    }

    private void Awake()
    {
        if (context == null)
        {
            context = GetComponent<EnemyContext>();
        }
    }

    private void OnEnable()
    {
        // 기본 시작 상태: 순찰
        ChangeState(new PatrolState());
    }

    private void Update()
    {
        if (currentState != null && context != null)
        {
            currentState.Tick(context, this);
        }
    }

    /// <summary>
    /// 상태를 교체한다. 기존 상태 Exit → 새 상태 Enter 순서로 호출.
    /// </summary>
    public void ChangeState(IEnemyState next)
    {
        if (currentState != null && context != null)
        {
            currentState.Exit(context);
        }

        currentState = next;

        if (currentState != null && context != null)
        {
            currentState.Enter(context);
        }

        strCurrentState = currentState.ToString();
    }
}
