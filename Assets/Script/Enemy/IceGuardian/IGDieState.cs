using UnityEngine;
using System.Collections;
using Cinemachine;

public class IGDieState : DieState
{
    [Header("Boss Die Settings")]
    public GameObject bossKeyPrefab;
    public CinemachineVirtualCamera bossCam;

    [SerializeField] private float shakeAmplitude = 3.5f;
    [SerializeField] private float shakeFrequency = 3.5f;

    private CinemachineBasicMultiChannelPerlin noise;

    public override void HandleDeathEffect()
    {
        BossMusicManager bossMusic = GameObject.FindObjectOfType<BossMusicManager>();
        if (bossMusic != null)
        {
            bossMusic.StopMusic();
        }

        if (animator != null && dieAnim != null)
        {
            animator.Play(dieAnim.name);
        }

        if (bossCam != null)
        {
            bossCam.Priority = 20;
            noise = bossCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        core.StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {

        if (noise != null)
        {
            noise.m_AmplitudeGain = shakeAmplitude;
            noise.m_FrequencyGain = shakeFrequency;
        }

        yield return new WaitForSeconds(1.25f); 

        if (noise != null)
        {
            noise.m_AmplitudeGain = 0f;
            noise.m_FrequencyGain = 0f;
        }

        if (bossCam != null)
        {
            bossCam.Priority = 5;
        }

        yield return new WaitForSeconds(dieAnim.length);

        if (bossKeyPrefab != null)
        {
            Instantiate(bossKeyPrefab, core.transform.position + Vector3.up * 1.5f, Quaternion.identity);
        }

        Destroy(core.gameObject);

        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null && audioManager.musicClip != null)
        {
            audioManager.musicAudioSource.clip = audioManager.musicClip;
            audioManager.musicAudioSource.Play();
        }

    }

    public override void Do()
    {
        HandleDeathEffect();
    }
}
