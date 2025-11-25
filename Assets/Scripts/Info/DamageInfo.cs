using UnityEngine;

public class DamageInfo
{
    public GameObject source;
    public float amount;
    public Vector3 hitPoint;
    public Vector3 hitNormal;
    public bool isCritical;

    public DamageInfo(GameObject src, float amt, Vector3 pt, Vector3 nrm, bool crlt)
    {
        source = src;
        amount = amt;
        hitPoint = pt;
        hitNormal = nrm;
        isCritical = crlt;
    }
}
