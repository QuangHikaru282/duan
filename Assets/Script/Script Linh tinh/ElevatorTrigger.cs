using UnityEngine;
using System.Collections;

public class ElevatorTrigger : MonoBehaviour
{
    public MonoBehaviour playerControllerScript;
    public float delay = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var elevator = GetComponentInParent<Elevator>();
        if (elevator != null)
        {
            StartCoroutine(HandleEndSequence());
            elevator.Activate();
        }
    }

    IEnumerator HandleEndSequence()
    {
        yield return new WaitForSeconds(delay);

        if (playerControllerScript != null)
            playerControllerScript.enabled = false;
    }
}
