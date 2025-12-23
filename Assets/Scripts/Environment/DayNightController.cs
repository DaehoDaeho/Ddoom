using UnityEngine;

public class DayNightController : MonoBehaviour
{
    [SerializeField] private float startHour = 12.0f;
    [SerializeField] private float secondsPerGameHour = 10.0f;  // 게임 시간 1시간이 현실의 몇 초인지.
    [SerializeField] private bool loop24Hours = true;   // 24시간 반복 여부.

    [SerializeField] private Light sunLight;
    [SerializeField] private float sunYaw = 30.0f;  // 태양이 도는 축의 좌우 방향 고정 값.

    [SerializeField] private Material skyboxTemplate;   // 하늘 머티리얼.
    [SerializeField] private string skyboxExposureProperty = "_Exposure";
    [SerializeField] private float nightExposure = 0.6f;
    [SerializeField] private float dayExposure = 1.2f;

    private float currentHour;  // 현재 시간.
    private Material runtimeSkybox;

    private void Awake()
    {
        currentHour = startHour;

        if(skyboxTemplate != null)
        {
            runtimeSkybox = new Material(skyboxTemplate);
            RenderSettings.skybox = runtimeSkybox;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AdvanceTime();
        UpdateSunRotation();
        UpdateSkybox();
    }

    public float GetCurrentHour()
    {
        return currentHour;
    }

    /// <summary>
    /// 시간 값을 낮 비율(0~1)로 바꾼다.
    /// 6시에서 0으로 시작, 12시에 1, 18시에 다시 0으로.
    /// </summary>
    /// <param name="hour"></param>
    /// <returns></returns>
    float ComputeDayFactor01(float hour)
    {
        // 6 ~ 18시 사이에서만 낮으로 간주.
        if(hour < 6.0f || hour > 18.0f)
        {
            return 0.0f;
        }

        if(hour <= 12.0f)
        {
            // 6~12 : 0~1
            float t = (hour - 6.0f) / 6.0f;
            return Mathf.Clamp01(t);
        }
        else
        {
            // 12~18 : 1~0
            float t = (hour - 12.0f) / 6.0f;
            return Mathf.Clamp01(t);
        }
    }

    /// <summary>
    /// 현재 시간으로 태양 회전을 계산해 적용한다.
    /// </summary>
    void UpdateSunRotation()
    {
        if(sunLight == null)
        {
            return;
        }

        float t = currentHour / 24.0f;  // 0~1 사이의 값으로 환산.
        float pitch = t * 360.0f - 90.0f;   // 시작 위치를 아래로 이동.

        sunLight.transform.rotation = Quaternion.Euler(pitch, sunYaw, 0.0f);
    }

    /// <summary>
    /// 시간을 진행시킨다.
    /// secondsPerGameHour 값이 작을수록 시간이 빨리 간다.
    /// </summary>
    void AdvanceTime()
    {
        float hourPerSecond = 1.0f / secondsPerGameHour;    // 초 당 몇 시간 진행되는지.
        currentHour += Time.deltaTime * hourPerSecond;

        if(loop24Hours == true)
        {
            if(currentHour >= 24.0f)
            {
                currentHour -= 24.0f;
            }

            if(currentHour < 0.0f)
            {
                currentHour += 24.0f;
            }
        }
        else
        {
            currentHour = Mathf.Clamp(currentHour, 0.0f, 24.0f);
        }
    }

    /// <summary>
    /// 현재 시간으로 하늘 값을 바꾼다.
    /// </summary>
    void UpdateSkybox()
    {
        if(runtimeSkybox == null)
        {
            return;
        }

        // 낮 비율 계산 : 6시 ~ 18시를 낮으로 가정하고 0~1 사이의 값으로 보간.
        float day01 = ComputeDayFactor01(currentHour);

        float exposure = Mathf.Lerp(nightExposure, dayExposure, day01);
        if(runtimeSkybox.HasProperty(skyboxExposureProperty) == true)
        {
            runtimeSkybox.SetFloat(skyboxExposureProperty, exposure);
        }
    }

    public float GetDaysFactor01()
    {
        return ComputeDayFactor01(currentHour);
    }
}
