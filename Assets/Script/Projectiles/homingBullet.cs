using UnityEngine;
using Pathfinding;  // AIPath, AIDestinationSetter

public class HomingBullet : MonoBehaviour
{
    [Header("Lifetime & Damage")]
    public float lifeTime = 5f;
    public int damage = 5;

    [Header("Animation")]
    public Animator animator;  // Kéo vào hoặc get ở Awake

    [Header("AI Components")]
    public AIPath aiPath;             // Gắn sẵn
    public AIDestinationSetter aiDest; // Gắn sẵn

    private int facingDirection;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool hasHit = false;
    private bool hasTarget = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null)
            animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Tắt AIPath ban đầu
        if (aiPath != null)
            aiPath.enabled = false;
    }

    void Start()
    {
        // Tự huỷ sau lifeTime
        Destroy(gameObject, lifeTime);

        float speed = 6f;
        Vector2 initialDir = Vector2.right;

        // Dùng vị trí spawn của đạn so với player để suy ra hướng bắn
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Nếu vị trí bullet bên trái so với player, tức bắn sang trái
            if (transform.position.x < player.transform.position.x)
            {
                initialDir = Vector2.left;
                spriteRenderer.flipX = true;
            }
            else
            {
                initialDir = Vector2.right;
                spriteRenderer.flipX = false;
            }
        }

        rb.velocity = initialDir * speed;
        facingDirection = (initialDir.x < 0) ? -1 : 1;
    }

    void Update()
    {
        Vector2 vel;
        if (aiPath != null && aiPath.enabled && hasTarget)
        {
            vel = aiPath.desiredVelocity;
        }
        else
        {
            vel = rb.velocity;
        }

        if (vel.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
            // Nếu sprite đã được flipX (bắn sang trái), hiệu chỉnh góc xoay
            if (spriteRenderer.flipX)
            {
                angle -= 180;
            }
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }


    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("phat hien va cham!");
        if (hasHit) return;

        // Va chạm Ground / Platform
        if (col.collider.CompareTag("Ground") || col.collider.CompareTag("MovingPlaform"))
        {
            TriggerHit();
        }
        // Va chạm Enemy => gây damage + nổ
        else if (col.collider.CompareTag("Enemy"))
        {
            IEnemy enemy = col.collider.GetComponent<IEnemy>();
            if (enemy != null)
            {
                // Tính hướng va chạm => hiển thị effect Range
                Vector2 bulletPos = transform.position;
                Vector2 enemyPos = col.collider.transform.position;
                float dx = enemyPos.x - bulletPos.x;
                int sign = (dx >= 0) ? 1 : -1;
                enemy.TakeDamage(damage, "Range", sign);
            }
            TriggerHit();
        }
    }

    void TriggerHit()
    {
        hasHit = true;
        rb.velocity = Vector2.zero;

        if (animator != null)
            animator.SetTrigger("HitTrigger");

        // Tắt collider để không va chạm liên tục
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        // Tắt AIPath => dừng chase
        if (aiPath != null)
            aiPath.enabled = false;

        // Hủy sau 0.6s (thời gian anim Hit)
        Destroy(gameObject, 0.6f);
    }

    // Gọi khi detection zone quét 1 enemy => bullet chase
    public void SetTarget(Transform enemy)
    {
        if (aiDest != null && enemy != null)
        {
            aiDest.target = enemy;
        }
        if (aiPath != null)
            aiPath.enabled = true;
        hasTarget = true;

        // Tắt velocity cũ, để A* chi phối
        rb.velocity = Vector2.zero;
    }
}