using UnityEngine;

public class HitscanWeapon : WeaponBase
{
    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private Transform firePoint;

    [SerializeField]
    private float damage = 20.0f;

    [SerializeField]
    private float maxDistance = 100.0f;

    [SerializeField]
    private float spreadAngleDeg = 0.0f;    // ÆÛÁü °¢µµ

    [SerializeField]
    private LayerMask hitMask;

    [SerializeField]
    private LayerMask ignoreMask = 0;

    [SerializeField]
    private ImpactVfxSpawner impactVfx;

    [SerializeField]
    private ParticleSystem muzzleFlash;

    void PlayMuzzleflash()
    {
        if(muzzleFlash != null)
        {
            if(muzzleFlash.isPlaying == false)
            {
                muzzleFlash.Play();
            }
            else
            {
                muzzleFlash.Stop();
                muzzleFlash.Play();
            }
        }
    }

    Vector3 ApplySpread(Vector3 dir, float angleDeg)
    {
        float yaw = Random.Range(-angleDeg, angleDeg);
        float pitch = Random.Range(-angleDeg, angleDeg);

        Quaternion spreadRot = Quaternion.Euler(pitch, yaw, 0.0f);
        Vector3 spreadDir = spreadRot * dir;
        return spreadDir.normalized;
    }

    protected override void Fire()
    {
        if(cameraTransform == null)
        {
            return;
        }

        Vector3 origin = cameraTransform.position;
        Vector3 direction = cameraTransform.forward;

        if(spreadAngleDeg > 0.0f)
        {
            direction = ApplySpread(direction, spreadAngleDeg);
        }

        Ray ray = new Ray(origin, direction);
        RaycastHit hit;

        bool didHit = Physics.Raycast(ray, out hit, maxDistance, hitMask, QueryTriggerInteraction.Ignore);

        PlayMuzzleflash();

        if(didHit == true)
        {
            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
            if(damageable != null)
            {
                damageable.TakeDamage(hit.collider.gameObject, damage, hit.point, hit.normal);
            }

            if(impactVfx != null)
            {
                impactVfx.SpawnImpact(hit.point, hit.normal);
            }

            Debug.DrawLine(origin, hit.point, Color.red, 0.2f, false);
        }
    }
}
