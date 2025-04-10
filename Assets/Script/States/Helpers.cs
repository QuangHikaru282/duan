using UnityEngine;

public static class Helpers
{
    public static float Map(
        float value, float originalMin, float originalMax,
        float newMin, float newMax, bool clamp
    )
    {
        float newValue = (value - originalMin) / (originalMax - originalMin)
                         * (newMax - newMin) + newMin;
        if (clamp)
        {
            newValue = Mathf.Clamp(newValue, newMin, newMax);
        }
        return newValue;
    }

/*    public static bool CheckLineOfSight2D(
        Transform from,
        Transform target,
        LOSSettings settings,
        bool debugRay = false
    )
    {
        if (!from || !target || settings == null) return false;

        Vector2 origin = from.position;
        Vector2 dest = target.position;
        Vector2 dir = dest - origin;
        float dist = dir.magnitude;

        if (dist > settings.detectRange) return false;

        if (settings.visionAngle <= 360f)
        {
            Vector2 facing = (from.localScale.x >= 0) ? Vector2.right : Vector2.left;
            float angle = Vector2.Angle(facing, dir);
            if (angle > settings.visionAngle / 2f) return false;
        }

        RaycastHit2D hit = Physics2D.Raycast(origin, dir.normalized, dist, settings.obstacleMask);
        if (debugRay)
        {
            Color c = (hit.collider != null) ? Color.yellow : Color.green;
            Debug.DrawRay(origin, dir.normalized * dist, c, 0.2f);
        }

        return hit.collider == null;
    }*/
}
