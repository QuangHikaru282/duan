using UnityEngine;

public class ChaseState : State
{
    public float chaseSpeed = 3f;

    public SearchState searchState;
    public AttackState attackState;
    public AnimationClip runClip;

    private Vector2 lastTimeSeenPlayerPos;
    private bool requestContinueChase = false;
    private bool playedRun = false;

    public override void SetCore(EnemyCore _core, bool createSubMachine = false)
    {
        base.SetCore(_core, searchState != null);
    }

    public override void Enter()
    {
        base.Enter();
        isComplete = false;
        exitReason = StateExitReason.None;
        requestContinueChase = false;
        playedRun = false;

        if (animator)
            animator.speed = 1.5f;

        if (machine != null)
            machine.state = null;
    }

    public override void Do()
    {
        base.Do();

        if (core.player)
        {
            float distanceToPlayer = Vector2.Distance(core.transform.position, core.player.position);
            if (distanceToPlayer <= attackState.attackRange &&
                Time.time >= attackState.lastAttackTime + attackState.attackCooldown)
            {
                parent.TrySet(attackState);
                return;
            }
        }

        if (requestContinueChase)
        {
            requestContinueChase = false;
            isComplete = false;
            exitReason = StateExitReason.None;
        }

        if (machine != null && machine.state != null)
        {
            if (machine.state.isComplete)
            {
                var childReason = machine.state.exitReason;
                if (childReason == StateExitReason.SawPlayer)
                {
                    machine.Set(this, true);
                }
                else
                {
                    isComplete = true;
                    exitReason = StateExitReason.NormalComplete;
                }
            }
            return;
        }

        // Nếu mất sight → chuyển sang search
        if (!los.isSeeingTarget)
        {
            core.lastKnownPosition = lastTimeSeenPlayerPos;
            if (searchState != null)
            {
                machine.Set(searchState);
            }
            else
            {
                isComplete = true;
                exitReason = StateExitReason.NormalComplete;
            }
            return;
        }

        // Tiếp tục đuổi
        if (core.player)
        {
            Vector2 skelePos = core.transform.position;
            Vector2 playerPos = core.player.position;
            lastTimeSeenPlayerPos = playerPos;

            float dirX = (playerPos.x < skelePos.x) ? -1f : 1f;
            Vector3 scale = core.transform.localScale;
            scale.x = (dirX > 0) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            core.transform.localScale = scale;

            float distance = Mathf.Abs(playerPos.x - skelePos.x);
            float stopDistance = attackState.attackRange - 0.1f;

            if (distance > stopDistance)
            {
                body.velocity = new Vector2(dirX * chaseSpeed, body.velocity.y);
            }
            else
            {
                body.velocity = new Vector2(0f, body.velocity.y);
            }

            if (!playedRun && runClip && animator)
            {
                animator.Play(runClip.name);
                playedRun = true;
            }

            /*var sep = core.GetComponent<EnemySeparationHandler>();
            if (sep != null)
                sep.ApplySeparation();*/

        }
    }


    public override void Exit()
    {
        base.Exit();
        body.velocity = Vector2.zero;
        if (animator)
            animator.speed = 1f;
    }

    public override State GetNextState()
    {
        if (!isComplete) return null;

        return core.patrolState;
    }

}