using UnityEngine;
using System.Collections;
using Cinemachine;

public class IGIntroState : State
{
    [Header("Intro Settings")]
    public AnimationClip introClip;
    public CinemachineVirtualCamera bossCam;

    [SerializeField] private float zoomDelay = 0.5f;
    [SerializeField] private float shakeDelay = 1.0f;
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeAmplitude = 2f;
    [SerializeField] private float shakeFrequency = 2f;
    private BossMusicManager musicManager;

    private Coroutine sequenceRoutine;
    private CinemachineBasicMultiChannelPerlin noise;

    public override void Enter()
    {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
            audioManager.musicAudioSource.Stop();
        musicManager = FindObjectOfType<BossMusicManager>();
        musicManager?.PlayIntroTheme();

        isComplete = false;
        exitReason = StateExitReason.None;
        core.canBeDamaged = false;
        body.velocity = Vector2.zero;

        if (animator && introClip != null)
        {
            animator.enabled = true;
            animator.Play(introClip.name, 0, 0f);
            animator.speed = 0f;
        }

        if (bossCam != null)
        {
            bossCam.Priority = 20;
            noise = bossCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        sequenceRoutine = core.StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
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

        if (animator != null)
        {
            animator.speed = 1f;
        }

        yield return new WaitForSeconds(introClip.length);

        if (bossCam != null)
            bossCam.Priority = 5;

        isComplete = true;
        exitReason = StateExitReason.NormalComplete;
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
