using UnityEngine;

public interface IDamageable
{
    void TakeDamage(GameObject src, float amount, Vector3 hitPoint, Vector3 hitNormal);
}
