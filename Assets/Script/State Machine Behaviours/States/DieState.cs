using UnityEngine;
using System.Collections;

public class DieState : State
{
    [Header("Animation")]
    public AnimationClip dieAnim;

    [Header("Destroy Settings")]
    public float destroyDelay = 1.5f;

    public override int priority => 999; // Luôn là priority cao nhất

    public override void Enter()
    {
        base.Enter();
        isComplete = false;
        exitReason = StateExitReason.None;

        if (dieAnim != null && animator != null)
        {
            animator.Play(dieAnim.name, 0, 0f);
        }

        // Ngăn quái tiếp tục di chuyển
        if (core.body != null)
            core.body.velocity = Vector2.zero;
    }

    public override void Do()
    {
        base.Do();

        HandleDeathEffect();
        core.StartCoroutine(DestroyAfterDelay(destroyDelay));
        // Gọi drop item nếu có
    }

    protected virtual void HandleDeathEffect()
    {

    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(core.gameObject);

        ItemDropper dropper = core.GetComponent<ItemDropper>();
        if (dropper != null)
            dropper.DropItems();
    }

    public override State GetNextState() => null;
}
