using UnityEngine;

/// <summary>
/// [설치] 적 루트
/// [핵심] 적의 현재 상황(이동 중/의심/공격 등)을 Animator 파라미터로 전달해
///       걷기/두리번/공격 애니메이션이 자연스럽게 전환되게 한다.
/// [필수] Animator, EnemyContext, EnemyStateMachine
/// </summary>
[RequireComponent(typeof(Animator))]
public class EnemyAnimationBridge : MonoBehaviour
{
    [SerializeField] private EnemyContext context;     // 적의 공용 정보(위치, 이동 등)
    [SerializeField] private EnemyStateMachine fsm;    // 상태 변경 감지용
    [SerializeField] private Animator animator;        // 애니메이터

    // Animator 파라미터 이름(애니메이터 창에서 같은 이름으로 만들어두기)
    [SerializeField] private string paramIsMoving = "isMoving";
    [SerializeField] private string paramIsSuspicious = "isSuspicious";
    [SerializeField] private string paramAttackTrigger = "attackTrigger";

    private Vector3 lastPos;                           // 이동 여부 판정용

    private void Reset()
    {
        animator = GetComponent<Animator>();
        context = GetComponent<EnemyContext>();
        fsm = GetComponent<EnemyStateMachine>();
    }

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (context == null)
        {
            context = GetComponent<EnemyContext>();
        }

        if (fsm == null)
        {
            fsm = GetComponent<EnemyStateMachine>();
        }

        lastPos = transform.position;
    }

    private void Update()
    {
        // 1) 이동 중인지 간단 판정(이전 프레임 대비 위치 변화)
        float moved = Vector3.Distance(transform.position, lastPos);
        bool moving = moved > 0.01f;

        if (animator != null)
        {
            animator.SetBool(paramIsMoving, moving);
        }

        lastPos = transform.position;

        // 2) 의심 상태 여부: 간단히 Hearing/LastSeen 표시를 묶어서 판단
        bool suspicious = false;
        if (context != null)
        {
            bool heard = context.GetHearing() != null &&
                context.GetHearing().HasRecentNoise() == true;
            bool saw = context.GetSight() != null &&
                context.GetSight().HasRecentSighting() == true &&
                context.GetSight().CanSeeTarget() == false;
            suspicious = heard == true || saw == true;
        }

        if (animator != null)
        {
            animator.SetBool(paramIsSuspicious, suspicious);
        }
    }

    /// <summary>
    /// 공격을 시작할 때 애니메이터에 트리거를 쏴서 공격 애니메이션을 재생하게 한다.
    /// (상태에서 이 함수를 호출)
    /// </summary>
    public void PlayAttackOnce()
    {
        if (animator != null)
        {
            animator.ResetTrigger(paramAttackTrigger);
            animator.SetTrigger(paramAttackTrigger);
        }
    }
}
