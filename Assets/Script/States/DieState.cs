using UnityEngine;

public class DieState : State
{
    public float vanishTime = 1f;
    private float timer = 0f;

    public override void Enter()
    {
        base.Enter();
        isComplete = false;
        exitReason = StateExitReason.None;
        timer = 0f;
        // animator.SetBool("isDead", true);
        if (body) body.velocity = Vector2.zero;
    }

    public override void Do()
    {
        base.Do();
        if (vanishTime > 0f)
        {
            timer += Time.deltaTime;
            if (timer >= vanishTime)
            {
                GameObject.Destroy(core.gameObject);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
