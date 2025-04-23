using UnityEngine;

public class BatDetectionArea : MonoBehaviour
{
    private BatNormal batNormal;

    void Start()
    {
        batNormal = GetComponentInParent<BatNormal>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Enter => StartChase");
            batNormal.StartChase(other.transform);
        }
    }


    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            batNormal.StopChase();
        }
    }
}
