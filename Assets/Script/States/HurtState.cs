using UnityEngine;

public class HurtState : State
{
    public AnimationClip hurtAnim;

    public override void Enter()
    {
        base.Enter();
        isComplete = false;
        exitReason = StateExitReason.None;

        if (hurtAnim != null && animator != null)
        {
            animator.SetTrigger("HurtTrigger");
            animator.Play(hurtAnim.name, 0, 0f);
        }
    }

    public override State GetNextState()
    {
        if (!isComplete) return null;

        if (core.hpSlider != null && core.hpSlider.value <= 0)
            return core.dieState;

        return core.idleState;
    }

    public override void Do()
    {
        base.Do();
        if (!animator || !hurtAnim) return;

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            isComplete = true;
            exitReason = StateExitReason.NormalComplete;
        }
    }
}
