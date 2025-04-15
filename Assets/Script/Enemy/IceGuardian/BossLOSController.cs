using UnityEngine;

public class BossLOSController : LOSController
{
    [Header("Multi Raycast Settings")]
    [Tooltip("Offset điểm bắn ray tính từ vị trí gốc. Mỗi offset sẽ bắn 2 tia trái/phải.")]
    public Vector2[] multiRayOffsets;

    protected override void Update()
    {
        Vector2 originBase = (Vector2)cachedTransform.position;
        bool sawThisFrame = false;

        foreach (var offset in multiRayOffsets)
        {
            Vector2 origin = originBase + offset;

            Vector2[] directions = { Vector2.left, Vector2.right };
            foreach (Vector2 dir in directions)
            {
                RaycastHit2D hit = Physics2D.Raycast(origin, dir, detectRange, obstacleMask);

                if (debugRay)
                {
                    Color debugColor = hit.collider ? Color.red : Color.yellow;
                    Debug.DrawRay(origin, dir * detectRange, debugColor);
                }

                if (hit.collider && hit.collider.CompareTag(playerTag))
                {
                    _target = hit.collider.transform;
                    sawThisFrame = true;
                    break;
                }
            }

            if (sawThisFrame) break;
        }

        if (sawThisFrame)
        {
            _isSeeingTarget = true;
            memoryTimer = memoryDuration;
        }
        else
        {
            memoryTimer -= Time.deltaTime;
            if (memoryTimer <= 0f)
            {
                _isSeeingTarget = false;
                _target = null;
            }
        }
    }
}
