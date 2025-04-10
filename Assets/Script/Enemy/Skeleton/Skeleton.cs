using UnityEngine;

public class Skeleton : EnemyCore
{
    [Header("Private States")]
    public ShieldState shieldState;
    public BoxCollider2D colliderBlock;

    void Awake()
    {
        if (colliderBlock != null)
            colliderBlock.enabled = false;
    }

    public void OnProjectileDetected()
    {
        if (state is ShieldState || state.priority >= shieldState.priority) return;
        machine.TrySet(shieldState, true);
    }
}
