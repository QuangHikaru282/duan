using UnityEngine;

public class NavigateState : State
{
    public Vector2 destination;
    public float speed = 2f;
    public float threshold = 0.1f;

    public AnimationClip runClip;
    private bool playedRun = false;

    public override void Enter()
    {
        base.Enter();
        isComplete = false;
        exitReason = StateExitReason.None;
        playedRun = false;
    }

    public override void Do()
    {
        base.Do();

        float dist = Vector2.Distance(core.transform.position, destination);
        if (dist < threshold)
        {
            isComplete = true;
            exitReason = StateExitReason.NormalComplete;
            return;
        }

        if (!playedRun && runClip && animator)
        {
            animator.Play(runClip.name);
            playedRun = true;
        }

        float dirX = (destination.x < core.transform.position.x) ? -1f : 1f;
        Vector3 scale = core.transform.localScale;
        scale.x = (dirX > 0) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        core.transform.localScale = scale;
    }

    public override void FixedDo()
    {
        base.FixedDo();
        Vector2 current = core.transform.position;
        Vector2 dir = (destination - current).normalized;
        body.velocity = new Vector2(dir.x * speed, body.velocity.y);
    }

    public override void Exit()
    {
        base.Exit();
        body.velocity = new Vector2(0f, body.velocity.y);
    }
}
