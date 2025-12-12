using UnityEngine;

public static class PhysicsRaycasterHelper
{
    public static bool TrySightRay(Vector3 pos, Vector3 dir, float maxDistance, out RaycastHit hit)
    {
        hit = new RaycastHit();
        
        ProjectPhysicsConfig cfg = ProjectPhysicsConfig.Get();
        if(cfg == null)
        {
            return false;
        }

        Ray ray = new Ray(pos, dir);
        LayerMask mask = cfg.GetSightMask();

        bool didHit = Physics.Raycast(ray, out hit, maxDistance, mask, QueryTriggerInteraction.Ignore);

        return didHit;
    }

    public static bool TryBulletRay(Camera cam, float maxDistance, out RaycastHit hit)
    {
        hit = new RaycastHit();
        if (cam == null)
        {
            return false;
        }

        ProjectPhysicsConfig cfg = ProjectPhysicsConfig.Get();
        if (cfg == null)
        {
            return false;
        }

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        LayerMask mask = cfg.GetBulletHitMask();

        bool didHit = Physics.Raycast(ray, out hit, maxDistance, mask, QueryTriggerInteraction.Ignore);

        return didHit;
    }

    public static bool TryInteractRay(Camera cam, float maxDistance, out RaycastHit hit)
    {
        hit = new RaycastHit();
        if (cam == null)
        {
            return false;
        }

        ProjectPhysicsConfig cfg = ProjectPhysicsConfig.Get();
        if (cfg == null)
        {
            return false;
        }

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        LayerMask mask = cfg.GetInteractMask();

        bool didHit = Physics.Raycast(ray, out hit, maxDistance, mask, QueryTriggerInteraction.Ignore);

        return didHit;
    }
}
