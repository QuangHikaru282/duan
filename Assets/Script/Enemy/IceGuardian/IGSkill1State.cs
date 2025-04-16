using UnityEngine;

public class IGSkill1State : State
{
    [Header("Skill Settings")]
    public AnimationClip skillAnim;
    public Transform skill1Point;
    [SerializeField] private float triggerRange = 2f;
    public int damage = 3;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float offsetX = 0f;
    [SerializeField] private float offsetY = 0f;
    [SerializeField] private float animDuration = 1.1f;
    [SerializeField] public float cooldown = 6f;
    private float lastUseTime = -Mathf.Infinity;

    public override int priority => 61;

    public bool IsCooldownReady() => Time.time >= lastUseTime + cooldown;

    public bool IsPlayerInTriggerRange()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return false;

        float dist = Vector2.Distance(core.transform.position, playerObj.transform.position);
        return dist <= triggerRange;
    }

    public override void Enter()
    {
        isComplete = false;
        exitReason = StateExitReason.None;

        if (animator && skillAnim)
            animator.Play(skillAnim.name);
    }

    public override void Do()
    {
        if (time >= animDuration)
        {
            lastUseTime = Time.time;
            isComplete = true;
            exitReason = StateExitReason.NormalComplete;
        }
    }

    public void Skill1DealDamage()
    {
        if (skill1Point == null) return;

        Vector2 attackPos = (Vector2)skill1Point.position + new Vector2(offsetX, offsetY);

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPos, triggerRange, playerLayer);
        foreach (Collider2D hit in hits)
        {
            var ps = hit.GetComponent<playerScript>();
            if (ps != null)
            {
                ps.TakeDamage(damage);
                HitStopManager.Instance.TriggerHitStop(0.25f);
            }
        }
    }

    public override State GetNextState()
    {
        if (!isComplete || exitReason != StateExitReason.NormalComplete) return null;
        return los.isSeeingTarget ? core.idleState : core.patrolState;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (skill1Point == null) return;

        Vector2 attackPos = (Vector2)skill1Point.position + new Vector2(offsetX, offsetY);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(attackPos, triggerRange);
    }
#endif
}
