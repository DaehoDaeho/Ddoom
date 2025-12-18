using UnityEngine;

public interface IHealthProvider
{
    float GetCurrent();
    float GetMax();
    void SetCurrent(float hp);
}
