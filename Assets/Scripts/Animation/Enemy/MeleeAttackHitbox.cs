using UnityEngine;

/// <summary>
/// [설치] 적 손/무기 위치의 자식 오브젝트(빈 오브젝트)에 부착
/// [핵심] '아주 짧은 시간' 동안 원형 범위를 검사하여 플레이어가 있으면 데미지를 준다.
///       애니메이션 이벤트(휘두르는 순간)에서 TryHitOnce()를 호출한다.
/// [필수] 플레이어 Health, 레이어/태그 세팅
/// </summary>
public class MeleeAttackHitbox : MonoBehaviour
{
    [SerializeField] private float radius = 1.2f;           // 타격 범위(미터)
    [SerializeField] private float damage = 12.0f;          // 데미지
    [SerializeField] private LayerMask targetMask;          // 플레이어가 속한 레이어

    /// <summary>
    /// 애니메이션 이벤트에서 호출: 지금 이 프레임에 한 번만 맞추기.
    /// </summary>
    public void TryHitOnce()
    {
        Debug.Log("Enter TryHitOnce");
        Collider[] cols = Physics.OverlapSphere(transform.position, radius, targetMask, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < cols.Length; ++i)
        {
            Health hp = cols[i].GetComponentInParent<Health>();
            if (hp != null)
            {
                Vector3 hitPoint = cols[i].ClosestPoint(transform.position);
                Vector3 hitNormal = (cols[i].transform.position - transform.position).normalized;

                DamageInfo info = new DamageInfo(gameObject, damage, hitPoint, hitNormal, false);
                hp.ApplyDamage(info);
            }
        }
    }

    // 장면에서 범위 확인용
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
