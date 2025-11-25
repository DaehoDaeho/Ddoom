using UnityEngine;
using TMPro;

public class WeaponHUD : MonoBehaviour
{
    [SerializeField]
    private WeaponController weaponcontroller;

    [SerializeField]
    private TMP_Text magText;

    [SerializeField]
    private TMP_Text reserveText;

    private WeaponBase boundWeapon;

    private void OnEnable()
    {
        BindToCurrentWeapon();
    }

    private void OnDisable()
    {
        UnbindWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        if(weaponcontroller == null)
        {
            return;
        }

        WeaponBase current = weaponcontroller.GetCurrentWeapon();
        if(current != boundWeapon)
        {
            UnbindWeapon();
            boundWeapon = current;
            BindWeapon(boundWeapon);
        }
    }

    void BindToCurrentWeapon()
    {
        if(weaponcontroller == null)
        {
            return;
        }

        boundWeapon = weaponcontroller.GetCurrentWeapon();
        BindWeapon(boundWeapon);
    }

    void BindWeapon(WeaponBase weapon)
    {
        if(weapon == null)
        {
            return;
        }

        weapon.OnAmmoChanged += HandleAmmoChanged;
        HandleAmmoChanged(weapon.GetAmmoInMagazine(), weapon.GetReserveAmmo());
    }

    void UnbindWeapon()
    {
        if(boundWeapon != null)
        {
            boundWeapon.OnAmmoChanged -= HandleAmmoChanged;
            boundWeapon = null;
        }
    }

    void HandleAmmoChanged(int ammoInMag, int reserve)
    {
        if(magText != null)
        {
            magText.text = ammoInMag.ToString();
        }

        if(reserveText != null)
        {
            reserveText.text = reserve.ToString();
        }
    }
}
