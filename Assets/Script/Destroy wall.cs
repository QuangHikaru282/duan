using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    private bool playerInRange = false;
    private Animator animator;
    private bool isBreaking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.X) && !isBreaking)
        {
            isBreaking = true;
            animator.SetTrigger("Break"); // Gọi animation phá
        }
    }

    // Gọi từ Animation Event khi animation kết thúc
    public void DestroyWall()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
