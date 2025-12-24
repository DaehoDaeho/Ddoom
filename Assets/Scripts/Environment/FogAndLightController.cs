using UnityEngine;
using TMPro;
using System.Text;

public class FogAndLightController : MonoBehaviour
{
    [SerializeField] private DayNightController dayNightController;
    [SerializeField] private Light sunLight;

    [SerializeField] private bool fogEnabled = true;    // Fog를 사용할지 여부.
    [SerializeField] private FogMode fogMode = FogMode.Exponential;

    [SerializeField] private Color dayFogColor = new Color(0.78f, 0.85f, 0.95f, 1.0f);
    [SerializeField] private Color nightFogcolor = new Color(0.05f, 0.07f, 0.10f, 1.0f);

    [SerializeField] private float dayFogDensity = 0.0025f;
    [SerializeField] private float nightFogDensity = 0.015f; 

    [SerializeField] private float daySunIntensity = 1.2f;
    [SerializeField] private float nightSunIntensity = 0.05f;
    [SerializeField] private float dayAmbientIntensity = 1.0f;
    [SerializeField] private float nightAmbientIntensity = 0.25f;

    [SerializeField] private float smoothSpeed = 3.0f;  // 변화 속도.

    [SerializeField] private TMP_Text textInfo;

    private Color currentFogColor;  // 현재 안개의 색상.
    private float currentFogDensity;    // 현재 안개의 잔하기.
    private float currentSunIntensity;  // 현재 태양 빛의 세기.
    private float currentAmbientIntensity;  // 현재 환경광의 세기.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        ApplyInstantFromCurrentTime();
    }

    // Update is called once per frame
    void Update()
    {
        if(dayNightController == null)
        {
            return;
        }

        float dayFactor = dayNightController.GetDaysFactor01();

        Color targetFogColor = Color.Lerp(nightFogcolor, dayFogColor, dayFactor);
        float targetFogDensity = Mathf.Lerp(nightFogDensity, dayFogDensity, dayFactor);
        float targetSunIntensity = Mathf.Lerp(nightSunIntensity, daySunIntensity, dayFactor);
        float targetAmbientIntensity = Mathf.Lerp(nightAmbientIntensity, dayAmbientIntensity, dayFactor);

        float step = Time.deltaTime * smoothSpeed;

        currentFogColor = Color.Lerp(nightFogcolor, dayFogColor, step);
        currentFogDensity = Mathf.Lerp(nightFogDensity, dayFogDensity, step * 0.05f);
        currentSunIntensity = Mathf.Lerp(nightSunIntensity, daySunIntensity, step * 0.5f);
        currentAmbientIntensity = Mathf.Lerp(nightAmbientIntensity, dayAmbientIntensity, step * 0.5f);

        ApplyRenderSettings();

        //if(textInfo != null)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("FogColor:").Append(currentFogColor).Append("\n").Append("FogDensity: ").Append(currentFogDensity).Append("\n").Append("SunIntensity: ").
        //        Append(currentSunIntensity).Append("\n").Append("AmbientIntensity: ").Append(currentAmbientIntensity);
        //    textInfo.text = sb.ToString();
        //}
    }

    /// <summary>
    /// 시작 시점에 현재 시간 기준으로 값을 즉시 맞춘다.
    /// </summary>
    void ApplyInstantFromCurrentTime()
    {
        if(dayNightController == null)
        {
            return;
        }

        float dayFactor = dayNightController.GetDaysFactor01();

        currentFogColor = Color.Lerp(nightFogcolor, dayFogColor, dayFactor);
        currentFogDensity = Mathf.Lerp(nightFogDensity, dayFogDensity, dayFactor);
        currentSunIntensity = Mathf.Lerp(nightSunIntensity, daySunIntensity, dayFactor);
        currentAmbientIntensity = Mathf.Lerp(nightAmbientIntensity, dayAmbientIntensity, dayFactor);

        ApplyRenderSettings();
    }

    /// <summary>
    /// 계산된 값을 전역 렌더 세팅에 적용한다.
    /// </summary>
    void ApplyRenderSettings()
    {
        RenderSettings.fog = fogEnabled;
        RenderSettings.fogMode = fogMode;
        RenderSettings.fogColor = currentFogColor;

        RenderSettings.fogDensity = currentFogDensity;

        RenderSettings.ambientIntensity = currentAmbientIntensity;

        if(sunLight != null)
        {
            sunLight.intensity = currentSunIntensity;
        }
    }

    public float GetCurrentFogDensity()
    {
        return currentFogDensity;
    }

    public float GetCurrentSunIntensity()
    {
        return currentSunIntensity;
    }
}
