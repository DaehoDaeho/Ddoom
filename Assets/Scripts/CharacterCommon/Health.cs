using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float maxHp = 100.0f;

    [SerializeField]
    private float currentHp = 100.0f;

    void Awake()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(float amount, Vector3 hitPoint, Vector3 hitNormal)
    {
        currentHp -= amount;

        Debug.Log("[Health] Take Damage: " + amount + ", HP: " + currentHp + " (" + gameObject.name + ")");

        if(currentHp <= 0.0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("[Health] Died: " + gameObject.name);
        Destroy(gameObject);
    }

    public float GetMaxHp()
    {
        return maxHp;
    }

    public float GetCurrentHp()
    {
        return currentHp;
    }
}
