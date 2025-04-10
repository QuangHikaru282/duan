using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Youlose : MonoBehaviour

{
     public AudioSource audioSource; // Kéo AudioSource vào đây trong Inspector
     public AudioClip youLose;
     public int maxHealth = 15;

    public void PlayGameOverSound()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = youLose; 
            audioSource.Play();
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (maxHealth <= 0)
        {
            PlayGameOverSound();

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
