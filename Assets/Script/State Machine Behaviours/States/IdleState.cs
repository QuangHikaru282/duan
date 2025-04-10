using UnityEngine;

public class IdleState : State
{
    [Header("Idle Settings")]
    [SerializeField] private float idleDuration = 2f;
    [SerializeField] private AnimationClip idleAnim;

    private float timer = 0f;

    public override void Enter()
    {
        isComplete = false;
        exitReason = StateExitReason.None;
        timer = 0f;

        if (idleAnim && animator)
        {
            animator.Play(idleAnim.name);
        }
    }

    public override void Do()
    {
        timer += Time.deltaTime;

        if (timer >= idleDuration)
        {
            isComplete = true;
            exitReason = StateExitReason.NormalComplete;
        }
    }

    public override State GetNextState()
    {
        if (!isComplete) return null;
        return core.patrolState;
    }
}
