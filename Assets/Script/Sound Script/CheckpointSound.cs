using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSound : MonoBehaviour
{
    private bool isActive = false;
    public GameObject checkpointUI;
    private Animator animator;
   
    // Start is called before the first frame update
    private AudioManager audioManager;
    private void Awake()
   {
    audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
   }
   void Start()
    {
        // Lấy component Animator
        animator = GetComponent<Animator>();
        if (checkpointUI != null)
        {
            checkpointUI.SetActive(false);
        }
      
    }

    // Update is called once per frame
     void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!isActive)
            {
                isActive = true;
            if (audioManager != null && audioManager.checkpointClip != null)
            {
            audioManager.PlaySFX(audioManager.checkpointClip);
            }
   
              
                // Lưu vị trí checkpoint vào playerScript
                playerScript player = collision.GetComponent<playerScript>();
                if (player != null)
                {
                    Debug.Log("check point da dc luu");
                    player.SetCheckpoint(transform.position);
                }

                // Hiển thị thông báo "Checkpoint reached!"
                if (checkpointUI != null)
                {
                    checkpointUI.SetActive(true);
                }
                

                // Kích hoạt animation CheckPointSaved
                if (animator != null)
                {
                    animator.SetBool("isSaved", true);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            if (checkpointUI != null)
            {
                checkpointUI.SetActive(false);
            }

        
            if (animator != null)
            {
                animator.SetBool("isSaved", false);
            }
        }
    }
}
