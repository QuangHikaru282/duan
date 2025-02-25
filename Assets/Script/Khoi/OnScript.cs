using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnScript : MonoBehaviour
{
    public GameObject targetObject; // Object chứa script cần bật
    public MonoBehaviour targetScript; // Script cần bật

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            targetScript.enabled = true; // Bật script
            Debug.Log("Script đã được bật!");
        }
    }
}
