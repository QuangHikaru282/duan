using UnityEngine;
using System.Collections;
using TMPro; // Đảm bảo rằng bạn đã cài đặt TextMeshPro trong dự án

public class NotiUIScript : MonoBehaviour
{
    public TextMeshProUGUI notificationText; // Tham chiếu đến component TextMeshProUGUI để hiển thị thông điệp
    public float displayDuration = 2f;      // Thời gian hiển thị thông báo

    public void ShowNotification(string message)
    {
        if (notificationText != null)
        {
            notificationText.text = message;
            gameObject.SetActive(true);
            StartCoroutine(HideAfterDelay());
        }
        else
        {
            Debug.LogWarning("Notification TextMeshProUGUI is not assigned in NotiUIScript.");
        }
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        gameObject.SetActive(false);
    }
}
