using UnityEngine;
using System.Collections;

public class KeyPickup : MonoBehaviour
{
    public KeyType keyType;
    public GameObject collectionNotificationUI;
    public float displayDuration = 0.8f;
    public string collectedMessage = "You have collected the key!";

    private Animator animator;
    private Collider2D col;
    private bool hasCollected = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasCollected) return;
        hasCollected = true;

        if (!collision.CompareTag("Player")) return;

        var inventory = collision.GetComponent<KeyInventory>();
        if (inventory != null)
        {
            if (col != null) col.enabled = false;

            inventory.AddKey(keyType);

            if (collectionNotificationUI != null)
            {
                collectionNotificationUI.SetActive(true);
                var text = collectionNotificationUI.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (text != null)
                    text.text = collectedMessage;

                StartCoroutine(HideNotification());
            }

            if (animator != null)
            {
                animator.SetTrigger("CollectedTrigger");
                StartCoroutine(DestroyAfterAnimation(0.8f));
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator HideNotification()
    {
        yield return new WaitForSeconds(displayDuration);
        if (collectionNotificationUI != null)
            collectionNotificationUI.SetActive(false);
    }

    IEnumerator DestroyAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
