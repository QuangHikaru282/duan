using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    public AttackState attackState;
    public HurtState hurtState;

    public void DealDamage()
    {
        if (attackState != null)
            attackState.DealDamage();
    }

    public void EndAttack()
    {
        if (attackState != null)
            attackState.EndAttack();
    }

    public void GetNextState()
    {
        if (hurtState != null)
            hurtState.GetNextState();
    }
}
