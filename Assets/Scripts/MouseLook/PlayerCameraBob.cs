using UnityEngine;

public class PlayerCameraBob : MonoBehaviour
{
    [SerializeField]
    private PlayerMotor playerMotor;

    [SerializeField]
    private bool enableBob = true;

    [SerializeField]
    private float amplitude = 0.03f;

    [SerializeField]
    private float frequency = 9.0f;

    private Vector3 baseLocalPos;
    private float phase;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baseLocalPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(enableBob == false)
        {
            if(transform.localPosition != baseLocalPos)
            {
                //transform.localPosition = Vector3.Lerp(transform.localPosition, baseLocalPos, 0.15f);
                transform.localPosition = baseLocalPos;
            }
            return;
        }

        if(playerMotor == null)
        {
            return;
        }

        phase += Time.deltaTime * frequency;

        float y = Mathf.Sin(phase) * amplitude;
        float x = Mathf.Cos(phase * 0.5f) * amplitude;

        Vector3 target = baseLocalPos + new Vector3(x, y, 0.0f);
        transform.localPosition = Vector3.Lerp(transform.localPosition, target, 0.25f);
    }
}
