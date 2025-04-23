using UnityEngine;
using System.Collections;

public class Unlockable : MonoBehaviour
{
    [Header("Key Requirement")]
    public KeyType requiredKeyType;
    public int requiredAmount = 1;

    [Header("UI Feedback")]
    public GameObject unlockPromptUI;
    public GameObject lockNotificationUI;
    public string requiredKeyMessage = "You need a key to unlock this.";
    public string unlockedMessage = "Unlocked successfully!";

    [Header("Animation & Target")]
    public Animator animator;

    [Header("Unlock Behavior")]
    public bool destroyAfterUnlock = false;
    public float destroyDelay = 1f;

    [Header("Optional Activation")]
    public BoxCollider2D elevatorTriggerCollider;


    private bool isPlayerNearby = false;
    private bool isUnlocked = false;

    void Start()
    {
        if (unlockPromptUI != null)
            unlockPromptUI.SetActive(false);

        if (lockNotificationUI != null)
            lockNotificationUI.SetActive(false);

        if (elevatorTriggerCollider != null)
            elevatorTriggerCollider.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isUnlocked && isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            TryUnlock();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isUnlocked)
            return;

        isPlayerNearby = true;

        var inventory = other.GetComponent<KeyInventory>();
        if (inventory == null)
            return;

        if (inventory.HasKey(requiredKeyType, requiredAmount))
        {
            if (unlockPromptUI != null)
                unlockPromptUI.SetActive(true);

            if (lockNotificationUI != null)
                lockNotificationUI.SetActive(false);
        }
        else
        {
            if (lockNotificationUI != null)
            {
                lockNotificationUI.SetActive(true);  // <<== DÒNG NÀY gọi pressF UI
                var text = lockNotificationUI.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (text != null)
                text.text = requiredKeyMessage;
            }

            if (unlockPromptUI != null)
                unlockPromptUI.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        isPlayerNearby = false;

        if (unlockPromptUI != null)
            unlockPromptUI.SetActive(false);

        if (lockNotificationUI != null)
            lockNotificationUI.SetActive(false);
    }

    void TryUnlock()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        var inventory = player.GetComponent<KeyInventory>();
        if (inventory == null || !inventory.UseKey(requiredKeyType, requiredAmount))
            return;

        isUnlocked = true;

        if (animator != null)
            animator.SetTrigger("Unlock");

        if (lockNotificationUI != null)
        {
            lockNotificationUI.SetActive(true);
            var text = lockNotificationUI.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (text != null)
                text.text = unlockedMessage;
        }

        if (unlockPromptUI != null)
            unlockPromptUI.SetActive(false);

        if (elevatorTriggerCollider != null)
            elevatorTriggerCollider.gameObject.SetActive(true);

        if (destroyAfterUnlock)
            StartCoroutine(DestroyAfterDelay());
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
