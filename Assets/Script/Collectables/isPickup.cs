using UnityEngine;

public class BulletPickup : MonoBehaviour
{
    public int bulletAmount = 3; // Số lượng đạn mà vật phẩm cung cấp

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Lấy script của người chơi để thêm đạn
            playerScript player = collision.GetComponent<playerScript>();
            if (player != null)
            {
                player.AddBullets(bulletAmount);

            }

            // Hủy vật phẩm sau khi nhặt
            Destroy(gameObject);

        }
    }
}

