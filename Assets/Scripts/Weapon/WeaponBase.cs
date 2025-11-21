using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField]
    protected int magazineSize = 30;    // 한 탄창의 장탄 수.

    [SerializeField]
    protected int ammoInMagazine = 30;  // 현재 탄창의 탄 수.

    [SerializeField]
    protected int reserveAmmo = 90; // 예비 탄약 개수.

    [SerializeField]
    protected float fireIntervalSec = 0.1f; // 발사 간격.

    [SerializeField]
    protected float reloadTimeSec = 1.5f;   // 재장전 시간.

    protected float fireCooldown = 0.0f;
    protected float reloadTimer = 0.0f;
    protected bool isReloading = false;
    
    public bool TryFire()
    {
        if(isReloading == true)
        {
            return false;
        }

        if(fireCooldown > 0.0f)
        {
            return false;
        }

        if(ammoInMagazine <= 0)
        {
            OnDryFire();
            return false;
        }

        --ammoInMagazine;
        fireCooldown = fireIntervalSec;

        Fire();

        return true;
    }

    public bool TryReload()
    {
        if(isReloading == true)
        {
            return false;
        }

        if(ammoInMagazine >= magazineSize)
        {
            return false;
        }

        if(reserveAmmo <= 0)
        {
            return false;
        }

        isReloading = true;
        reloadTimer = reloadTimeSec;
        OnReloadStart();
        return true;
    }

    public void ManualTick(float deltaTime)
    {
        if(fireCooldown > 0.0f)
        {
            fireCooldown -= deltaTime;
            if(fireCooldown < 0.0f)
            {
                fireCooldown = 0.0f;
            }
        }

        if(isReloading == true)
        {
            reloadTimer -= deltaTime;
            if(reloadTimer <= 0.0f)
            {
                FinishReload();
            }
        }
    }

    protected virtual void FinishReload()
    {
        isReloading = false;
        reloadTimer = 0.0f;

        int need = magazineSize - ammoInMagazine;
        int load = Mathf.Min(need, reserveAmmo);

        ammoInMagazine += need;
        reserveAmmo -= need;

        OnReloadComplete();
    }

    protected abstract void Fire();

    protected virtual void OnDryFire()
    {
        Debug.Log("[Weapon] Dry Fire (empty magazine)");
    }

    protected virtual void OnReloadStart()
    {
        Debug.Log("[Weapon] Reload Started");
    }

    protected virtual void OnReloadComplete()
    {
        Debug.Log("[Weapon] Reload Completed, AmmoInMag: " + ammoInMagazine + ", Reserve: " + reserveAmmo);
    }

    public int GetAmmoInMagazine()
    {
        return ammoInMagazine;
    }

    public int GetReserveAmmo()
    {
        return reserveAmmo;
    }

    public bool GetIsReloading()
    {
        return isReloading;
    }

    public float GetFireCooldown()
    {
        return fireCooldown;
    }
}
