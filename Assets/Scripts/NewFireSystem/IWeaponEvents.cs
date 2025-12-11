using UnityEngine;

public interface IWeaponEvents
{
    void OnFired();                           // 발사 성공이 일어났을 때
    void OnReloadStarted();                   // 재장전이 시작되었을 때
    void OnReloadFinished(int newMag, int newReserve);  // 재장전이 끝났을 때 새 탄창과 예비를 전달
}
