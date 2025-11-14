using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    public bool useNewInputSystem = true;
    public Vector2 MoveInput { get; private set; }
    public bool LShftPressed { get; private set; }

    // Update is called once per frame
    void Update()
    {
        float x = 0.0f;
        float y = 0.0f;
        LShftPressed = false;

        if (useNewInputSystem == true)
        {
            if (Keyboard.current.aKey.isPressed == true)
            {
                x -= 1.0f;
            }

            if (Keyboard.current.dKey.isPressed == true)
            {
                x += 1.0f;
            }

            if (Keyboard.current.sKey.isPressed == true)
            {
                y -= 1.0f;
            }

            if (Keyboard.current.wKey.isPressed == true)
            {
                y += 1.0f;
            }

            if(Keyboard.current.leftShiftKey.isPressed == true)
            {
                LShftPressed = true;
            }
        }
        else
        {
            x = Input.GetAxisRaw("Horizontal");
            y = Input.GetAxisRaw("Vertical");

            if(Input.GetKey(KeyCode.LeftShift) == true)
            {
                LShftPressed = true;
            }
        }

        Vector2 raw = new Vector2(x, y);
        MoveInput = raw.normalized;
    }
}
