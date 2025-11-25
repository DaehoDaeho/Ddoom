using System;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    private WeaponInputReader inputReader;

    [SerializeField]
    private WeaponBase currentWeapon;

    [SerializeField]
    private bool fireAsHold = true;

    [SerializeField]
    private bool useManualTick = true;

    private bool wasTriggerPressedPrev;

    // Update is called once per frame
    void Update()
    {
        if(useManualTick == false)
        {
            Tick(Time.deltaTime);
        }
    }

    public void ManualTick(float deltaTime)
    {
        if(useManualTick == true)
        {
            Tick(deltaTime);
        }
    }

    void Tick(float deltaTime)
    {
        if(currentWeapon != null)
        {
            currentWeapon.ManualTick(deltaTime);
        }

        if(inputReader == null || currentWeapon == null)
        {
            return;
        }

        if(inputReader.WasReloadPressedThisFrame == true)
        {
            currentWeapon.TryReload();
        }

        if(fireAsHold == true)
        {
            if(inputReader.IsTriggerPressed == true)
            {
                currentWeapon.TryFire();
            }
        }
        else
        {
            bool pressed = inputReader.IsTriggerPressed;
            if(pressed == true && wasTriggerPressedPrev == false)
            {
                currentWeapon.TryFire();
            }
            wasTriggerPressedPrev = pressed;
        }
    }

    public void SetCurrentWeapon(WeaponBase weapon)
    {
        if(weapon != null)
        {
            currentWeapon = weapon;
        }
    }

    public WeaponBase GetCurrentWeapon()
    {
        return currentWeapon;
    }
}
