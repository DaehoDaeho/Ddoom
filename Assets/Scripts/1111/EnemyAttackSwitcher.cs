using UnityEngine;

/// <summary>
/// [설치] 적 루트
/// [핵심] 이 적이 근거리형인지 원거리형인지 설정하고,
///       공격 애니메이션 트리거와 실제 타격 함수를 한 곳에서 관리한다.
/// [필수] EnemyAnimationBridge, (선택) MeleeAttackHitbox, (선택) RangedShooter
/// </summary>
public class EnemyAttackSwitcher : MonoBehaviour
{
    public enum AttackType
    {
        Melee,
        Ranged
    }

    [SerializeField] private AttackType attackType = AttackType.Melee;
    [SerializeField] private EnemyAnimationBridge animBridge;
    [SerializeField] private MeleeAttackHitbox meleeHitbox;
    [SerializeField] private RangedShooter rangedShooter;

    private void Reset()
    {
        animBridge = GetComponent<EnemyAnimationBridge>();
    }

    /// <summary>
    /// 상태(AttackState)에서 호출: "공격을 시작해!" 요청.
    /// 여기서는 애니메이션만 시작한다. 실제 데미지는 애니메이션 이벤트에서 처리.
    /// </summary>
    public void RequestAttackAnimation()
    {
        if (animBridge != null)
        {
            animBridge.PlayAttackOnce();
        }
    }

    // ===== 아래 두 함수는 "애니메이션 이벤트"에서 호출 =====
    // 공격 클립 안 특정 프레임에서 함수를 넣어줄 것.

    /// <summary>
    /// 근거리 타격 타이밍: 휘두르는 순간에 애니메이션 이벤트로 호출.
    /// </summary>
    public void OnMeleeHit()
    {
        if (attackType == AttackType.Melee && meleeHitbox != null)
        {
            meleeHitbox.TryHitOnce();
        }
    }

    /// <summary>
    /// 원거리 발사 타이밍: 방아쇠 순간에 애니메이션 이벤트로 호출.
    /// </summary>
    public void OnShoot()
    {
        if (attackType == AttackType.Ranged && rangedShooter != null)
        {
            rangedShooter.TryShootOnce();
        }
    }
}
