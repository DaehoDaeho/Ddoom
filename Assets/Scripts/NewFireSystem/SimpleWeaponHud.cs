using UnityEngine;
using TMPro;

public class SimpleWeaponHud : MonoBehaviour, IWeaponEvents
{
    [SerializeField] private WeaponControllerPlus weapon;         // 무기 제어 참조
    [SerializeField] private TMP_Text textMag;                    // 탄창 수 표시
    [SerializeField] private TMP_Text textReserve;                // 예비 탄 표시
    [SerializeField] private TMP_Text textState;                  // 상태 표시

    private void Update()
    {
        if (weapon == null)
        {
            return;
        }

        // 매 프레임 기본 숫자 갱신
        textMag.text = weapon.GetMag().ToString();
        textReserve.text = weapon.GetReserve().ToString();

        // 상태 문구 갱신
        if (weapon.GetIsReloading() == true)
        {
            textState.text = "Reloading";
        }
        else
        {
            textState.text = "";
        }
    }

    public void OnFired()
    {
        // 발사 시 즉시 텍스트를 재확인 : 지금은 단순히 숫자만 다시 반영
        if (weapon != null)
        {
            textMag.text = weapon.GetMag().ToString();
        }
    }

    public void OnReloadStarted()
    {
        textState.text = "Reloading";
    }

    public void OnReloadFinished(int newMag, int newReserve)
    {
        textMag.text = newMag.ToString();
        textReserve.text = newReserve.ToString();
        textState.text = "";
    }
}
