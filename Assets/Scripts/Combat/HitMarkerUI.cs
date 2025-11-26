using UnityEngine;
using UnityEngine.UI;

public class HitMarkerUI : MonoBehaviour
{
    [SerializeField]
    private Image markerImage;

    [SerializeField]
    private GameObject playerOwner;

    [SerializeField]
    private float showTime = 0.08f;

    private float timer;

    private void OnEnable()
    {
        if(DamageEventBus.Instance != null)
        {
            DamageEventBus.Instance.OnAnyDamageDealt += HandleAnyDamage;
        }
    }

    private void OnDisable()
    {
        if (DamageEventBus.Instance != null)
        {
            DamageEventBus.Instance.OnAnyDamageDealt -= HandleAnyDamage;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(markerImage == null)
        {
            return;
        }

        if(timer > 0.0f)
        {
            if(markerImage.enabled == false)
            {
                markerImage.enabled = true;
            }

            timer -= Time.deltaTime;

            if(timer <= 0.0f)
            {
                markerImage.enabled = false;
            }
        }
        else
        {
            if(markerImage.enabled == true)
            {
                markerImage.enabled = false;
            }
        }
    }

    void HandleAnyDamage(GameObject source, DamageInfo info, bool killed)
    {
        if(source == playerOwner)
        {
            timer = showTime;
        }
    }
}
