using UnityEngine;

/// <summary>
/// [상태] 공격: 사거리 안에서 주기적으로 피해를 준다.
/// </summary>
public class AttackState : BaseEnemyState
{
    private bool requestedThisCycle = false; // 중복 요청 방지

    //=========================================================
    private float telegraphDuration = 0.3f;  // 예고 시간(초)
    private float orbitT = 0.0f;             // 대기 회전용 타이머

    private AttackTelegraph telegraph;
    private EnemyEngageAgent engage;
    private EnemyAttackSwitcher switcher;
    //=========================================================

    public override void Enter(EnemyContext ctx)
    {
        // 첫 프레임에 바라보기 보정
        EnemySight sight = ctx.GetSight();
        if (sight != null)
        {
            ctx.FaceTarget(sight.GetLastSeenPosition());
        }

        requestedThisCycle = false;

        //==================================================
        if (telegraph == null)
        {
            telegraph = ctx.GetComponent<AttackTelegraph>();
        }

        if (engage == null)
        {
            engage = ctx.GetComponent<EnemyEngageAgent>();
        }

        if (switcher == null)
        {
            switcher = ctx.GetComponent<EnemyAttackSwitcher>();
        }

        // 텔레그래프가 켜져 있었다면 안전하게 초기화
        if (telegraph != null && telegraph.IsPlaying() == true)
        {
            telegraph.Cancel();
        }
        //==================================================
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
            //===================================================
            if (telegraph != null && telegraph.IsPlaying() == true)
            {
                telegraph.Cancel();
            }
            //===================================================

            // 시야를 잃으면 의심으로 하향
            fsm.ChangeState(new SuspiciousState());

            requestedThisCycle = false;

            return;
        }

        if (dist > ctx.GetAttackRange())
        {
            //===================================================
            if (telegraph != null && telegraph.IsPlaying() == true)
            {
                telegraph.Cancel();
            }
            //===================================================

            // 사거리 밖이면 추적
            fsm.ChangeState(new ChaseState());

            requestedThisCycle = false;

            return;
        }

        //=======================================================
        // 토큰 체크: 없으면 대기/자리 잡기
        if (engage != null && engage.HasPermission() == false)
        {
            orbitT += Time.deltaTime;
            Vector3 orbit = engage.GetOrbitPointAround(pos, orbitT);
            ctx.MoveTo(orbit);
            return;
        }
        //=======================================================

        // 사거리 안: 바라보고 공격 시도
        ctx.FaceTarget(pos);
        ctx.TryAttack(pos);

        // 공격 쿨다운은 EnemyContext.TryAttack가 관리했지만,
        // 오늘은 '애니메이션 -> 이벤트' 흐름이므로 애니메이션만 요청.
        if (requestedThisCycle == false)
        {
            //========================================================
            // 텔레그래프 시작
            if (telegraph != null)
            {
                telegraph.BeginTelegraph(telegraphDuration);
            }

            requestedThisCycle = true;
            return;
            //========================================================
        }

        //==========================================================
        // 텔레그래프 완료되었으면 공격 애니 요청
        if (telegraph != null)
        {
            if (telegraph.IsPlaying() == true)
            {
                return; // 아직 예고 중
            }

            if (telegraph.IsCompleted() == true)
            {
                if (switcher != null)
                {
                    switcher.RequestAttackAnimation(); // 데미지는 애니 이벤트에서 처리
                }

                // 다음 사이클 준비(필요시 간격 타이머를 추가해도 됨)
                requestedThisCycle = false;
                return;
            }
        }
        else
        {
            // 텔레그래프가 없다면 바로 애니 시작(비권장이지만 안전 처리)
            if (switcher != null)
            {
                switcher.RequestAttackAnimation();
            }
            requestedThisCycle = false;
        }
        //==========================================================
    }
}