using UnityEngine;
using System.Collections;

public class StaggerState : State
{
    [Header("Stagger Settings")]
    public AnimationClip staggerAnim;
    public Collider2D staggerHitbox;
    public float staggerDuration = 5f;
    public float dropInterval = 1f;
    public int maxDropCount = 5;

    private Coroutine dropRoutine;
    private int dropCount = 0;
    private float timer = 0f;
    private ItemDropper dropper;

    public override int priority => 90;

    public override void Enter()
    {
        isComplete = false;
        exitReason = StateExitReason.None;
        dropCount = 0;
        timer = 0f;

        core.canBeDamaged = false;

        if (animator && staggerAnim)
            animator.Play(staggerAnim.name);

        dropper = core.GetComponent<ItemDropper>();
        if (dropper != null)
            dropRoutine = core.StartCoroutine(AutoDropRoutine());

        if (staggerHitbox != null)
            staggerHitbox.enabled = true;
    }

    IEnumerator AutoDropRoutine()
    {
        while (dropCount < maxDropCount)
        {
            yield return new WaitForSeconds(dropInterval);
            dropper.DropItems();
            dropCount++;
        }
    }

    public void DropOnHit()
    {
        if (dropper != null && dropCount < maxDropCount)
        {
            dropper.DropItems();
            dropCount++;
        }
    }

    public override void Do()
    {
        timer += Time.deltaTime;
        if (timer >= staggerDuration)
        {
            isComplete = true;
            exitReason = StateExitReason.NormalComplete;
        }
    }

    public override void Exit()
    {
        if (dropRoutine != null)
        {
            core.StopCoroutine(dropRoutine);
            dropRoutine = null;
        }

        if (staggerHitbox != null)
            staggerHitbox.enabled = false;

        core.canBeDamaged = true;
    }

    public override State GetNextState()
    {
        return core.chaseState;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Skill") || other.CompareTag("AOE"))
        {
            DropOnHit();
        }
    }
}
