using UnityEngine;

public class WitchDetectionArea : MonoBehaviour
{
    private WitchNormal witch;

    void Start()
    {
        witch = GetComponentInParent<WitchNormal>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (witch != null)
                witch.OnPlayerDetected();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (witch != null)
                witch.OnPlayerLost();
        }
    }
}
