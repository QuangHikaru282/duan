using UnityEngine;

public class IdleState : State
{
    [Header("Idle Settings")]
    public AnimationClip idleAnim;
    [SerializeField] private float idleDuration = 2f;
    [SerializeField] public float idleOverride = -1f;
    [SerializeField] public bool isCooldownIdle = false;

    private float timer = 0f;

    public override void Enter()
    {
        base.Enter();
        isComplete = false;
        exitReason = StateExitReason.None;
        timer = 0f;

        if (idleAnim)
        {
            animator.Play(idleAnim.name);
        }

        if (idleOverride > 0f)
        {
            idleDuration = idleOverride;
            idleOverride = -1f;
        }
    }

    public override void Do()
    {
        base.Do();
        timer += Time.deltaTime;

        if (timer >= idleDuration || !core.groundSensor.grounded)
        {
            isComplete = true;
            exitReason = StateExitReason.NormalComplete;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override State GetNextState()
    {
        if (!isComplete) return null;

        if (isCooldownIdle)
        {
            isCooldownIdle = false;
            return los.isSeeingTarget ? core.chaseState : core.patrolState;
        }

        return core.patrolState;
    }

}
