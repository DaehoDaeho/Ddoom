using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 무기의 현재 퍼짐(도)을 읽어 화면 십자선 4개의 간격을 조절.
/// </summary>
public class CrosshairUI : MonoBehaviour
{
    [SerializeField]
    private HitscanWeapon weapon; // 퍼짐 정보를 읽을 무기
    
    [SerializeField]
    private RectTransform upBar;
    
    [SerializeField]
    private RectTransform downBar;
    
    [SerializeField]
    private RectTransform leftBar;
    [SerializeField]
    private RectTransform rightBar;

    [SerializeField]
    private float baseGap = 12.0f;     // 기본 간격(px)
    
    [SerializeField]
    private float pixelsPerDegree = 6.0f; // 퍼짐 1도당 늘어날 픽셀
    
    [SerializeField]
    private float smooth = 15.0f;      // 보간 속도

    private float currentGap; // 현재 반영 중인 간격(px)

    private void Update()
    {
        if (weapon == null)
        {
            return;
        }

        float spread = weapon.GetEffectiveSpreadDeg();         // 최근 퍼짐(도)
        float targetGap = baseGap + (spread * pixelsPerDegree); // 목표 간격(px)
        float t = 1.0f - Mathf.Exp(-smooth * Time.deltaTime);  // 지수 보간

        currentGap = Mathf.Lerp(currentGap, targetGap, t);
        ApplyGap(currentGap);
    }

    private void ApplyGap(float gap)
    {
        if (upBar != null)
        {
            upBar.anchoredPosition = new Vector2(0.0f, gap);
        }
        
        if (downBar != null)
        {
            downBar.anchoredPosition = new Vector2(0.0f, -gap);
        }

        if (leftBar != null)
        {
            leftBar.anchoredPosition = new Vector2(-gap, 0.0f);
        }

        if (rightBar != null)
        {
            rightBar.anchoredPosition = new Vector2(gap, 0.0f);
        }
    }
}
