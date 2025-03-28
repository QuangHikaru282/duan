using UnityEngine;

public class ChaseState : State
{

    [Header("Chase Settings")]
    [Tooltip("Tốc độ di chuyển khi chase.")]
    public float chaseSpeed = 3f;

    [Tooltip("Phạm vi tối đa để quái 'thấy' player.")]
    public float detectRange = 5f;

    [Tooltip("Góc nhìn, nếu muốn mô phỏng tầm nhìn (0 = không check góc).")]
    public float visionAngle = 0f;

    [Tooltip("Layer obstacles (tường, ground) chặn tầm nhìn.")]
    public LayerMask obstacleMask;

    [Tooltip("Tần suất kiểm tra line-of-sight (giây).")]
    public float losCheckInterval = 0.5f;
    private float losCheckTimer = 0f;

    [Header("Search State (sub-state)")]
    public SearchState searchState;  // Gắn qua Inspector

    public override void SetCore(EnemyCore _core, bool createSubMachine = false)
    {
        // ChaseState có subMachine
        base.SetCore(_core, true);
    }

    public override void Enter()
    {
        base.Enter();
        isComplete = false;
        losCheckTimer = 0f;
        // priority, forceInterrupt => gán qua Inspector
    }

    public override void Do()
    {
        base.Do();

        // Nếu sub-state (Search) đang chạy => DoBranch() đã gọi -> kiểm tra isComplete
        if (machine.state != null)
        {
            // Nếu sub-state xong => ta cũng đánh dấu xong => quay về main
            if (machine.state.isComplete)
            {
                isComplete = true;
            }
            return;
        }

        // Sub-state chưa set => quái đang chase
        losCheckTimer += Time.deltaTime;
        if (losCheckTimer >= losCheckInterval)
        {
            losCheckTimer = 0f;

            bool canSee = Helpers.CheckLineOfSight2D(
                from: core.transform,
                target: core.player,
                maxRange: detectRange,
                obstacleMask: obstacleMask,
                maxAngle: visionAngle,
                debugRay: true
            );

            if (!canSee)
            {
                // Mất sight => chuyển sang search
                if (searchState != null)
                {
                    machine.Set(searchState);
                }
                return;
            }
        }

        // Di chuyển về phía player
        if (core.player)
        {
            Vector2 skelePos = core.transform.position;
            Vector2 playerPos = core.player.position;
            float dirX = (playerPos.x < skelePos.x) ? -1f : 1f;

            // Lật sprite
            Vector3 scale = core.transform.localScale;
            scale.x = (dirX > 0) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            core.transform.localScale = scale;

            // Di chuyển
            body.velocity = new Vector2(dirX * chaseSpeed, body.velocity.y);

            // Cập nhật lastKnownPosition
            core.lastKnownPosition = playerPos;
        }
    }

    public override void Exit()
    {
        base.Exit();
        // Reset
        body.velocity = Vector2.zero;
    }
}
