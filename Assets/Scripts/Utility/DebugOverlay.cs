using UnityEngine;

public class DebugOverlay : MonoBehaviour
{
    [SerializeField] private KeyCode toggleKey = KeyCode.F2;

    [SerializeField] private DayNightController dayNightController;
    [SerializeField] private FogAndLightController fogAndLightController;
    [SerializeField] private DayNightSightTuner dayNightSightTuner;

    [SerializeField] private bool visibleAtStart = true;    // 시작 시 표시 여부.
    [SerializeField] private float refreshInterval = 0.25f; // 갱신 간격.

    private bool visible;   // 현재 표시 상태.
    private float elapsed;  // 타이머 변수.
    private string cachedText;  // 화면에 출력할 문자열 캐싱.

    private void Awake()
    {
        visible = visibleAtStart;
        elapsed = 0.0f;
        cachedText = "";
    }

    // Update is called once per frame
    void Update()
    {
        bool pressed = Input.GetKeyDown(toggleKey);
        if (pressed == true)
        {
            visible = !visible;
        }

        if(visible == false)
        {
            return;
        }

        elapsed += Time.deltaTime;
        if(elapsed >= refreshInterval)
        {
            elapsed = 0.0f;
            RebuildText();
        }
    }

    void RebuildText()
    {
        string line1;
        string line2;
        string line3;

        if(dayNightController != null)
        {
            float hour = dayNightController.GetCurrentHour();
            float day01 = dayNightController.GetDaysFactor01();
            line1 = "시간 " + hour.ToString() + " 낮비율 " + day01.ToString();
        }
        else
        {
            line1 = "시간 연결 없음";
        }

        if(fogAndLightController != null)
        {
            float fogDensity = fogAndLightController.GetCurrentFogDensity();
            float sunIntensity = fogAndLightController.GetCurrentSunIntensity();
            line2 = "안개 " + fogDensity.ToString() + " 태양빛 " + sunIntensity.ToString();
        }
        else
        {
            line2 = "안개 연결 없음";
        }

        if (dayNightSightTuner != null)
        {
            float multiplier = dayNightSightTuner.GetCurrentMultiplier();
            line3 = "탐지배율 " + multiplier.ToString();
        }
        else
        {
            line3 = "탐지배율 연결 없음";
        }

        // \n -> 개행문자 : 다음줄로 건너뛴다.
        cachedText = line1 + "\n" + line2 + "\n" + line3 + "\n" + "F2:디버그 토글";
    }

    void OnGUI()
    {
        if(visible == false)
        {
            return;
        }

        GUI.skin.label.fontSize = 32;
        GUI.Label(new Rect(10, 100, 1000, 200), cachedText);
    }
}
