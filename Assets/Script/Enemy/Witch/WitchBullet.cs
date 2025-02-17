using UnityEngine;
using System.Collections;

public class WitchBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 5f;       // Tốc độ bay của đạn
    public float lifeTime = 5f;     // Thời gian tồn tại của đạn trước khi tự hủy
    public int damage = 0;          // Sát thương của đạn


    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool hasExploded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Hủy sau lifeTime
        Destroy(gameObject, lifeTime);
        Vector2 bulletDirection = transform.right;
        rb.velocity = bulletDirection * speed;
    }

    void Update()
    {
        // Đạn di chuyển tự động theo velocity; không cần xử lý logic Update thêm.
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasExploded)
            return;

        // Kiểm tra va chạm với Player, Ground hoặc MovingPlaform
        if (collision.CompareTag("Player") || collision.CompareTag("Ground") || collision.CompareTag("MovingPlaform"))
        {
            // Nếu va chạm với Player thì gây sát thương
            if (collision.CompareTag("Player"))
            {
                playerScript ps = collision.GetComponent<playerScript>();
                if (ps != null)
                {
                    ps.TakeDamage(damage);
                }
            }

            // Nếu có Animator, kích hoạt trigger explosion
            Animator anim = GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("ExplodeTrigger");
            }

            // Dừng chuyển động của đạn
            rb.velocity = Vector2.zero;
            hasExploded = true;

            // Bắt đầu coroutine để chờ animation explosion (0.7 giây) rồi destroy object
            StartCoroutine(ExplosionRoutine());
        }
    }
    IEnumerator ExplosionRoutine()
    {
        // Chờ 0.7 giây cho đến khi animation explosion hoàn thành
        yield return new WaitForSeconds(0.7f);
        Destroy(gameObject);
    }

}
