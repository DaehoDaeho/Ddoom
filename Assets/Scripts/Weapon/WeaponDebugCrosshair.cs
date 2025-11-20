using UnityEngine;

public class WeaponDebugCrosshair : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private float rayLength = 3.0f;

    private void LateUpdate()
    {
        if(cameraTransform == null)
        {
            return;
        }

        Vector3 origin = cameraTransform.position;
        Vector3 dir = cameraTransform.forward;
        Debug.DrawRay(origin, dir * rayLength, Color.green, 0.0f, false);
    }
}
