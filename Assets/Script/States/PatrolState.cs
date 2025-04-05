using UnityEngine;

public class PatrolState : State
{
    public NavigateState navigate;
    public IdleState idle;
    public Transform anchor1;
    public Transform anchor2;
    public ChaseState chase;

    public Vector2 checkAreaSize = new Vector2(1f, 0.2f);
    public Vector2 checkAreaOffset = new Vector2(0.5f, -0.5f);
    public Vector2 wallCheckOffset = new Vector2(0.5f, 0f);
    public float wallCheckDistance = 1f;

    private bool anchorsDetached = false;

    public override void SetCore(EnemyCore _core, bool createSubMachine = false)
    {
        base.SetCore(_core, true);
    }

    public override void Enter()
    {
        base.Enter();
        isComplete = false;
        exitReason = StateExitReason.None;

        if (!anchorsDetached)
            DetachAnchors();

        GoToNextDestination();
    }

    public override void Do()
    {
        base.Do();

        // Đã thấy player → chuyển sang chase
        if (los.isSeeingTarget)
        {
            isComplete = true;
            exitReason = StateExitReason.SawPlayer;
            return;
        }

        CheckForFlip();

        if (machine.state != null && machine.state.isComplete)
        {
            StateExitReason childReason = machine.state.exitReason;

            if (machine.state == navigate)
            {
                if (childReason == StateExitReason.NormalComplete)
                {
                    if (idle != null)
                    {
                        machine.Set(idle, true);
                        body.velocity = new Vector2(0f, body.velocity.y);
                    }
                    else
                    {
                        GoToNextDestination();
                    }
                }
            }
            else if (machine.state == idle)
            {
                if (childReason == StateExitReason.NormalComplete)
                {
                    GoToNextDestination();
                }
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        body.velocity = Vector2.zero;
    }

    public override State GetNextState()
    {
        if (!isComplete) return null;

        if (exitReason == StateExitReason.SawPlayer)
            return core.chaseState;

        return null;
    }

    void DetachAnchors()
    {
        if (anchor1 && anchor1.parent == core.transform)
            anchor1.parent = null;
        if (anchor2 && anchor2.parent == core.transform)
            anchor2.parent = null;

        anchorsDetached = true;
    }

    void GoToNextDestination()
    {
        if (!navigate) return;

        Vector2 a1 = anchor1 ? (Vector2)anchor1.position : (Vector2)core.transform.position;
        Vector2 a2 = anchor2 ? (Vector2)anchor2.position : (Vector2)core.transform.position;
        Vector2 curr = navigate.destination;

        navigate.destination = (curr == a1) ? a2 : a1;
        machine.Set(navigate, true);
    }

    void CheckForFlip()
    {
        LayerMask groundMask = core.groundSensor.groundMask | core.groundSensor.platformMask;

        Vector2 offset = new Vector2(checkAreaOffset.x * Mathf.Sign(core.transform.localScale.x), checkAreaOffset.y);
        Vector2 areaCenter = (Vector2)core.transform.position + offset;
        Collider2D groundHit = Physics2D.OverlapBox(areaCenter, checkAreaSize, 0f, groundMask);
        bool noGround = (groundHit == null);

        Vector2 wallRayStart = (Vector2)core.transform.position
            + new Vector2(wallCheckOffset.x * Mathf.Sign(core.transform.localScale.x), wallCheckOffset.y);
        RaycastHit2D wallHit = Physics2D.Raycast(wallRayStart, Vector2.right * Mathf.Sign(core.transform.localScale.x), wallCheckDistance, groundMask);
        bool hitWall = (wallHit.collider != null);

        if (noGround || hitWall)
        {
            Vector3 scale = core.transform.localScale;
            scale.x *= -1;
            core.transform.localScale = scale;

            if (machine.state == navigate)
            {
                navigate.isComplete = true;
                navigate.exitReason = StateExitReason.NormalComplete;
            }
        }
    }
}
