using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 2f;              // Tốc độ di chuyển
    public float moveDistance = 5f;       // Khoảng cách di chuyển từ vị trí ban đầu
    private bool movingRight = true;      // Hướng di chuyển ban đầu
    private Vector2 startPosition;        // Vị trí ban đầu của platform

    private Rigidbody2D rb;

    void Start()
    {
        // Lưu vị trí ban đầu
        startPosition = transform.position;

        // Lấy Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true; // Đảm bảo Rigidbody2D là Kinematic
    }

    void FixedUpdate()
    {
        // Tính toán vị trí mục tiêu
        float xPosition = transform.position.x;

        if (movingRight)
        {
            xPosition += speed * Time.fixedDeltaTime;

            if (xPosition >= startPosition.x + moveDistance)
            {
                xPosition = startPosition.x + moveDistance;
                movingRight = false;
            }
        }
        else
        {
            xPosition -= speed * Time.fixedDeltaTime;

            if (xPosition <= startPosition.x - moveDistance)
            {
                xPosition = startPosition.x - moveDistance;
                movingRight = true;
            }
        }

        // Di chuyển platform bằng Rigidbody2D
        rb.MovePosition(new Vector2(xPosition, transform.position.y));
    }

    // Khi một đối tượng vào collision của platform
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // Đặt platform là parent của người chơi
            collision.collider.transform.SetParent(transform);
        }
    }

    // Khi một đối tượng rời khỏi collision của platform
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // Bỏ platform khỏi parent của người chơi
            collision.collider.transform.SetParent(null);
        }
    }
}