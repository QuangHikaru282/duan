using UnityEngine;

public class DieState : State
{
    [Header("Die Settings")]
    [Tooltip("Thời gian trước khi Destroy (nếu muốn). Nếu < 0, quái ở lại.")]
    public float vanishTime = 1f;
    private float timer = 0f;

    public override void Enter()
    {
        base.Enter();
        isComplete = false;
        timer = 0f;

        if (animator)
        {
            animator.SetBool("isDead", true);
        }

        if (body)
        {
            body.velocity = Vector2.zero;
        }
    }

    public override void Do()
    {
        base.Do();
        if (vanishTime > 0f)
        {
            timer += Time.deltaTime;
            if (timer >= vanishTime)
            {
                GameObject.Destroy(core.gameObject);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
