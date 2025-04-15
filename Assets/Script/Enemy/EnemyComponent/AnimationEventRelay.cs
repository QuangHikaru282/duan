using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    public AttackState attackState;
    public HurtState hurtState;
    public IGSkill1State skill1State;
    public IGSkill2State skill2State;

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

    public void Skill1DealDamage()
    {
        if (skill1State != null)
            skill1State.Skill1DealDamage();
    }

    public void SpawnIceBlast()
    {
        if (skill2State != null)
            skill2State.SpawnIceBlast();
    }

    public void GetNextState()
    {
        if (hurtState != null)
            hurtState.GetNextState();
    }
}
