using UnityEngine;

/// <summary>
/// 상체 뼈를 목표 방향으로 제한 각도 내에서 부드럽게 회전시킨다.
/// - 과한 회전 방지를 위해 yaw/pitch 한도를 둔다.
/// - LateUpdate에서 실행해 애니메이션 이후에 보정.
/// </summary>
public class UpperBodyAimRig : MonoBehaviour
{
    [SerializeField] private EnemyContext context;    // 목표 좌표 제공
    [SerializeField] private Transform spine;         // 상체(가슴) 뼈
    [SerializeField] private float maxYawDeg = 35.0f; // 좌우 한도
    [SerializeField] private float maxPitchUpDeg = 20.0f;   // 위로 한도
    [SerializeField] private float maxPitchDownDeg = 25.0f; // 아래로 한도
    [SerializeField] private float aimLerp = 10.0f;   // 부드러운 따라가기 속도
    [SerializeField] private float weight = 0.7f;     // 적용 비율(0~1)

    private Quaternion baseLocalRotation; // 애니메이션 기준 회전 저장

    private void Reset()
    {
        context = GetComponent<EnemyContext>();
    }

    private void Awake()
    {
        if (spine != null)
        {
            baseLocalRotation = spine.localRotation;
        }
    }

    private void LateUpdate()
    {
        if (spine == null)
        {
            return;
        }

        // 기본 회전(애니메이션 출력)을 기준으로 시작
        Quaternion targetLocal = baseLocalRotation;

        Vector3 aimPoint = spine.position + transform.forward * 10.0f;

        if (context != null && context.GetSight() != null)
        {
            aimPoint = context.GetSight().GetLastSeenPosition();
        }

        // 월드 방향 계산 후, 현재 상체 기준으로 제한
        Vector3 dirWorld = (aimPoint - spine.position);
        if (dirWorld.sqrMagnitude <= 0.0001f)
        {
            dirWorld = transform.forward;
        }

        dirWorld.Normalize();

        // 상체의 로컬 기준 벡터로 변환
        Vector3 dirLocal = spine.parent != null
            ? spine.parent.InverseTransformDirection(dirWorld)
            : dirWorld;

        // 로컬 기준에서 yaw/pitch 추출
        float yaw = Mathf.Atan2(dirLocal.x, dirLocal.z) * Mathf.Rad2Deg;         // 좌우
        float pitch = -Mathf.Asin(dirLocal.y) * Mathf.Rad2Deg;                    // 위아래(간단 근사)

        // 각도 제한
        yaw = Mathf.Clamp(yaw, -maxYawDeg, maxYawDeg);
        float minPitch = -maxPitchUpDeg;
        float maxPitch = maxPitchDownDeg;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // 제한된 yaw/pitch를 로컬 회전으로
        Quaternion rotYaw = Quaternion.AngleAxis(yaw, Vector3.up);
        Quaternion rotPitch = Quaternion.AngleAxis(pitch, Vector3.right);
        Quaternion limitedLocal = baseLocalRotation * rotYaw * rotPitch;

        // 가중치와 보간으로 부드럽게
        Quaternion blended = Quaternion.Slerp(spine.localRotation, limitedLocal, weight);
        Quaternion result = Quaternion.Slerp(spine.localRotation, blended, Mathf.Clamp01(aimLerp * Time.deltaTime));

        spine.localRotation = result;
    }
}
