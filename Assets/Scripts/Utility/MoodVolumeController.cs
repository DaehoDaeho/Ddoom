using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MoodVolumeController : MonoBehaviour
{
    [SerializeField] private Volume targetVolume;
    [SerializeField] private float changeSpeed = 2.0f;

    [SerializeField] private KeyCode dayKey = KeyCode.Alpha1;
    [SerializeField] private KeyCode eveningKey = KeyCode.Alpha2;
    [SerializeField] private KeyCode neutralKey = KeyCode.Alpha3;

    [Header("낮의 목표 값")]
    [SerializeField] private float dayPostExposure = 0.2f;  // 밝기 목표 값.
    [SerializeField] private float dayContrast = 5.0f;  // 대비 목표 값.
    [SerializeField] private float daySaturation = 0.0f;    // 색 강도 목표 값.
    [SerializeField] private float dayBloomIntensity = 0.3f;   // 블룸의 목표 값.
    [SerializeField] private float dayVignetteItensity = 0.08f; // 비넷의 목표 값.

    [Header("저녁의 목표 값")]
    [SerializeField] private float eveningPostExposure = -0.2f;  // 밝기 목표 값.
    [SerializeField] private float eveningContrast = 12.0f;  // 대비 목표 값.
    [SerializeField] private float eveningSaturation = -5.0f;    // 색 강도 목표 값.
    [SerializeField] private float eveningBloomIntensity = 0.6f;   // 블룸의 목표 값.
    [SerializeField] private float eveningVignetteItensity = 0.18f; // 비넷의 목표 값.

    [Header("기본 목표 값")]
    [SerializeField] private float neutralPostExposure = 0.0f;  // 밝기 목표 값.
    [SerializeField] private float neutralContrast = 0.0f;  // 대비 목표 값.
    [SerializeField] private float neutralSaturation = 0.0f;    // 색 강도 목표 값.
    [SerializeField] private float neutralBloomIntensity = 0.25f;   // 블룸의 목표 값.
    [SerializeField] private float neutralVignetteItensity = 0.1f; // 비넷의 목표 값.

    private ColorAdjustments colorAdjusstments;
    private Bloom bloom;
    private Vignette vignette;

    private float targetPostExposure;
    private float targetContrast;
    private float targetSaturation;
    private float targetBloomIntensity;
    private float targetVignetteIntensity;

    void Awake()
    {
        bool hasColor = targetVolume.profile.TryGet(out colorAdjusstments);
        bool hasBloom = targetVolume.profile.TryGet(out bloom);
        bool hasVignette = targetVolume.profile.TryGet(out vignette);

        if(hasColor == false)
        {
            colorAdjusstments = null;
        }

        if(hasBloom == false)
        {
            bloom = null;
        }

        if(hasVignette == false)
        {
            vignette = null;
        }

        SetTargetsNeutral();
        ApplyTargetInstant();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        MoveCurrentTarget();
    }

    void MoveCurrentTarget()
    {
        float t = changeSpeed * Time.deltaTime;

        if(t > 1.0f)
        {
            t = 1.0f;
        }

        if(colorAdjusstments != null)
        {
            float currentExposure = colorAdjusstments.postExposure.value;
            float nextExposure = Mathf.Lerp(currentExposure, targetPostExposure, t);
            colorAdjusstments.postExposure.value = nextExposure;

            float currentContrast = colorAdjusstments.contrast.value;
            float nextContrast = Mathf.Lerp(currentContrast, targetContrast, t);
            colorAdjusstments.contrast.value = nextContrast;

            float currentSaturation = colorAdjusstments.saturation.value;
            float nextSaturation = Mathf.Lerp(currentSaturation, targetSaturation, t);
            colorAdjusstments.saturation.value = nextSaturation;
        }

        if(bloom != null)
        {
            float currentBloom = bloom.intensity.value;
            float nextBloom = Mathf.Lerp(currentBloom, targetBloomIntensity, t);
            bloom.intensity.value = nextBloom;
        }

        if(vignette != null)
        {
            float currentVignette = vignette.intensity.value;
            float nextVignette = Mathf.Lerp(currentVignette, targetVignetteIntensity, t);
            vignette.intensity.value = nextVignette;
        }
    }

    void ApplyTargetInstant()
    {
        if(colorAdjusstments != null)
        {
            colorAdjusstments.postExposure.value = targetPostExposure;
            colorAdjusstments.contrast.value = targetContrast;
            colorAdjusstments.saturation.value = targetSaturation;
        }

        if(bloom != null)
        {
            bloom.intensity.value = targetBloomIntensity;
        }

        if(vignette != null)
        {
            vignette.intensity.value = targetVignetteIntensity;
        }
    }

    void EnsureOverridesActive()
    {
        if(colorAdjusstments != null)
        {
            colorAdjusstments.active = true;
        }

        if(bloom != null)
        {
            bloom.active = true;
        }

        if(vignette != null)
        {
            vignette.active = true;
        }
    }

    void SetTargetsDay()
    {
        targetPostExposure = dayPostExposure;
        targetContrast = dayContrast;
        targetSaturation = daySaturation;
        targetBloomIntensity = dayBloomIntensity;
        targetVignetteIntensity = dayVignetteItensity;

        EnsureOverridesActive();
    }

    void SetTargetsEvening()
    {
        targetPostExposure = eveningPostExposure;
        targetContrast = eveningContrast;
        targetSaturation = eveningSaturation;
        targetBloomIntensity = eveningBloomIntensity;
        targetVignetteIntensity = eveningVignetteItensity;

        EnsureOverridesActive();
    }

    void SetTargetsNeutral()
    {
        targetPostExposure = neutralPostExposure;
        targetContrast = neutralContrast;
        targetSaturation = neutralSaturation;
        targetBloomIntensity = neutralBloomIntensity;
        targetVignetteIntensity = neutralVignetteItensity;

        EnsureOverridesActive();
    }

    void HandleInput()
    {
        if(Input.GetKeyDown(dayKey) == true)
        {
            SetTargetsDay();
        }

        if(Input.GetKeyDown(eveningKey) == true)
        {
            SetTargetsEvening();
        }

        if(Input.GetKeyDown(neutralKey) == true)
        {
            SetTargetsNeutral();
        }
    }
}
