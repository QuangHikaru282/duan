using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    public AttackState attackState;

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
}
