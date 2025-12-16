using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float fuseSeconds = 2.0f;  // 지연 시간.
    [SerializeField] private float radius = 4.0f;   // 폭발 반경.
    [SerializeField] private float maxDamage = 120.0f;
    [SerializeField] private LayerMask hitMask = ~0;
    [SerializeField] private bool destroyAfterExplode = true;   // 폭발 후 파괴할 것인지 여부.

    [SerializeField] private GameObject explodePrefab;
    [SerializeField] private float fxLife = 2.0f;

    private float elapsed;
    private bool exploded;

    // Update is called once per frame
    void Update()
    {
        if(exploded == true)
        {
            return;
        }

        elapsed += Time.deltaTime;

        if(elapsed >= fuseSeconds)
        {
            Explode();
        }
    }

    public void ForceExplode()
    {
        elapsed = fuseSeconds;
    }

    void Explode()
    {
        if(exploded == true)
        {
            return;
        }

        exploded = true;

        Vector3 center = transform.position;

        if(explodePrefab != null)
        {
            GameObject fx = Instantiate(explodePrefab, center, Quaternion.identity);
            Destroy(fx, fxLife);
        }

        Collider[] hits = Physics.OverlapSphere(center, radius, hitMask, QueryTriggerInteraction.Ignore);

        for(int i=0; i<hits.Length; ++i)
        {
            Collider c = hits[i];
            if (c == null)
            {
                continue;
            }

            Transform t = c.transform;
            IDamageable dmg = t.GetComponentInParent<IDamageable>();
            if(dmg == null)
            {
                continue;
            }

            float dist = Vector3.Distance(center, t.position);
            if(dist > radius)
            {
                continue;
            }

            float k = 1.0f - Mathf.Clamp01(dist / radius);
            float amount = maxDamage * k;
            dmg.TakeDamage(gameObject, amount, c.transform.position, Vector3.zero);
        }

        if(destroyAfterExplode == true)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
