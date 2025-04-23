using UnityEngine;

public class BulletPickup : MonoBehaviour
{
    public int bulletAmount = 5;
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerScript player = collision.GetComponent<playerScript>();
            if (player != null)
            {
                audioManager.PlaySFX(audioManager.arrowClip);
                player.AddBullets(bulletAmount);

            }

            Destroy(gameObject);

        }
    }
}

