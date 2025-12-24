using UnityEngine;
using System;

public class Health : MonoBehaviour, IDamageable, IHealthProvider
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
    public event Action<float, float> OnChangedHP;

    private TeamMember team;

    private bool invincible = false;    // 무적 상태인지 여부.

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

        //if(ignoreFriendlyFire == true && IsFriendlyFire(info.source) == true)
        //{
        //    return;
        //}

        // 무적 상태면 대미지 적용을 하지 않는다.
        if (invincible == true)
        {
            return;
        }

        float oldHp = currentHp;
        currentHp -= info.amount;

        if(OnDamaged != null)
        {
            OnDamaged.Invoke(info, oldHp, currentHp);
        }

        if(OnChangedHP != null)
        {
            OnChangedHP.Invoke(currentHp, maxHp);
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

        //TeamMember srcTeam = source.GetComponent<TeamMember>();
        
        //if(srcTeam == null || team == null)
        //{
        //    return false;
        //}

        //return srcTeam.GetTeamId() == team.GetTeamId();
        return false;
    }

    public float GetCurrent()
    {
        return currentHp;
    }

    public float GetMax()
    {
        return maxHp;
    }

    public void SetCurrent(float hp)
    {
        currentHp = hp;

        if(OnChangedHP != null)
        {
            OnChangedHP.Invoke(currentHp, maxHp);
        }
    }

    /// <summary>
    /// hp를 가득 채우기.
    /// </summary>
    public void SetHPToMax()
    {
        currentHp = maxHp;

        if (OnChangedHP != null)
        {
            OnChangedHP.Invoke(currentHp, maxHp);
        }
    }

    /// <summary>
    /// 무적 상태 토글.
    /// </summary>
    public void ToggleInvincible()
    {
        invincible = !invincible;
    }
}
