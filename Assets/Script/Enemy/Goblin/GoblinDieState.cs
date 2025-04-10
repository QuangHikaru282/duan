using UnityEngine;

public class GoblinDieState : DieState
{
    protected override void HandleDeathEffect()
    {
        if (core.body == null) return;

        float facingDirection = Mathf.Sign(core.transform.localScale.x);
        Vector2 knockback = new Vector2(-facingDirection * 2f, core.body.velocity.y);
        core.body.velocity = knockback;
    }
}
