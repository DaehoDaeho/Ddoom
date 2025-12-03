using UnityEngine;

/// <summary>
/// [설치] 적의 총구/눈 위치(레이 시작점)에 부착
/// [핵심] 애니메이션 이벤트에서 TryShootOnce()가 호출되면,
///       바라보는 방향으로 레이캐스트를 날려 맞으면 데미지를 준다.
/// [필수] 플레이어 레이어, Health, hitMask
/// </summary>
public class RangedShooter : MonoBehaviour
{
    [SerializeField] private float damage = 8.0f;           // 1발 데미지
    [SerializeField] private float maxDistance = 60.0f;     // 사거리
    [SerializeField] private LayerMask hitMask = ~0;        // 맞출 대상 레이어
    [SerializeField] private Transform aimSource;            // 바라볼 기준(없으면 자기 자신)

    private void Awake()
    {
        if (aimSource == null)
        {
            aimSource = transform;
        }
    }

    /// <summary>
    /// 애니메이션 이벤트에서 호출: 한 번만 발사.
    /// </summary>
    public void TryShootOnce()
    {
        if (aimSource == null)
        {
            return;
        }

        Vector3 origin = aimSource.position;
        Vector3 dir = aimSource.forward;

        RaycastHit hit;
        bool didHit = Physics.Raycast(origin, dir, out hit, maxDistance, hitMask, QueryTriggerInteraction.Ignore);

        if (didHit == true)
        {
            Health hp = hit.collider.GetComponentInParent<Health>();
            if (hp != null)
            {
                DamageInfo info = new DamageInfo(gameObject, damage, hit.point, hit.normal, false);
                hp.ApplyDamage(info);
            }
        }

        Debug.DrawLine(origin, didHit == true ? hit.point : origin + dir * maxDistance, Color.cyan, 0.2f, false);
    }
}
