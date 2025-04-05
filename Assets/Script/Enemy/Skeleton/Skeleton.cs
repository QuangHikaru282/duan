using UnityEngine;

public class Skeleton : EnemyCore, IEnemy
{
    //public ShieldState shieldState; // Đây là state đặc biệt của Skeleton

    public void TakeDamage(int damage, string damageType, int attackDirection)
    {
        Debug.Log("Skeleton nhận damage: " + damage);
        machine.Set(hurtState, true);
    }
}
