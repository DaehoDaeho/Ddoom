using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField]
    private PlayerInputReader inputReader;

    [SerializeField]
    private bool autoFaceMoveDirection = true;

    [SerializeField]
    private float turnSpeedDegreesPerSec = 540.0f;

    [SerializeField]
    private bool useManualTick = true;

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
        if(autoFaceMoveDirection == false)
        {
            return;
        }

        if(inputReader == null)
        {
            return;
        }

        Vector2 input = inputReader.MoveInput;
        if(input.sqrMagnitude < 0.0001f)
        {
            return;
        }

        float targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;

        Quaternion current = transform.rotation;
        Quaternion target = Quaternion.Euler(0.0f, targetAngle, 0.0f);

        float maxStep = turnSpeedDegreesPerSec * deltaTime;
        transform.rotation = Quaternion.RotateTowards(current, target, maxStep);
    }
}
