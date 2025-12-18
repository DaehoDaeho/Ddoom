using UnityEngine;

public interface IAmmoProvider
{
    int GetInMag();
    int GetReserve();
    void SetInMag(int amount);
    void SetReserve(int amount);
}
