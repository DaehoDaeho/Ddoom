using UnityEngine;

[DefaultExecutionOrder(1000)]
/// <summary>
/// 카메라 피벗(CameraPivot)의 로컬 회전에 작은 반동 각도를 더해 '뷰 킥'을 구현.
/// 발사 시 AddKick(yaw, pitch)를 호출하고, LateUpdate에서 서서히 0으로 복귀.
/// </summary>
public class PlayerViewKick : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;          // CameraPivot
    [SerializeField] private float returnSpeedDegPerSec = 90.0f; // 복귀 속도(도/초)
    [SerializeField] private float maxPitchKickDeg = 10.0f;  // 최대 상하 반동 제한
    [SerializeField] private float maxYawKickDeg = 5.0f;     // 최대 좌우 반동 제한

    private float currentPitchKick = 0.0f; // 현재 적용 중인 피치 반동
    private float currentYawKick = 0.0f;   // 현재 적용 중인 요 반동

    private void LateUpdate()
    {
        if (cameraPivot == null)
        {
            return;
        }

        // 1) 현재 반동 각도를 0을 향해 서서히 되돌림
        float step = returnSpeedDegPerSec * Time.deltaTime; // 이번 프레임 복귀량(도)
        currentPitchKick = MoveToward(currentPitchKick, 0.0f, step);
        currentYawKick = MoveToward(currentYawKick, 0.0f, step);

        // 2) CameraPivot의 기존 로컬 회전에 반동을 '추가'로 곱한다
        Quaternion baseRot = cameraPivot.localRotation;
        Quaternion kickRot = Quaternion.Euler(currentPitchKick, currentYawKick, 0.0f);
        cameraPivot.localRotation = baseRot * kickRot;
    }

    /// <summary>
    /// 발사 시 호출: 반동 각도를 누적. 제한을 넘어가지 않도록 클램프.
    /// </summary>
    public void AddKick(float yawDeg, float pitchDeg)
    {
        currentPitchKick += pitchDeg; // 위로 톡 튀게 +값 사용
        currentYawKick += yawDeg;

        // 과도 누적 방지
        currentPitchKick = Mathf.Clamp(currentPitchKick, -maxPitchKickDeg, maxPitchKickDeg);
        currentYawKick = Mathf.Clamp(currentYawKick, -maxYawKickDeg, maxYawKickDeg);
    }

    private float MoveToward(float value, float target, float maxDelta)
    {
        if (value < target)
        {
            return Mathf.Min(value + maxDelta, target);
        }
        else
        {
            return Mathf.Max(value - maxDelta, target);
        }
    }
}
