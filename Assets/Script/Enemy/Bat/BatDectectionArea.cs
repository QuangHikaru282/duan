using UnityEngine;

public class BatDetectionArea : MonoBehaviour
{
    private BatNormal batNormal; // Đổi tên biến cho đồng bộ

    void Start()
    {
        // Tìm script BatNormal ở đối tượng cha (hoặc ông cha)  
        batNormal = GetComponentInParent<BatNormal>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Enter DetectionArea with: " + other.name);
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
            // Khi player rời vùng, Bat quay lại tuần tra
            batNormal.StopChase();
        }
    }
}
