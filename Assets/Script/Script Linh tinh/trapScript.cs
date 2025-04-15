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
}
