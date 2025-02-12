using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healthAmount = 1; // Số lượng máu mà vật phẩm cung cấp
    private bool isPickedUp = false; // Cờ kiểm tra xem vật phẩm đã được nhặt hay chưa

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isPickedUp && collision.CompareTag("Player"))
        {
            isPickedUp = true;

            // Lấy script của người chơi để thêm máu
            playerScript player = collision.GetComponent<playerScript>();
            if (player != null)
            {
                player.AddHealth(healthAmount);
            }

            // Hủy vật phẩm sau khi nhặt
            Destroy(gameObject);
        }
    }
}
