using UnityEngine;

public class HurtState : State
{
    public AnimationClip hurtAnim;

    public override void Enter()
    {
        isComplete = false;
        exitReason = StateExitReason.None;

        if (hurtAnim != null && animator != null)
        {
            animator.Play(hurtAnim.name);
        }
    }

    public override State GetNextState()
    {
        if (!isComplete) return null;

        if (core.hpSlider != null && core.hpSlider.value <= 0)
            return core.dieState;

        return core.chaseState;
    }

    public override void Do()
    {
        if (!animator || !hurtAnim) return;

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {
            isComplete = true;
            exitReason = StateExitReason.NormalComplete;
        }
    }
}
