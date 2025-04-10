using UnityEngine;

public class WaterfallTrigger : MonoBehaviour
{
    private WaterfallDisappearance waterfallDisappearance;

    void Start()
    {
        // Lấy tham chiếu đến script WaterfallDisappearance từ các đối tượng con
        waterfallDisappearance = GetComponentInChildren<WaterfallDisappearance>();

        if (waterfallDisappearance == null)
        {
            Debug.LogError("Không tìm thấy WaterfallDisappearance trong các đối tượng con của Waterfall.");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra nếu va chạm với người chơi
        if (collision.CompareTag("Player"))
        {


            if (waterfallDisappearance != null)
            {
                waterfallDisappearance.StartDisappearingEffect();
            }
        }
    }
}
