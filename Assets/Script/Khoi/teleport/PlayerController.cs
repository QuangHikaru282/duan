using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private TeleportPoint currentTeleportPoint; 

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && currentTeleportPoint != null)
        {
            currentTeleportPoint.TeleportPlayer(gameObject);
            currentTeleportPoint = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TeleportPoint"))
        {
            currentTeleportPoint = other.GetComponent<TeleportPoint>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("TeleportPoint"))
        {
            currentTeleportPoint = null;
        }
    }
}
