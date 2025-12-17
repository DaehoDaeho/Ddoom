using UnityEngine;

public class ImpactVfxSpawner : MonoBehaviour
{
    [SerializeField]
    private float impactLifetime = 1.0f;

    [SerializeField]
    private SimplePool pool;

    [SerializeField]
    private SimplePool poolDecal;

    public void SpawnImpact(Vector3 position, Vector3 normal)
    {
        if(pool == null)
        {
            return;
        }

        Quaternion rot = Quaternion.LookRotation(normal, Vector3.up);
        PooledObject go = pool.Rent(position, rot, impactLifetime);
        PooledObject go2 = poolDecal.Rent(position+normal*0.01f, rot, impactLifetime);
    }
}
