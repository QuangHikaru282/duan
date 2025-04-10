using UnityEngine;

public class BulletPickup : MonoBehaviour
{
    public int bulletAmount = 3; // Số lượng đạn mà vật phẩm cung cấp
    public AudioClip pickupSound; 
    public AudioSource audioSource;

    void OnTriggerEnter2D(Collider2D collision)
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.Play(); // nhớ gọi Destroy sau một khoảng delay
        if (collision.CompareTag("Player"))
        {
            // Lấy script của người chơi để thêm đạn
            playerScript player = collision.GetComponent<playerScript>();
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            if (player != null)
            {
                player.AddBullets(bulletAmount);

            }

            // Hủy vật phẩm sau khi nhặt
            Destroy(gameObject);

        }
    }
}

