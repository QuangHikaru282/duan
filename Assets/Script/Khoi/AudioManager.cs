using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Create variables to store audio sources
    public AudioSource musicAudioSource;
    public AudioSource vfxAudioSource;

    // Create variables to store audio clips
    public AudioClip musicClip;
    public AudioClip checkpointClip;
    public AudioClip arrowClip;
    public AudioClip manaClip;
    public AudioClip heartClip;
    public AudioClip winClip;

    // Unity Message
    void Start()
    {
        musicAudioSource.clip = musicClip;
        musicAudioSource.Play();
    }

    public void PlaySFX(AudioClip sfxClip)
    {
        vfxAudioSource.clip = sfxClip;
        vfxAudioSource.PlayOneShot(sfxClip);
    }
}
