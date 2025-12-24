using UnityEngine;

public class DayNightSightTuner : MonoBehaviour
{
    [SerializeField] private DayNightController dayNightcontroller;
    [SerializeField] private EnemySight[] enemySights;

    [SerializeField] private float dayMultiplier = 1.0f;
    [SerializeField] private float nightMultiplier = 0.6f;
    [SerializeField] private float applyThreshold = 0.02f;

    private float lastAppliedMultiplier;

    private void Awake()
    {
        lastAppliedMultiplier = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(dayNightcontroller == null)
        {
            return;
        }

        float dayFactor = dayNightcontroller.GetDaysFactor01();
        float multiplier = Mathf.Lerp(nightMultiplier, dayMultiplier, dayFactor);

        float diff = Mathf.Abs(multiplier - lastAppliedMultiplier);
        if(diff < applyThreshold)
        {
            return;
        }

        lastAppliedMultiplier = multiplier;
        ApplyToAll(multiplier);
    }

    void ApplyToAll(float multiplier)
    {
        if(enemySights == null)
        {
            return;
        }

        for(int i=0; i<enemySights.Length; ++i)
        {
            EnemySight sight = enemySights[i];
            if(sight != null)
            {
                sight.ApplySightMultiplier(multiplier);
            }
        }
    }

    public float GetCurrentMultiplier()
    {
        return lastAppliedMultiplier;
    }
}
