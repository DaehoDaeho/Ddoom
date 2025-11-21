using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponInputReader : MonoBehaviour
{
    public bool IsTriggerPressed { get; private set; }
    public bool WasReloadPressedThisFrame { get; private set; }

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
        }
        else
        {
            IsTriggerPressed = Input.GetMouseButton(0) == true;
            WasReloadPressedThisFrame = Input.GetKeyDown(KeyCode.R) == true;
        }
    }
}
