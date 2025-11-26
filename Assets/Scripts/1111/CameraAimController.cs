using UnityEngine;

/// <summary>
/// 우클릭 조준 중일 때 카메라 FOV를 부드럽게 줄여서 줌 느낌 제공.
/// </summary>
public class CameraAimController : MonoBehaviour
{
    [SerializeField]
    private WeaponInputReader inputReader; // 조준 상태 참조
    
    [SerializeField]
    private float hipFov = 60.0f;          // 평소 시야
    
    [SerializeField]
    private float adsFov = 45.0f;          // 조준 시 시야
    
    [SerializeField]
    private float lerpSpeed = 10.0f;       // 보간 속도(클수록 빠름)

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = hipFov;
    }

    private void LateUpdate()
    {
        if (inputReader == null)
        {
            return;
        }

        bool aiming = inputReader.IsAimPressed == true;
        float target = aiming == true ? adsFov : hipFov;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, target, 1.0f - Mathf.Exp(-lerpSpeed * Time.deltaTime));
    }
}
