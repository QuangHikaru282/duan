using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class Save : MonoBehaviour
{
    public PlayerManager playerManager;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Đảm bảo Player có tag là "Player"
        {

            playerManager.OnSaveGameData();
            
        }
    }
}

