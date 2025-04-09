using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;    // Tốc độ di chuyển của đạn
    public float lifeTime = 5f;  // Thời gian tồn tại của đạn trước khi tự hủy

    private Vector2 direction;
    private int facingDirection;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    public int damage = 3;       // Sát thương của đạn

    void Start()
    {
        // Hủy đạn sau khi hết thời gian tồn tại
        Destroy(gameObject, lifeTime);

        // Lấy Rigidbody và SpriteRenderer
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Đảm bảo đạn không bị rơi do trọng lực
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.velocity = direction * speed; // Set velocity
        }

        // Lật sprite dựa trên hướng
        if (direction.x < 0)
        {
            spriteRenderer.flipX = true;
            facingDirection = -1;
        }
        else
        {
            spriteRenderer.flipX = false;
            facingDirection = 1;
        }
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        // Nếu rb đã tồn tại, có thể cập nhật velocity ngay khi SetDirection được gọi
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
    }

    public void SetDamage(int damageValue)
    {
        damage = damageValue;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            IEnemy enemyScript = collision.GetComponentInParent<IEnemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(damage, "Range", facingDirection);
            }

            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground") || collision.CompareTag("MovingPlaform"))
        {
            Destroy(gameObject);
        }
    }

}
