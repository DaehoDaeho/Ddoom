using UnityEngine;

public class DryWeapon : WeaponBase
{
    protected override void Fire()
    {
        Debug.Log("[Weapon] Fire! AmmoInMag: " + ammoInMagazine + ", Reserve: " + reserveAmmo);
    }
}
