using UnityEngine;


public class TrapScript : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
      
            playerScript player = collision.GetComponent<playerScript>();
            if (player != null)
            {
                player.TakeDamage(1);
            }
        }
    }

    /*    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // Lấy script của người chơi
            playerScript player = collision.collider.GetComponent<playerScript>();
            if (player != null)
            {
                // Gây sát thương cho người chơi
                player.TakeDamage(1);
            }
        }
    }*/
}
