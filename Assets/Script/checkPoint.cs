using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isActive = false;

    // Tham chiếu đến UI thông báo
    public GameObject checkpointUI;

    // Tham chiếu đến Animator của Checkpoint
    private Animator animator;

    void Start()
    {
        // Lấy component Animator
        animator = GetComponent<Animator>();

        // Đảm bảo UI thông báo tắt ban đầu
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
