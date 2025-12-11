using UnityEngine;

public class PlayerUpdateHub : MonoBehaviour
{
    public enum UpdateMode
    {
        Update,
        FixedUpdate
    }

    [SerializeField]
    private UpdateMode updateMode = UpdateMode.Update;

    [SerializeField]
    private PlayerInputReader inputReader;

    [SerializeField]
    private PlayerMotor playerMotor;
    
    [SerializeField]
    private PlayerMouseLook mouseLook;

    [SerializeField]
    private WeaponInputReader weaponInputReader;

    [SerializeField]
    private WeaponController weaponController;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(updateMode == UpdateMode.Update)
        {
            Step(Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if(updateMode == UpdateMode.FixedUpdate)
        {
            Step(Time.fixedDeltaTime);
        }
    }

    void Step(float deltaTime)
    {
        if(inputReader != null)
        {
            inputReader.ManualTick(deltaTime);
        }

        if(playerMotor != null)
        {
            playerMotor.ManualTick(deltaTime);
        }

        if(mouseLook != null)
        {
            mouseLook.ManualTick(deltaTime);
        }

        //if (weaponInputReader != null)
        //{
        //    weaponInputReader.ManualTick(deltaTime);
        //}

        //if (weaponController != null)
        //{
        //    weaponController.ManualTick(deltaTime);
        //}
    }
}
