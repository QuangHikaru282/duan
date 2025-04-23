using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isActive = false;
    public GameObject checkpointUI;
    private Animator animator;
    private AudioManager audioManager;

    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        animator = GetComponent<Animator>();
        if (checkpointUI != null)
        {
            checkpointUI.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!isActive)
            {
                isActive = true;

                audioManager.PlaySFX(audioManager.checkpointClip);
                playerScript player = collision.GetComponent<playerScript>();
                if (player != null)
                {
                    player.SetCheckpoint(transform.position);
                }

                if (checkpointUI != null)
                {
                    checkpointUI.SetActive(true);
                }

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
