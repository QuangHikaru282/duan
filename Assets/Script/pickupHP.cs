// HealthPickup.cs
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healthAmount = 1;
    private bool isPickedUp = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isPickedUp && collision.CompareTag("Player"))
        {
            isPickedUp = true;
            playerScript player = collision.GetComponent<playerScript>();
            if (player != null)
            {
                player.AddHealth(healthAmount);
            }
            Destroy(gameObject);
        }
    }
}
