using UnityEngine;

public class CustomMovingPlatform : MonoBehaviour
{
    public Vector2 pointB;            // Vị trí kết thúc
    public float speed = 2f;

    private Vector2 pointA;           // Vị trí bắt đầu
    private Vector2 targetPosition;   // Vị trí mục tiêu hiện tại
    private Rigidbody2D rb;

    private bool isUnlocked = false;  // Trạng thái mở khóa

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;

        // Point A là vị trí hiện tại
        pointA = transform.position;

        // Mục tiêu là pointB
        targetPosition = pointB;

        // Platform đứng yên cho tới khi được mở khóa
    }

    void FixedUpdate()
    {
        if (!isUnlocked)
            return; // Không làm gì nếu chưa được mở khóa

        Vector2 currentPosition = rb.position;
        Vector2 direction = (targetPosition - currentPosition).normalized;

        // Khoảng cách cần di chuyển trong frame
        float distanceThisFrame = speed * Time.fixedDeltaTime;

        // Di chuyển platform
        Vector2 newPosition = currentPosition + direction * distanceThisFrame;
        rb.MovePosition(newPosition);

        // Kiểm tra nếu platform đã đến gần vị trí mục tiêu thì đổi hướng
        if (Vector2.Distance(newPosition, targetPosition) < 0.1f)
        {
            // Đổi hướng di chuyển
            if (targetPosition == pointA)
            {
                targetPosition = pointB;
            }
            else
            {
                targetPosition = pointA;
            }
        }
    }

    // Khi người chơi tiếp xúc với nền tảng
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("da set parent cho player");
            // Gán người chơi làm con của nền tảng
            collision.transform.SetParent(transform);
        }
    }

    // Khi người chơi rời khỏi nền tảng
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Kiểm tra xem nền tảng có đang hoạt động không
            if (gameObject.activeInHierarchy)
            {
                // Hủy gán người chơi khỏi nền tảng
                collision.transform.SetParent(null);
            }
        }
    }

    // Phương thức để mở khóa nền tảng
    public void UnlockPlatform()
    {
        if (!isUnlocked)
        {
            isUnlocked = true;
            Debug.Log("Platform has been unlocked and will start moving.");
            // Có thể thêm hiệu ứng hoặc âm thanh mở khóa tại đây
        }
    }
}
