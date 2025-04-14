using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    private IceGuardian ig;
    private LOSController los;

    void Awake()
    {
        ig = GetComponentInParent<IceGuardian>();
        los = ig != null ? ig.GetComponent<LOSController>() : null;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (ig == null || los == null) return;

        if (other.CompareTag("Player") && los.isSeeingTarget)
        {
            Debug.Log("Detector thay va bao cho IG");
            ig.OnPlayerDetected();
        }
    }
}
