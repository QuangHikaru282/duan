using UnityEngine;

public class HurtState : State
{
    [Header("Hurt Settings")]
    [SerializeField] private float hurtDuration = 0.5f;
    private float timer = 0f;

    public override void Enter()
    {
        base.Enter();
        isComplete = false;
        timer = 0f;

        // Gọi animation Hurt
        if (animator)
        {
            animator.SetTrigger("Hurt");
        }

        // Có thể knockback quái
        // body.velocity = new Vector2(-core.transform.localScale.x * 2f, body.velocity.y);
    }

    public override void Do()
    {
        base.Do();

        timer += Time.deltaTime;
        if (timer >= hurtDuration)
        {
            // Quái hết trạng thái hurt => isComplete
            isComplete = true;
        }
    }

    public override void Exit()
    {
        base.Exit();
        // Reset animation param, velocity...
    }
}
