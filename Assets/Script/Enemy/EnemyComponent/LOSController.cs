using UnityEngine;

public class LOSController : MonoBehaviour
{
    [Header("Tầm nhìn")]
    public float detectRange = 5f;
    public LayerMask obstacleMask;
    public string playerTag = "Player";

    [Header("Bộ nhớ")]
    public float memoryDuration = 1.5f;
    protected float memoryTimer = 0f;

    [Header("Ray Offset (tùy chỉnh theo sprite)")]
    public Vector2 raycastOffset = Vector2.zero;

    [Header("Debug")]
    public bool debugRay = false;

    public bool isSeeingTarget => _isSeeingTarget;
    public Transform target => _target;
    protected bool _isSeeingTarget = false;
    protected Transform _target = null;
    protected Transform cachedTransform;

    protected virtual void Awake()
    {
        cachedTransform = transform;
    }

    protected virtual void Update()
    {
        Vector2 origin = (Vector2)cachedTransform.position + raycastOffset;
        Vector2[] directions = { Vector2.left, Vector2.right };
        bool sawThisFrame = false;

        foreach (Vector2 dir in directions)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, detectRange, obstacleMask);
            if (debugRay)
            {
                Debug.DrawRay(origin, dir * detectRange, hit.collider ? Color.red : Color.yellow);
            }

            if (hit.collider && hit.collider.CompareTag(playerTag))
            {
                _target = hit.collider.transform;
                sawThisFrame = true;
                break;
            }
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
