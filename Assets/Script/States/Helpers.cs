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

    public static bool CheckLineOfSight2D(
        Transform from, Transform target,
        float maxRange, LayerMask obstacleMask,
        float maxAngle = 0f, bool debugRay = false
    )
    {
        if (from == null || target == null) return false;

        Vector2 origin = from.position;
        Vector2 dest = target.position;
        Vector2 dir = dest - origin;
        float dist = dir.magnitude;

        // 1) Check range
        if (dist > maxRange) return false;

        // 2) Check angle (nếu có yêu cầu)
        if (maxAngle > 0f)
        {
            // Quái giả định "mặt" hướng theo localScale.x
            Vector2 facing = (from.localScale.x >= 0) ? Vector2.right : Vector2.left;
            float angle = Vector2.Angle(facing, dir);
            if (angle > maxAngle) return false;
        }

        // 3) Raycast coi có bị chặn
        RaycastHit2D hit = Physics2D.Raycast(origin, dir.normalized, dist, obstacleMask);
        if (debugRay)
        {
            Color c = (hit) ? Color.yellow : Color.green;
            Debug.DrawRay(origin, dir.normalized * dist, c, 0.2f);
        }

        return !hit; // true nếu không gặp obstacle
    }
}
