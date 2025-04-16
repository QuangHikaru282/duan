using UnityEngine;

public class IGSkill2State : State
{
    [Header("Skill2 Settings")]
    public AnimationClip skill2Anim;
    public Transform skill2Point;
    public GameObject iceBlastPrefab;

    [SerializeField] private float animDuration = 1.2f;
    [SerializeField] private float cooldown = 8f;
    private float lastUseTime = -Mathf.Infinity;

    public override int priority => 82;

    public bool IsCooldownReady() => Time.time >= lastUseTime + cooldown;

    public override void Enter()
    {
        isComplete = false;
        exitReason = StateExitReason.None;

        if (animator && skill2Anim)
            animator.Play(skill2Anim.name);
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

    public void SpawnIceBlast()
    {
        if (skill2Point == null || iceBlastPrefab == null) return;


        GameObject rightBlast = Object.Instantiate(iceBlastPrefab, skill2Point.position, Quaternion.identity);
        rightBlast.transform.localScale = new Vector3(1, 1, 1);

        GameObject leftBlast = Object.Instantiate(iceBlastPrefab, skill2Point.position, Quaternion.identity);
        leftBlast.transform.localScale = new Vector3(-1, 1, 1);
    }

    public override State GetNextState()
    {
        if (!isComplete || exitReason != StateExitReason.NormalComplete) return null;
        return los.isSeeingTarget ? core.idleState : core.patrolState;
    }
}
