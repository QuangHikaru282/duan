using UnityEngine;
using TMPro; // TextMeshPro
using System.Collections;

public class Lock : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject lockNotificationUI;          // tham chieu den UI thong bao khi tiep can o khoa
    public string requiredKeyMessage = "You need a key to unlock this platform."; // thong diep yeu cau chia khoa
    public string unlockedMessage = "Platform has been unlocked!"; // thong diep khi mo khoa thanh cong

    [Header("Platform Settings")]
    public CustomMovingPlatform linkedPlatform;     // tham chieu den nen tang duoc lien ket

    [Header("Unlock Prompt Settings")]
    public GameObject unlockPromptUI;              // tham chieu den UI thong bao "Press E to unlock"

    private bool isPlayerNearby = false;
    private Animator animator;                     // them Animator
    private bool isUnlocked = false;               // trang thai da mo khoa

    void Start()
    {
        if (lockNotificationUI != null)
        {
            lockNotificationUI.SetActive(false);  // tat UI thong bao yeu cau chia khoa
        }

        if (unlockPromptUI != null)
        {
            unlockPromptUI.SetActive(false);      // tat UI prompt "Press E to unlock"
        }

        // lay Animator component neu co
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // kiem tra neu player co trong pham vi, chua mo khoa, va nhan phim E
        if (isPlayerNearby && !isUnlocked && Input.GetKeyDown(KeyCode.E))
        {
            AttemptUnlock();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isUnlocked)
        {
            isPlayerNearby = true;
            
            ShowLockNotification(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
           
            HideLockNotification();
        }
    }

    void ShowLockNotification(GameObject player)
    {
        if (player == null)
            return;

        playerScript playerScript = player.GetComponent<playerScript>();
        if (playerScript == null)
            return;

        if (playerScript.keyCount > 0)
        {
            if (unlockPromptUI != null)
            {
                unlockPromptUI.SetActive(true);   // hien thi UI nhan E de mo khoa
            }

            if (lockNotificationUI != null)
            {
                lockNotificationUI.SetActive(false); // tat UI thong bao yeu cau chia khoa
            }
        }
        else
        {
  
            if (lockNotificationUI != null)
            {
                lockNotificationUI.SetActive(true);  // hien thi UI yeu cau chia khoa
                // thiet lap thong diep yeu cau chia khoa
                TextMeshProUGUI textComponent = lockNotificationUI.GetComponentInChildren<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.text = requiredKeyMessage;
                }
            }

            if (unlockPromptUI != null)
            {
                unlockPromptUI.SetActive(false);    // tat UI nhan E de mo khoa
            }
        }
    }

    void HideLockNotification()
    {
        if (lockNotificationUI != null)
        {
            lockNotificationUI.SetActive(false); // tat UI thong bao yeu cau chia khoa
        }

        if (unlockPromptUI != null)
        {
            unlockPromptUI.SetActive(false);     // tat UI prompt "Press E to unlock"
        }
    }

    void AttemptUnlock()
    {
        playerScript player = FindObjectOfType<playerScript>();
        if (player != null && player.UseKey())
        {
            // mo khoa nen tang
            if (linkedPlatform != null)
            {
                linkedPlatform.UnlockPlatform();
            }

            // hien thi thong bao da mo khoa
            if (lockNotificationUI != null)
            {
                lockNotificationUI.SetActive(true); // hien thi UI thong bao mo khoa
                TextMeshProUGUI textComponent = lockNotificationUI.GetComponentInChildren<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.text = unlockedMessage;
                }

                // tu dong tat thong bao sau 2 giay
                Invoke("HideLockNotification", 2f);
            }

            // chay animation mo khoa neu co
            if (animator != null)
            {
                animator.SetTrigger("Unlock");
            }

            // tat prompt "Press E to unlock" neu dang hien thi
            if (unlockPromptUI != null)
            {
                unlockPromptUI.SetActive(false);
            }

            // set isUnlocked = true va disable o khoa
            isUnlocked = true;
            StartCoroutine(DisableLock());
        }
        else
        {
            // neu khong co chia khoa, hien thi thong bao hoac thong bao khac
           
        }
    }

    IEnumerator DisableLock()
    {

        yield return new WaitForSeconds(2f);

        // Vo hieu hoa cac collider de khong nhan su va cham nua
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
    }

}
