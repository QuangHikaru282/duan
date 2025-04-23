using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var elevator = GetComponentInParent<Elevator>();
        if (elevator != null)
        {
            elevator.Activate();
        }
    }
}
