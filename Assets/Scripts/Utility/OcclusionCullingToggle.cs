using UnityEngine;

public class OcclusionCullingToggle : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private KeyCode toggleKey = KeyCode.O;

    private void Reset()
    {
        if(targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(toggleKey) == true)
        {
            targetCamera.useOcclusionCulling = !targetCamera.useOcclusionCulling;
        }
    }
}
