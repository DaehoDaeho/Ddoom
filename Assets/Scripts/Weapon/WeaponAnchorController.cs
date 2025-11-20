using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponAnchorController : MonoBehaviour
{
    [SerializeField]
    private PlayerMotor playerMotor;

    [SerializeField]
    private Transform cameraTransform;

    [Header("Mouse Delta Setting")]
    [SerializeField]
    private float swayPosAmount = 0.02f;

    [SerializeField]
    private float swayRotAmountDeg = 2.0f;

    [SerializeField]
    private float swaySensitivity = 1.0f;

    [SerializeField]
    private float swaySmoothing = 12.0f;

    [Header("Move Reaction Setting")]
    [SerializeField]
    private float bobAmplitude = 0.02f;

    [SerializeField]
    private float bobFrequency = 7.0f;

    [SerializeField]
    private float bobSideAmplityde = 0.01f;

    [SerializeField]
    private float bobSpeedScale = 1.0f;

    private Vector3 baseLocalPos;
    private Quaternion baseLocalRot;

    private float bobPhase;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baseLocalPos = transform.localPosition;
        baseLocalRot = transform.localRotation;
    }

    private void LateUpdate()
    {
        Vector2 mouseDelta = ReadMouseDelta();
        ApplySway(mouseDelta, Time.deltaTime);
        ApplyBob(Time.deltaTime);
    }

    Vector2 ReadMouseDelta()
    {
        if(Mouse.current != null)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            return delta;
        }

        float dx = Input.GetAxis("Mouse X");
        float dy = Input.GetAxis("Mouse Y");

        return new Vector2(dx * 100.0f, dy * 100.0f);
    }

    void ApplySway(Vector2 mouseDelta, float deltaTime)
    {
        float sx = mouseDelta.x * swaySensitivity * deltaTime;
        float sy = mouseDelta.y * swaySensitivity * deltaTime;

        Vector3 targetPosOffset = new Vector3(-sx * swayPosAmount, -sy * swayPosAmount, 0.0f);

        float targetPitch = sy * swayRotAmountDeg;
        float targetYaw = -sx * swayRotAmountDeg;
        float targetRoll = -sx * swayRotAmountDeg;

        Quaternion targetRotOffset = Quaternion.Euler(targetPitch, targetYaw, targetRoll);

        float t = 1.0f - Mathf.Exp(-swaySmoothing * deltaTime);

        Vector3 desiredPos = baseLocalPos + targetPosOffset;
        transform.localPosition = Vector3.Lerp(transform.localPosition, desiredPos, t);

        Quaternion desiredRot = baseLocalRot * targetRotOffset;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, desiredRot, t);
    }

    void ApplyBob(float deltaTime)
    {
        float speedScale = 1.0f;

        if(playerMotor != null)
        {
            //speedScale = bobSpeedScale;
            speedScale = playerMotor.GetHozizontalSpeed() == 0.0f ? 0.25f : playerMotor.GetHozizontalSpeed();
        }

        if(speedScale <= 0.0f || bobAmplitude <= 0.0f)
        {
            transform.localPosition = baseLocalPos;
            return;
        }

        bobPhase += deltaTime * bobFrequency * speedScale;

        float y = Mathf.Sin(bobPhase) * bobAmplitude;
        float x = Mathf.Cos(bobPhase * 0.5f) * bobSideAmplityde;

        Vector3 target = new Vector3(baseLocalPos.x + x, baseLocalPos.y + y, transform.localPosition.z);
        transform.localPosition = Vector3.Lerp(transform.localPosition, target, 0.15f);
    }
}
