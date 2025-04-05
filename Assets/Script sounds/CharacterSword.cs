using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSword : MonoBehaviour
{
    public AudioSource audioSource; // Tham chiếu đến AudioSource
    public AudioClip slashSound; // Âm thanh chém
  
    
       void Start()
    {
         if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
            
    }
    public CharacterSword attackSound;
void Update()
{
    
    if (Input.GetKeyDown(KeyCode.J)) // Chém khi nhấn phím Space
    {
        
        attackSound.PlaySlashSound();
    }
}
     public void PlaySlashSound()
    {
        if (slashSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(slashSound);
        }
    }
    

}
 

   
    
   

