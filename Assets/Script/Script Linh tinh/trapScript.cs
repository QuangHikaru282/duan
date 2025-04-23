using UnityEngine;
using System.Collections;

public class TrapScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerScript player = collision.GetComponent<playerScript>();
            if (player != null)
            {
                player.cantBeDamaged = false;
                player.TakeDamage(1);
                StartCoroutine(DelayedRespawn(player));
            }
        }
    }

    private IEnumerator DelayedRespawn(playerScript player)
    {
        yield return new WaitForSeconds(0.5f);
        player.Respawn();
    }
}
