using UnityEngine;

public class IdleState : State
{
    // Animation clip cho trạng thái Idle
    public AnimationClip idleAnim;

    // Thời gian quái sẽ idle trước khi tự complete
    [SerializeField] private float idleDuration = 1f;
    private float timer = 0f;

    public override void Enter()
    {
        base.Enter();
        if (idleAnim)
        {
            animator.Play(idleAnim.name);
        }
        timer = 0f;
        isComplete = false;
    }

    public override void Do()
    {
        base.Do();

        timer += Time.deltaTime;

        // Nếu quái không còn grounded hoặc đã idle đủ thời gian, coi như xong
        if (!core.groundSensor.grounded || timer >= idleDuration)
        {
            isComplete = true;
        }
    }

    public override void Exit()
    {
        base.Exit();
        // Dọn dẹp hoặc reset biến nếu cần
    }
}
