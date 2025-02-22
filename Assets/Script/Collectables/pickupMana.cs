using UnityEngine;
using System.Collections;

public class ManaPickup : MonoBehaviour
{
    [Header("Mana Settings")]
    public int manaAmount = 10;
    private bool isPickedUp = false;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isPickedUp && collision.CompareTag("Player"))
        {
            isPickedUp = true;

            // Gọi AddMana
            ManaManager.Instance.AddMana(manaAmount);

            // Chuyển animation sang "collected"
            if (animator != null)
            {
                animator.SetBool("isPickedUp", true);
            }

            // Tắt collider để không bị trigger lần nữa
            Collider2D col2d = GetComponent<Collider2D>();
            if (col2d != null)
            {
                col2d.enabled = false;
            }

            // Chờ 0.6s => hủy object
            StartCoroutine(DestroyAfterCollected(0.6f));
        }
    }

    IEnumerator DestroyAfterCollected(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
