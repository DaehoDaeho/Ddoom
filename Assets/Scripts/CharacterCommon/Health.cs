using UnityEngine;
using System;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float maxHp = 100.0f;

    [SerializeField]
    private float currentHp = 100.0f;

    [SerializeField]
    private bool ignoreFriendlyFire = true;

    [SerializeField]
    private float invulnerableTimeOnHit = 0.2f;

    private float invulnerableRemain = 0.0f;

    public event Action<DamageInfo, float, float> OnDamaged;
    public event Action OnDied;

    private TeamMember team;

    void Awake()
    {
        currentHp = maxHp;
        team = GetComponent<TeamMember>();
    }

    void Update()
    {
        if(invulnerableRemain > 0.0f)
        {
            invulnerableRemain -= Time.deltaTime;
            if(invulnerableRemain < 0.0f)
            {
                invulnerableRemain = 0.0f;
            }
        }
    }

    public void TakeDamage(GameObject src, float amount, Vector3 hitPoint, Vector3 hitNormal)
    {
        //DamageInfo info = new DamageInfo(null, amount, hitPoint, hitNormal, false);
        DamageInfo info = new DamageInfo(src, amount, hitPoint, hitNormal, false);
        ApplyDamage(info);
    }

    public void ApplyDamage(DamageInfo info)
    {
        if(info == null)
        {
            return;
        }

        if (info.amount <= 0.0f)
        {
            return;
        }

        if(invulnerableRemain > 0.0f)
        {
            return;
        }

        if(ignoreFriendlyFire == true && IsFriendlyFire(info.source) == true)
        {
            return;
        }

        float oldHp = currentHp;
        currentHp -= info.amount;

        if(OnDamaged != null)
        {
            OnDamaged.Invoke(info, oldHp, currentHp);
        }

        if(info.source != null && DamageEventBus.Instance != null)
        {
            bool killed = currentHp <= 0.0f;
            DamageEventBus.Instance.RaiseHit(info.source, info, killed);
        }

        if(invulnerableTimeOnHit > 0.0f)
        {
            invulnerableRemain = invulnerableTimeOnHit;
        }

        if(currentHp <= 0.0f)
        {
            currentHp = 0.0f;

            if(OnDied != null)
            {
                OnDied.Invoke();
            }

            Destroy(gameObject);
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

    public bool IsInvulnerable()
    {
        return invulnerableRemain > 0.0f;
    }

    bool IsFriendlyFire(GameObject source)
    {
        if(source = null)
        {
            return false;
        }

        TeamMember srcTeam = source.GetComponent<TeamMember>();
        
        if(srcTeam == null || team == null)
        {
            return false;
        }

        return srcTeam.GetTeamId() == team.GetTeamId();
    }
}
