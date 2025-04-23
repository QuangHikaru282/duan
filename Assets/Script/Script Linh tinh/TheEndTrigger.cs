using UnityEngine;
using System.Collections;

public class TheEndTrigger : MonoBehaviour
{
    public GameObject TheEnd;
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;

        if (collision.CompareTag("Player"))
        {
            triggered = true;
            if (TheEnd != null)
                TheEnd.SetActive(true);
        }
    }
}
