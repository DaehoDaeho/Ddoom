using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private PlayerInputReader inputReader;

    [SerializeField]
    private float moveSpeed = 5.0f;

    private float moveSpeedMultiplier = 1.0f;

    // Update is called once per frame
    void Update()
    {
        if(inputReader == null)
        {
            return;
        }

        Vector2 input = inputReader.MoveInput;
        Vector3 moveDir = new Vector3(input.x, 0.0f, input.y);

        if(moveDir.sqrMagnitude < 0.0001f)
        {
            return;
        }

        moveSpeedMultiplier = 1.0f;
        if(inputReader.LShftPressed == true)
        {
            moveSpeedMultiplier = 1.5f;
        }

        Vector3 delta = moveDir * moveSpeed * moveSpeedMultiplier * Time.deltaTime;

        transform.position += delta;
    }

    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
}
