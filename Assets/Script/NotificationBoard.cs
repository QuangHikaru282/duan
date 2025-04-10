using UnityEngine;
using TMPro;

public class NotificationBoard : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject notificationUI;               // Tham chiếu đến UI thông báo
    public string message = "This is a notification!"; // Thông điệp hiển thị

    [Header("Display Settings")]
    public float displayDuration = 2f;              // Thời gian hiển thị thông báo (nếu muốn thông báo tự động tắt sau thời gian)
    public bool shouldAutoHide = false;             // Có tự động tắt thông báo sau thời gian hay không

    //private bool isPlayerNearby = false;

    void Start()
    {
        if (notificationUI != null)
        {
            notificationUI.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ShowNotification();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HideNotification();
        }
    }

    public void ShowNotification()
    {
        if (notificationUI != null)
        {
            notificationUI.SetActive(true);
            // Nếu sử dụng TextMeshPro để hiển thị thông điệp
            TextMeshProUGUI textComponent = notificationUI.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = message;
            }

            if (shouldAutoHide)
            {
                CancelInvoke("HideNotification"); // Hủy bất kỳ lời gọi Hide trước đó
                Invoke("HideNotification", displayDuration);
            }
        }
    }

    public void HideNotification()
    {
        if (notificationUI != null)
        {
            notificationUI.SetActive(false);
        }
    }
}
