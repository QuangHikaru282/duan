using UnityEngine;
using UnityEngine.Audio;

public class BossMusicManager : MonoBehaviour
{
    public AudioClip introMusic;
    public AudioClip phase2Music;
    private AudioSource audioSource;

    [SerializeField]
    private AudioMixer myMixer; // Thêm dòng này để dùng chung AudioMixer

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Áp dụng âm lượng khi bắt đầu
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            float volume = PlayerPrefs.GetFloat("musicVolume");
            myMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        }
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
