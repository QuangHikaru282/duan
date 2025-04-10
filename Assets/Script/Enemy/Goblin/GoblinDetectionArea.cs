using UnityEngine;

public class GoblinDetectionArea : MonoBehaviour
{
    private GoblinNormal goblin;

    void Start()
    {
        goblin = GetComponentInParent<GoblinNormal>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Bắt đầu chase
            goblin.StartChase(other.transform);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Player rời vùng => dừng chase => goblin về state Return
            goblin.StopChase();
        }
    }
}
