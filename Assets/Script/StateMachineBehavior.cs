using UnityEngine;

public class AttackStateBehaviour : StateMachineBehaviour
{
    // Tên của state hiện tại
    public string stateName;

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerScript player = animator.GetComponent<playerScript>();
        if (player != null)
        {
            // Kiểm tra tên của state để xác định biến cần đặt lại
            if (stateName == "MeleeAttack1" || stateName == "MeleeAttack2")
            {
                player.isAttacking = false;

            }
            else if (stateName == "AirAttack")
            {
                player.isAirAttacking = false;

            }
            else if (stateName == "BowAttack")
            {
                player.isBowAttacking = false;

            }
        }
    }
}
