using UnityEngine;

public enum FireMode
{
    Semi,     // 단발
    Auto      // 연사
}

[CreateAssetMenu(menuName = "FPS/Weapon Data", fileName = "WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("발사 방식")]
    public FireMode fireMode;                 // 발사 방식  단발 또는 연사

    [Header("발사 간격과 피해")]
    public float fireInterval = 0.12f;        // 두 발 사이의 시간 간격
    public float damage = 20.0f;              // 한 발당 피해량

    [Header("탄약")]
    public int magCapacity = 15;              // 한 탄창에 들어가는 탄 수
    public int startReserve = 45;             // 시작 시 보유 예비 탄약 수

    [Header("재장전")]
    public float reloadTime = 1.65f;          // 재장전에 걸리는 시간

    [Header("사정과 판정")]
    public float maxDistance = 120.0f;        // 판정이 닿는 최대 거리
    public LayerMask hitMask = ~0;            // 판정에 반응할 레이어
}
