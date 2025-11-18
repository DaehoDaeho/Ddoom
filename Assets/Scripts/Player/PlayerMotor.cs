using Unity.VisualScripting;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    private PlayerInputReader inputReader;

    [SerializeField]
    private CharacterController controller;

    [SerializeField]
    private float moveSpeed = 5.0f;

    [SerializeField]
    private float jumpHeight = 1.2f;

    [SerializeField]
    private float gravity = -9.81f;

    [SerializeField]
    private float gravityScale = 2.0f;  // 중력 배수. 실제 체감 중력을 조절.

    [SerializeField]
    private float groundedStickyVelocity = -2.0f;   // 지면 밀착을 위한 y 속도 고정값 (지면일 때 살짝 아래로 눌러줌)

    [SerializeField]
    private KeyCode jumpKey = KeyCode.Space;

    [SerializeField]
    private bool useManualTick = true;

    private float verticalVelocity = 0.0f;  // 현재 프레임의 수직 속도.

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
        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }

        if (controller == null)
        {
            return;
        }

        if (inputReader == null)
        {
            return;
        }

        Vector2 input = inputReader.MoveInput;

        Vector3 moveDir = new Vector3(input.x, 0.0f, input.y);
        Vector3 horizontalVelocity = moveDir * moveSpeed;

        bool isGrounded = controller.isGrounded;

        if (isGrounded == true && verticalVelocity < 0.0f)
        {
            verticalVelocity = groundedStickyVelocity;
        }

        if (isGrounded == true && Input.GetKeyDown(jumpKey) == true)
        {
            // 초기 점프 속도 계산.
            float effectiveGravity = gravity * gravityScale;
            float v0 = Mathf.Sqrt(2.0f * -effectiveGravity * jumpHeight);
            verticalVelocity = v0;
        }

        verticalVelocity += (gravity * gravityScale) * Time.deltaTime;

        Vector3 velocity = new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);
        Vector3 delta = velocity * Time.deltaTime;

        controller.Move(delta);
    }

    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }

    public void SetJumpHeight(float newHeight)
    {
        jumpHeight = newHeight;
    }
}
