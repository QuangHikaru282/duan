using UnityEngine;
using System.Collections;

public class AttackState : State
{

    // Clip ani cho trạng thái tấn công
    public AnimationClip attackAnim;
    // Ví dụ: phạm vi tấn công (có thể dùng cho logic hit)
    public float attackRange = 1f;

    public override void Enter()
    {
        // Phát ani tấn công khi state bắt đầu
        animator.Play(attackAnim.name);
        // Bạn có thể kích hoạt sự kiện gây damage qua Animation Event nếu cần
    }

    public override void Do()
    {
        // Giả sử rằng sau khi ani kết thúc thì state hoàn thành
        if (time >= attackAnim.length)
        {
            isComplete = true;
        }
    }

    public override void Exit()
    {
        // Reset các biến liên quan nếu cần
    }
}
