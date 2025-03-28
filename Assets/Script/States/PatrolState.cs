using UnityEngine;

public class PatrolState : State
{
    public NavigateState navigate;
    public IdleState idle;
    public Transform anchor1;
    public Transform anchor2;

    public float losCheckInterval = 0.5f;
    private float losCheckTimer = 0f;
    public float detectRange = 5f;
    public float visionAngle = 30f;
    public LayerMask obstacleMask;

    public ChaseState chase;

    public Vector2 checkAreaSize = new Vector2(1f, 0.2f);
    public Vector2 checkAreaOffset = new Vector2(0.5f, -0.5f);
    public Vector2 wallCheckOffset = new Vector2(0.5f, 0f);
    public float wallCheckDistance = 1f;
    private bool anchorsDetached = false;

    void DetachAnchors()
    {
        if (anchor1 != null && anchor1.parent == core.transform)
            anchor1.parent = null;
        if (anchor2 != null && anchor2.parent == core.transform)
            anchor2.parent = null;
        anchorsDetached = true;
    }

    public override void SetCore(EnemyCore _core, bool createSubMachine = false)
    {
        // Vì Patrol cần sub-machine để quản lý Navigate/Idle, đặt createSubMachine = true
        base.SetCore(_core, true);
    }

    public override void Enter()
    {
        base.Enter();
        isComplete = false;
        losCheckTimer = 0f;
        if (!anchorsDetached) DetachAnchors();
        GoToNextDestination();
    }

    public override void Do()
    {
        base.Do();

        if (Time.time >= losCheckTimer)
        {
            losCheckTimer = Time.time + losCheckInterval;
            bool canSee = Helpers.CheckLineOfSight2D(
                from: core.transform,
                target: core.player,
                maxRange: detectRange,
                obstacleMask: obstacleMask,
                maxAngle: visionAngle
            );
            if (canSee)
            {
                Set(chase);
                return;
            }
        }

        CheckForFlip();

        // Nếu sub-state xong => chuyển
        if (machine.state != null && machine.state.isComplete)
        {
            if (machine.state == navigate)
            {
                // Navigate xong => sang Idle (nếu có)
                if (idle != null)
                {
                    machine.Set(idle, true);
                    body.velocity = new Vector2(0f, body.velocity.y);
                    return;
                }
                else
                {
                    // Hoặc quay lại navigate anchor khác
                    GoToNextDestination();
                    return;
                }
            }
            else if (machine.state == idle)
            {
                // Idle xong => đi anchor tiếp
                GoToNextDestination();
                return;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        body.velocity = Vector2.zero;
    }

    void GoToNextDestination()
    {
        if (!navigate) return;
        Vector2 a1 = anchor1 ? (Vector2)anchor1.position : core.transform.position;
        Vector2 a2 = anchor2 ? (Vector2)anchor2.position : core.transform.position;
        Vector2 curr = navigate.destination;
        navigate.destination = (curr == a1) ? a2 : a1;
        machine.Set(navigate, true);
    }

    void CheckForFlip()
    {
        LayerMask combinedMask = core.groundSensor.groundMask | core.groundSensor.platformMask;

        Vector2 adjOffset = new Vector2(
            checkAreaOffset.x * Mathf.Sign(core.transform.localScale.x),
            checkAreaOffset.y
        );
        Vector2 areaCenter = (Vector2)core.transform.position + adjOffset;

        Collider2D groundHit = Physics2D.OverlapBox(areaCenter, checkAreaSize, 0f, combinedMask);
        bool noGround = (groundHit == null);

        Vector2 wallRayStart = (Vector2)core.transform.position
            + new Vector2(wallCheckOffset.x * Mathf.Sign(core.transform.localScale.x),
                          wallCheckOffset.y);
        Vector2 wallRayDir = Vector2.right * Mathf.Sign(core.transform.localScale.x);

        RaycastHit2D wallHit = Physics2D.Raycast(wallRayStart, wallRayDir, wallCheckDistance, combinedMask);
        bool hitWall = (wallHit.collider != null);

        if (noGround || hitWall)
        {
            Vector3 scale = core.transform.localScale;
            scale.x *= -1;
            core.transform.localScale = scale;

            if (machine.state == navigate)
            {
                navigate.isComplete = true;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (!core) return;

        Vector2 adjOffset = new Vector2(
            checkAreaOffset.x * (core.transform != null ? Mathf.Sign(core.transform.localScale.x) : 1f),
            checkAreaOffset.y
        );
        Vector2 areaCenter = (Vector2)core.transform.position + adjOffset;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(areaCenter, checkAreaSize);

        Gizmos.color = Color.yellow;
        Vector2 wallRayStart = (Vector2)core.transform.position
            + new Vector2(wallCheckOffset.x * Mathf.Sign(core.transform.localScale.x),
                          wallCheckOffset.y);
        Vector2 wallRayDir = Vector2.right * Mathf.Sign(core.transform.localScale.x);
        Vector2 wallRayEnd = wallRayStart + wallRayDir * wallCheckDistance;
        Gizmos.DrawLine(wallRayStart, wallRayEnd);
    }
}
