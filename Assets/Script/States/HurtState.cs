using UnityEngine;

public class HurtState : State
{
    [SerializeField] private float hurtDuration = 0.5f;
    private float timer = 0f;

    public override void Enter()
    {
        base.Enter();
        isComplete = false;
        exitReason = StateExitReason.None;
        timer = 0f;
        // animator.SetTrigger("Hurt");
    }

    public override void Do()
    {
        base.Do();
        timer += Time.deltaTime;
        if (timer >= hurtDuration)
        {
            isComplete = true;
            exitReason = StateExitReason.NormalComplete;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
