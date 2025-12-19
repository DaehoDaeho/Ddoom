using UnityEngine;
using TMPro;

public class SimpleFpsCounter : MonoBehaviour
{
    [SerializeField] private float updateInterval = 0.25f;
    [SerializeField] private TMP_Text fpsText;
    private float elapsed;
    private float fps;

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;

        if(elapsed >= updateInterval)
        {
            float dt = Time.deltaTime;
            if(dt > 0.000001f)
            {
                fps = 1.0f / dt;
            }

            if(fpsText != null)
            {
                fpsText.text = fps.ToString();
            }

            elapsed = 0.0f;
        }
    }
}
