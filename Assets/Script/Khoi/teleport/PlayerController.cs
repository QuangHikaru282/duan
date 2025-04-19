using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private TeleportPoint currentTeleportPoint; // Lưu điểm dịch chuyển đang va chạm

    // Kiểm tra mỗi frame
    private void Update()
    {
        // Nếu nhấn phím F và đang va chạm với điểm dịch chuyển
        if (Input.GetKeyDown(KeyCode.F) && currentTeleportPoint != null)
        {
            currentTeleportPoint.TeleportPlayer(gameObject);
        }
    }

    // Khi va chạm với điểm dịch chuyển
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TeleportPoint"))
        {
            currentTeleportPoint = other.GetComponent<TeleportPoint>();
        }
    }

    // Khi rời khỏi điểm dịch chuyển
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("TeleportPoint"))
        {
            currentTeleportPoint = null;
        }
    }
}
