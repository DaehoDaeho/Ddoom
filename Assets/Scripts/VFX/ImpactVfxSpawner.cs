using UnityEngine;

public class ImpactVfxSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject defaultImpactPrefab;

    [SerializeField]
    private float impactLifetime = 1.0f;

    public void SpawnImpact(Vector3 position, Vector3 normal)
    {
        if(defaultImpactPrefab == null)
        {
            return;
        }

        Quaternion rot = Quaternion.LookRotation(normal, Vector3.up);
        GameObject go = Instantiate(defaultImpactPrefab, position, rot);
        if(go != null)
        {
            Destroy(go, impactLifetime);
        }
    }
}
