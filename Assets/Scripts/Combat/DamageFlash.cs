using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [SerializeField]
    private Color flashColor = Color.white;

    [SerializeField]
    private float flashIntensity = 2.0f;

    [SerializeField]
    private float flashDuration = 0.08f;

    private Renderer rend;
    private MaterialPropertyBlock mpb;
    private float timer;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();

        Health health = GetComponentInParent<Health>();
        if(health != null)
        {
            health.OnDamaged += HandleDamaged;
        }
    }

    private void OnDestroy()
    {
        Health health = GetComponentInParent<Health>();
        if (health != null)
        {
            health.OnDamaged -= HandleDamaged;
        }
    }

    void HandleDamaged(DamageInfo info, float oldHp, float newHp)
    {
        timer = flashDuration;
    }

    private void LateUpdate()
    {
        //if(timer > 0.0f)
        //{
        //    timer -= Time.deltaTime;
        //    float t = Mathf.Clamp01(timer / flashDuration);

        //    rend.GetPropertyBlock(mpb);
        //    mpb.SetColor("_EmissionColor", flashColor * flashIntensity * t);
        //    rend.SetPropertyBlock(mpb);
        //}
        //else
        //{
        //    rend.GetPropertyBlock(mpb);
        //    mpb.SetColor("_EmissionColor", Color.black);
        //    rend.SetPropertyBlock(mpb);
        //}
    }
}
