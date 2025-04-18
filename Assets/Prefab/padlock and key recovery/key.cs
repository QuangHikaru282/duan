using UnityEngine;
using System.Collections;

public class Key : MonoBehaviour
{
    [Header("Key Settings")]
    public int keyValue = 1; // Giá trị của chìa khóa (mặc định là 1)

    [Header("UI Settings")]
    public GameObject collectionNotificationUI; // Tham chiếu đến UI thông báo khi thu thập chìa khóa

    [Header("Display Settings")]
    public float displayDuration = 2f;              // Thời gian hiển thị thông báo (nếu muốn thông báo tự động tắt sau thời gian)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerScript player = collision.GetComponent<playerScript>();
            if (player != null)
            {
                //player.AddKey();

                Destroy(gameObject);

                // Hiển thị thông báo thu thập chìa khóa
                if (collectionNotificationUI != null)
                {
                    collectionNotificationUI.SetActive(true);
                    StartCoroutine(HideNotification());
                }

            }
        }
    }

        IEnumerator HideNotification()
    {
        yield return new WaitForSeconds(displayDuration);
        if (collectionNotificationUI != null)
        {
            collectionNotificationUI.SetActive(false);
        }       
    }
}
