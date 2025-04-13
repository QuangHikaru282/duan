using UnityEngine;

public class ShieldState : State
{
    public float shieldDuration = 1.2f;
    private float timer = 0f;

    [Header("Collider dùng để chặn đòn")]
    public BoxCollider2D colliderBlock;

    private bool hasEnabledShield = false;

    public override void Enter()
    {
        base.Enter();
        isComplete = false;
        exitReason = StateExitReason.None;
        timer = 0f;
        hasEnabledShield = false;
        body.velocity = Vector2.zero;

        if (animator)
        {
            animator.Play("skele_Shield", 0, 1f);
            animator.speed = 0f; // Dừng ở frame đầu
        }
    }

    public override void Do()
    {
        base.Do();
        EnableShield();
        timer += Time.deltaTime;

        if (timer >= shieldDuration)
        {
            if (animator)
                animator.speed = 1f; // Resume để thoát khỏi state

            isComplete = true;
            exitReason = StateExitReason.NormalComplete;
        }
    }

    public override void Exit()
    {
        DisableShield();
        if (animator)
            animator.speed = 1f;
    }

    public override State GetNextState()
    {
        if (!isComplete) return null;
        return core.idleState;
    }

    // Animation event gọi ở frame giơ khiên
    public void EnableShield()
    {
        if (hasEnabledShield) return;

        hasEnabledShield = true;
        if (colliderBlock != null)
            colliderBlock.enabled = true;
    }

    private void DisableShield()
    {
        if (colliderBlock != null)
            colliderBlock.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
       
        if (other.CompareTag("Skill"))
        {
            Destroy(other.gameObject); // Chặn skill bay tới
        }
    }

}
