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
        Destroy(gameObject, lifeTime);

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.velocity = direction * speed;
        }

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

        if (rb != null)
            rb.velocity = direction * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);


        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x);
        scale.x *= direction.x < 0 ? -1 : 1;
        transform.localScale = scale;
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
