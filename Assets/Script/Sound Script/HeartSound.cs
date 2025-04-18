using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartSound : MonoBehaviour
{
    private AudioManager audioManager;
    // Start is called before the first frame update
   private void Awake()
   {
    audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
   }
   private void OnTriggerEnter2D(Collider2D collision)
   {
    if (collision.CompareTag("Player"))
    {
        audioManager.PlaySFX(audioManager.heartClip);
        Destroy(gameObject);
    }
   }
}
