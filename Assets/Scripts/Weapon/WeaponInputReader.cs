using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponInputReader : MonoBehaviour
{
    public bool IsTriggerPressed { get; private set; }
    public bool WasReloadPressedThisFrame { get; private set; }

    public bool IsAimPressed { get; private set; }      // ¡∂¡ÿ(»¶µÂ)

    [SerializeField]
    private bool useManualTick = true;

    // Update is called once per frame
    void Update()
    {
        if(useManualTick == false)
        {
            Tick();
        }
    }

    public void ManualTick(float deltaTime)
    {
        if(useManualTick == true)
        {
            Tick();
        }
    }

    void Tick()
    {
        if(Mouse.current != null)
        {
            IsTriggerPressed = Mouse.current.leftButton.isPressed == true;
            WasReloadPressedThisFrame = Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame == true;
            IsAimPressed = Mouse.current.rightButton.isPressed == true;
        }
        else
        {
            IsTriggerPressed = Input.GetMouseButton(0) == true;
            WasReloadPressedThisFrame = Input.GetKeyDown(KeyCode.R) == true;
            IsAimPressed = Input.GetMouseButton(1) == true;
        }
    }
}
