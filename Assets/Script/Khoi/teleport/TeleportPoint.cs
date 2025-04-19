using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour
{
    public bool isMainPoint; // Đánh dấu đây là điểm chính (true) hay phụ (false)
    public TeleportPoint mainTeleportPoint; // Tham chiếu đến điểm chính (chỉ dùng cho điểm phụ)
    public GameObject fKeyPrefab; // Prefab "Fnew" để hiển thị nút F

    private GameObject fKeyInstance; // Biến lưu nút F khi hiển thị


    public BoxCollider2D detectionArea; // Bán kính phát hiện quái
    public LayerMask enemyLayer; // Layer của quái

    // Khi player va chạm với điểm dịch chuyển
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Hiển thị nút F ở vị trí của điểm dịch chuyển
            if (fKeyPrefab != null && fKeyInstance == null)
            {
                fKeyInstance = Instantiate(fKeyPrefab, transform.position, Quaternion.identity);
            }
        }
    }

    // Khi player rời khỏi điểm dịch chuyển
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Xóa nút F nếu đang hiển thị
            if (fKeyInstance != null)
            {
                Destroy(fKeyInstance);
                fKeyInstance = null;
            }
        }
    }

    // Hàm dịch chuyển player
    public void TeleportPlayer(GameObject player)
    {
        if (!isMainPoint && mainTeleportPoint != null)
        {
            // Kiểm tra xem có quái ở trong vùng detectionArea không
            Collider2D[] enemiesInArea = new Collider2D[10]; // Mảng để lưu các collider phát hiện được
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(enemyLayer);
            filter.useTriggers = true;

            int enemyCount = detectionArea.OverlapCollider(filter, enemiesInArea);

            if (enemyCount > 0)
            {
                Debug.Log("Không thể dịch chuyển: có quái ở gần!");
                return; // Ngăn chặn dịch chuyển
            }

            // Nếu không có quái ở gần, thực hiện dịch chuyển
            player.transform.position = mainTeleportPoint.transform.position;
        }
        else
        {
            Debug.Log("Không thể dịch chuyển: là điểm chính hoặc chưa gán mainTeleportPoint");
        }
    }


}