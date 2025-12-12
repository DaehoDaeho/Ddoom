using UnityEngine;
using System;

public class ProjectPhysicsConfig : MonoBehaviour
{
    [Header("일반 마스크")]
    [SerializeField] private LayerMask bulletHitMask;   // 명중 판정용 레이어 마스크.
    [SerializeField] private LayerMask sightMask;   // 시야 검사용 레이어 마스크.
    [SerializeField] private LayerMask interactMask;    // 상호작용 검사용 레이어 마스크.

    [Serializable]
    private struct IgnorePair
    {
        public string layerAName;
        public string layerBName;
    }

    [Header("충돌 무시 레이어 이름 쌍")]
    [SerializeField] private IgnorePair[] ignorePairs;

    private static ProjectPhysicsConfig instance;   // 싱글톤 패턴을 위한 인스턴스.

    private void Awake()
    {
        instance = this;

        for(int i=0; i<ignorePairs.Length; ++i)
        {
            int a = LayerMask.NameToLayer(ignorePairs[i].layerAName);
            int b = LayerMask.NameToLayer(ignorePairs[i].layerBName);

            if(a == -1)
            {
                Debug.LogWarning($"레이어 이름이 없습니다: {ignorePairs[i].layerAName}");
                continue;
            }

            if (b == -1)
            {
                Debug.LogWarning($"레이어 이름이 없습니다: {ignorePairs[i].layerBName}");
                continue;
            }

            Physics.IgnoreLayerCollision(a, b, true);
        }
    }

    public static ProjectPhysicsConfig Get()
    {
        return instance;
    }

    public LayerMask GetBulletHitMask()
    {
        return bulletHitMask;
    }

    public LayerMask GetSightMask()
    {
        return sightMask;
    }

    public LayerMask GetInteractMask()
    {
        return interactMask;
    }
}
