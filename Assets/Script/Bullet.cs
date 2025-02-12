using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;          // Toc do di chuyen cua dan
    public float lifeTime = 5f;        // Thoi gian ton tai cua dan truoc khi tu huy

    private Vector2 direction;
    private int facingDirection;
    private SpriteRenderer spriteRenderer;

    public int damage = 3;             // Sat thuong cua dan

    void Start()
    {
        // Huy dan sau khi het thoi gian ton tai
        Destroy(gameObject, lifeTime);

        // Lay SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Lat sprite dua tren huong
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
        // Di chuyen dan
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
            // Goi ham de xu ly khi dan trung ke dich
            EnemyMovement enemy = collision.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, "Range", facingDirection);
            }

            // Huy dan sau khi trung ke dich
            Destroy(gameObject);
        }
/*        else if (!collision.CompareTag("Player"))
        {
            // Huy dan khi va cham voi bat ky thu gi khac tru nguoi choi
            Destroy(gameObject);
        }*/
    }
}
