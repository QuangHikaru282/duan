using UnityEngine;

public class SearchState : State
{
    public AnimationClip idleAnim;
    public float searchDuration = 3f;
    private float searchTimer = 0f;

    public float flipInterval = 1f;
    private float flipTimer = 0f;

    public override void Enter()
    {
        base.Enter();
        isComplete = false;
        exitReason = StateExitReason.None;
        searchTimer = 0f;
        flipTimer = 0f;

        if (idleAnim)
        {
            animator.Play(idleAnim.name);
        }

        body.velocity = new Vector2(0f, body.velocity.y);
    }

    public override void Do()
    {
        base.Do();

        // Nếu thấy lại player → kết thúc state
        if (los.isSeeingTarget)
        {
            isComplete = true;
            exitReason = StateExitReason.SawPlayer;
            return;
        }

        // Nếu hết thời gian tìm kiếm → thoát state
        searchTimer += Time.deltaTime;
        if (searchTimer >= searchDuration)
        {
            isComplete = true;
            exitReason = StateExitReason.NormalComplete;
            return;
        }

        // Đảo hướng mỗi khoảng thời gian nhất định
        flipTimer += Time.deltaTime;
        if (flipTimer >= flipInterval)
        {
            flipTimer = 0f;
            Vector3 scale = core.transform.localScale;
            scale.x *= -1;
            core.transform.localScale = scale;
        }
    }

    public override void Exit()
    {
        base.Exit();
        body.velocity = new Vector2(0f, body.velocity.y);
    }

    public override State GetNextState()
    {
        if (!isComplete) return null;

        if (exitReason == StateExitReason.SawPlayer)
            return core.chaseState;

        return core.patrolState;
    }

}
