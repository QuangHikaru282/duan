using UnityEngine;

public class AttackState : State
{
    public AnimationClip atkAnim;
    public Transform atkPoint;
    public Vector2 attackOffset = new Vector2(0.5f, 0f);
    public float attackRange = 1f;
    public LayerMask playerLayer;
    public int damage = 1;
    public float lastAttackTime = -Mathf.Infinity;

    [Header("Cooldown")]
    public float attackCooldown = 1.5f;

    public override int priority => 60;
    public override bool forceInterrupt => false;

    private Vector2 finalAttackPosition;

    public override void Enter()
    {
        base.Enter();
        isComplete = false;
        exitReason = StateExitReason.None;

        body.velocity = Vector2.zero;

        if (atkPoint)
        {
            float facing = Mathf.Sign(core.transform.localScale.x);
            Vector2 offset = new Vector2(attackOffset.x * facing, attackOffset.y);
            finalAttackPosition = (Vector2)atkPoint.position + offset;
        }

        if (animator && atkAnim)
        {
            animator.Play(atkAnim.name);
        }
    }

    public override void Do()
    {
        base.Do();
        body.velocity = new Vector2(0f, body.velocity.y);
    }

    public override void FixedDo()
    {
        base.FixedDo();
        body.velocity = new Vector2(0f, body.velocity.y);
    }

    public override void Exit()
    {
        base.Exit();
        body.velocity = new Vector2(0f, body.velocity.y);
    }

    public override State GetNextState()
    {
        if (!isComplete) return null;

            core.idleState.idleOverride = attackCooldown;
            core.idleState.isCooldownIdle = true;
            return core.idleState;
    }


    // Gọi từ animation event
    public void DealDamage()
    {
        if (!atkPoint) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(finalAttackPosition, attackRange, playerLayer);
        foreach (Collider2D hit in hits)
        {
            var player = hit.GetComponent<playerScript>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }

    public void EndAttack()
    {
        lastAttackTime = Time.time;
        isComplete = true;
        exitReason = StateExitReason.NormalComplete;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (atkPoint)
        {
            float facing = Application.isPlaying
                ? Mathf.Sign(core.transform.localScale.x)
                : Mathf.Sign(transform.root.localScale.x); // Fallback trong editor

            Vector2 offset = new Vector2(attackOffset.x * facing, attackOffset.y);
            Vector2 drawPos = (Vector2)atkPoint.position + offset;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(drawPos, attackRange);
        }
    }
#endif
}
