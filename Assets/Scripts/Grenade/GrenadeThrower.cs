using UnityEngine;

public class GrenadeThrower : MonoBehaviour
{
    [SerializeField] private Camera viewCamera;
    [SerializeField] private Grenade grenadePrefab;

    [SerializeField] private float throwSpeed = 15.0f;  // ≈ı√¥ º”µµ.
    [SerializeField] private float upwardBonus = 2.5f;
    [SerializeField] private int maxGrenades = 3;
    [SerializeField] private bool ignoreGrenadeCount = false;

    [SerializeField] private KeyCode throwKey = KeyCode.G;

    private int remain;

    private void Reset()
    {
        if(viewCamera == null)
        {
            viewCamera = Camera.main;
        }
    }

    private void Awake()
    {
        remain = maxGrenades;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(throwKey) == true)
        {
            TryThrow();
        }
    }

    public int GetRemain()
    {
        return remain;
    }

    public void Refill()
    {
        remain = maxGrenades;
    }

    void TryThrow()
    {
        if(ignoreGrenadeCount == false && remain <= 0)
        {
            return;
        }

        if(grenadePrefab == null || viewCamera == null)
        {
            return;
        }

        Vector3 origin = viewCamera.transform.position +
            viewCamera.transform.forward * 0.4f + viewCamera.transform.right * 0.1f;

        Vector3 dir = viewCamera.transform.forward;

        Grenade g = Instantiate(grenadePrefab, origin, Quaternion.identity);

        Rigidbody rb = g.GetComponent<Rigidbody>();
        if(rb != null)
        {
            Vector3 velocity = dir * throwSpeed + Vector3.up * upwardBonus;
            rb.linearVelocity = velocity;
        }

        --remain;
    }
}
