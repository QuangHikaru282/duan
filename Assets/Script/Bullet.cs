using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;    // Tốc độ di chuyển của đạn
    public float lifeTime = 5f;  // Thời gian tồn tại của đạn trước khi tự hủy

    private Vector2 direction;
    private int facingDirection;
    private SpriteRenderer spriteRenderer;

    public int damage = 3;       // Sát thương của đạn

    void Start()
    {
        // Hủy đạn sau khi hết thời gian tồn tại
        Destroy(gameObject, lifeTime);

        // Lấy SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();

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

    void Update()
    {
        // Di chuyển đạn
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    public void SetDamage(int damageValue)
    {
        damage = damageValue;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            IEnemy enemyScript = collision.GetComponent<IEnemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(damage, "Range", facingDirection);
            }
            Destroy(gameObject);
        }

        else if (collision.CompareTag("Ground") || collision.CompareTag("Obstacle"))
        {
            // Chạm sàn hoặc chướng ngại => hủy đạn
            Destroy(gameObject);
        }
        // Ngược lại, nếu collider không phải Enemy / Ground / Obstacle:
        // => không hủy đạn, cho nó bay xuyên
    }
}
