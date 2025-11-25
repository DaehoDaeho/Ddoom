using UnityEngine;
using System;

public class DamageEventBus : MonoBehaviour
{
    // 정적 변수
    public static DamageEventBus Instance;

    public event Action<GameObject, DamageInfo, bool> OnAnyDamageDealt;

    private void Awake()
    {
        Instance = this;
    }

    public void RaiseHit(GameObject source, DamageInfo info, bool killed)
    {
        if(OnAnyDamageDealt != null)
        {
            OnAnyDamageDealt.Invoke(source, info, killed);
        }
    }
}
