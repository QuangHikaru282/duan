using UnityEngine;

public class BossMusicManager : MonoBehaviour
{
    public AudioClip introMusic;
    public AudioClip phase2Music;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayIntroTheme()
    {
        if (introMusic != null)
        {
            audioSource.clip = introMusic;
            audioSource.Play();
        }
    }

    public void PlayPhase2Theme()
    {
        if (phase2Music != null)
        {
            audioSource.clip = phase2Music;
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

}
