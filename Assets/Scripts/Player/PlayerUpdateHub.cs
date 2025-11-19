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
    }
}
