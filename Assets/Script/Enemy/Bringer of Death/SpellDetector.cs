using UnityEngine;

public class SpellDetector : MonoBehaviour
{
    private BringerOfDeath bod;
    private LOSController los;

    void Awake()
    {
        bod = GetComponentInParent<BringerOfDeath>();
        los = bod != null ? bod.GetComponent<LOSController>() : null;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (bod == null || los == null) return;

        if (other.CompareTag("Player") && los.isSeeingTarget)
        {
            bod.OnPlayerDetected();
        }
    }
}
