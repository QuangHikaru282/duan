using UnityEngine;
using System.Collections;
using Cinemachine;

public class IGPhase2State : State
{
    [Header("Phase2 Settings")]
    public AnimationClip phase2Anim;
    public CinemachineVirtualCamera bossCam;

    [SerializeField] private float zoomDelay = 0.3f;
    [SerializeField] private float shakeDelay = 0.8f;
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeAmplitude = 2f;
    [SerializeField] private float shakeFrequency = 2f;
    private BossMusicManager musicManager;
    private Coroutine sequenceRoutine;
    private CinemachineBasicMultiChannelPerlin noise;

    public override int priority => 100; 

    public override void Enter()
    {
        isComplete = false;
        exitReason = StateExitReason.None;
        core.canBeDamaged = false;

        if (animator && phase2Anim != null)
        {
            animator.Play(phase2Anim.name);
        }

        musicManager = FindObjectOfType<BossMusicManager>();
        musicManager?.PlayPhase2Theme();

        if (bossCam != null)
        {
            bossCam.Priority = 20;
            noise = bossCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        ApplyPhase2Buffs();
        sequenceRoutine = core.StartCoroutine(Phase2Sequence());
    }

    private IEnumerator Phase2Sequence()
    {
        yield return new WaitForSeconds(zoomDelay);

        yield return new WaitForSeconds(shakeDelay);
        if (noise != null)
        {
            noise.m_AmplitudeGain = shakeAmplitude;
            noise.m_FrequencyGain = shakeFrequency;
        }

        yield return new WaitForSeconds(shakeDuration);

        if (noise != null)
        {
            noise.m_AmplitudeGain = 0f;
            noise.m_FrequencyGain = 0f;
        }

        yield return new WaitForSeconds(phase2Anim.length - zoomDelay - shakeDelay - shakeDuration);

        if (bossCam != null)
            bossCam.Priority = 5;

        isComplete = true;
        exitReason = StateExitReason.NormalComplete;
    }

    private void ApplyPhase2Buffs()
    {
        if (ig == null) return;

        if (ig.skill1State != null)
        {
            ig.skill1State.damage += 1;
            ig.skill1State.cooldown = Mathf.Max(0f, ig.skill1State.cooldown - 3f);
            //ig.skill1State.PriorityValue = 82;
        }


        if (ig.attackState != null)
        {
            ig.attackState.damage += 1;
        }


        if (ig.chaseState != null)
        {
            var chase = ig.chaseState as ChaseState;
            if (chase != null)
                chase.chaseSpeed += 3f; 
        }
    }

    public override void Exit()
    {
        if (sequenceRoutine != null)
        {
            core.StopCoroutine(sequenceRoutine);
            sequenceRoutine = null;
        }

        if (noise != null)
        {
            noise.m_AmplitudeGain = 0f;
            noise.m_FrequencyGain = 0f;
        }

        if (bossCam != null)
            bossCam.Priority = 5;
        core.canBeDamaged = true;
    }

    public override State GetNextState()
    {
        if (!isComplete) return null;
        return core.chaseState;
    }
}
