using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMouseLook : MonoBehaviour
{
    [SerializeField]
    private Transform cameraPivot;

    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private float sensitivity = 1.0f;

    [SerializeField]
    private float minPitchDeg = -80.0f;

    [SerializeField]
    private float maxPitchDeg = 80.0f;

    [SerializeField]
    private float turnSpeedDegreeesPerSec = 720.0f;

    [SerializeField]
    private bool useManualTick = true;

    private float yaw;
    private float pitch;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        yaw = euler.y;

        if(cameraPivot != null)
        {
            Vector3 pivotEuler = cameraPivot.localRotation.eulerAngles;
            pitch = Normalize180(pivotEuler.x);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(useManualTick == false)
        {
            Tick(Time.deltaTime);
        }
    }

    public void ManualTick(float deltaTime)
    {
        if(useManualTick == true)
        {
            Tick(deltaTime);
        }
    }

    void Tick(float deltaTime)
    {
        Vector2 mouseDelta = ReadMouseDelta();

        float yawDelta = mouseDelta.x * sensitivity * deltaTime;    // 수평
        float pitchDelta = mouseDelta.y * sensitivity * deltaTime;  // 수직

        pitch -= pitchDelta;
        yaw += yawDelta;

        pitch = Mathf.Clamp(pitch, minPitchDeg, maxPitchDeg);

        Quaternion targetBodyRot = Quaternion.Euler(0.0f, yaw, 0.0f);
        Quaternion targetPivotRot = Quaternion.Euler(pitch, 0.0f, 0.0f);

        if(turnSpeedDegreeesPerSec > 0.0f)
        {
            float maxStep = turnSpeedDegreeesPerSec * deltaTime;

            Quaternion currentBody = transform.rotation;
            transform.rotation = Quaternion.RotateTowards(currentBody, targetBodyRot, maxStep);

            if(cameraPivot != null)
            {
                Quaternion currentPivot = cameraPivot.localRotation;
                cameraPivot.localRotation = Quaternion.RotateTowards(currentPivot, targetPivotRot, maxStep);
            }
        }
        else
        {
            transform.rotation = targetBodyRot;
            if(cameraPivot != null)
            {
                cameraPivot.localRotation = targetPivotRot;
            }
        }
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

    // 0~360 값을 -180~180 범위로 변환
    float Normalize180(float degrees)
    {
        float d = degrees;
        if(d > 180.0f)
        {
            d -= 360.0f;
        }

        return d;
    }
}
