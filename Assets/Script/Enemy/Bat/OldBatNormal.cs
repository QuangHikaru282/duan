using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class EnemyMovement : MonoBehaviour
{
    [Header("Health Settings")]
    private int currentHealth;             // Màu hiện tại của quái
    public Slider healthBar;               // Thanh máu của quái

    [Header("Particle Effects")]
    public GameObject meleeEffect;         // Hiệu ứng hạt khi bị tấn công gần chiến
    public GameObject rangeEffect;         // Hiệu ứng hạt khi bị tấn công tầm xa

    public float speed = 2f;               // Tốc độ di chuyển
    public Vector2 pointB;                 // Vị trí kết thúc

    private Vector2 pointA;                // Vị trí bắt đầu
    private Vector2 targetPosition;        // Vị trí mục tiêu hiện tại
    private Rigidbody2D rb;

    [Header("Stun Settings")]
    public float stunDuration = 3f;        // Thời gian quái bị choáng

    private Collider2D enemyCollider;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isStunned = false;        // Trạng thái choáng
    private bool isDead = false;           // Trạng thái chết
    private float stunTimer = 0f;          // Bộ đếm thời gian choáng

    [Header("Platform Unlock Settings")]
    public CustomMovingPlatform linkedPlatform;     // Nền tảng được liên kết để mở khóa
    public NotiUIScript notificationUI;             // Tham chiếu đến script NotiUIScript để quản lý UI thông báo
    public string platformUnlockedMessage = "Platform has been unlocked!"; // Thông điệp khi nền tảng được mở khóa

    void Start()
    {
        // Lưu vị trí ban đầu
        pointA = transform.position;

        // Mục tiêu ban đầu là pointB
        targetPosition = pointB;

        // Lấy các component cần thiết
        rb = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Thiết lập thanh máu
        if (healthBar != null)
        {
            // Đặt currentHealth bằng giá trị max của healthBar
            currentHealth = (int)healthBar.maxValue;
            healthBar.value = healthBar.maxValue;
        }
        else
        {
            // Nếu không có thanh máu, bạn có thể đặt giá trị mặc định cho currentHealth
            currentHealth = 5; // Giá trị mặc định
        }

        // Tắt UI thông báo mở khóa nền tảng ban đầu
        if (notificationUI != null)
        {
            notificationUI.gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if (!isDead)
        {
            if (isStunned)
            {
                // Đếm thời gian choáng
                stunTimer -= Time.fixedDeltaTime;
                if (stunTimer <= 0f)
                {
                    RecoverFromStun();
                }
            }
            else
            {
                // Di chuyển về targetPosition
                Vector2 currentPosition = rb.position;
                Vector2 direction = (targetPosition - currentPosition).normalized;

                // Khoảng cách cần di chuyển trong frame
                float distanceThisFrame = speed * Time.fixedDeltaTime;

                // Di chuyển quái
                Vector2 newPosition = currentPosition + direction * distanceThisFrame;
                rb.MovePosition(newPosition);

                // Liệt kê hướng di chuyển và lật sprite
                if (direction.x > 0.01f)
                {
                    spriteRenderer.flipX = false; // Di chuyển sang phải
                }
                else if (direction.x < -0.01f)
                {
                    spriteRenderer.flipX = true; // Di chuyển sang trái
                }

                // Kiểm tra nếu quái đã đến gần vị trí mục tiêu
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
        }
    }

    // Xử lý va chạm với người chơi
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Bounds playerBounds = collision.collider.bounds;
            Bounds enemyBounds = enemyCollider.bounds;

            // Kiểm tra nếu người chơi nhảy lên đầu quái
            if (playerBounds.min.y >= enemyBounds.max.y - 0.1f)
            {
                if (isStunned)
                {
                    // Quái bị tấn công lần thứ hai khi đang choáng => chết
                    Die();

                    // Người chơi bật lên
                    Rigidbody2D playerRb = collision.collider.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        playerRb.velocity = new Vector2(playerRb.velocity.x, 10f); // Điều chỉnh giá trị y để thay đổi độ cao bật lên
                    }
                }
                else
                {
                    // Quái bị choáng
                    Stun();

                    // Người chơi bật lên
                    Rigidbody2D playerRb = collision.collider.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        playerRb.velocity = new Vector2(playerRb.velocity.x, 10f);
                    }
                }
            }
            else
            {
                // Lấy script của người chơi
                playerScript player = collision.collider.GetComponent<playerScript>();
                if (player != null)
                {
                    // Gây sát thương cho người chơi
                    player.TakeDamage(1);
                }
            }
        }
    }

    void Stun()
    {
        isStunned = true;
        stunTimer = stunDuration;

        // Nếu có Animator, chuyển sang trạng thái choáng
        if (animator != null)
        {
            animator.SetBool("isStunned", true);
        }

        // Dừng di chuyển (đã xử lý trong FixedUpdate)
    }

    void RecoverFromStun()
    {
        isStunned = false;

        // Nếu có Animator, chuyển về trạng thái bình thường
        if (animator != null)
        {
            animator.SetBool("isStunned", false);
        }
    }

    // Overloaded TakeDamage methods để đảm bảo tính tương thích
    public void TakeDamage(int damage)
    {
        TakeDamage(damage, "Melee", 1); // Default damageType và attackDirection
    }

    public void TakeDamage(int damage, string damageType, int attackDirection)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        Debug.Log("Bat took damage: " + damage + ", current health: " + currentHealth);

        // Cập nhật thanh máu
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        // Hiển thị hiệu ứng hạt
        ShowHitEffect(damageType, attackDirection);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void ShowHitEffect(string damageType, int attackDirection)
    {
        GameObject effectPrefab = null;

        if (damageType == "Melee" && meleeEffect != null)
        {
            effectPrefab = meleeEffect;
        }
        else if (damageType == "Range" && rangeEffect != null)
        {
            effectPrefab = rangeEffect;
        }

        if (effectPrefab != null)
        {
            // Tạo hiệu ứng hạt tại vị trí quái
            GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);

            // Xoay hiệu ứng dựa trên hướng tấn công
            Vector3 scale = effect.transform.localScale;
            scale.x *= attackDirection;
            effect.transform.localScale = scale;

            // Hủy hiệu ứng sau một thời gian (nếu cần)
            // Destroy(effect, 1f); // Thời gian tùy thuộc vào hiệu ứng của bạn
        }
    }

    void Die()
    {
        if (isDead)
            return;

        isDead = true;

        // Animator
        if (animator != null)
        {
            Debug.Log("Die parameter is true");
            animator.SetBool("isDead", true);
        }

        // Vô hiệu hóa collider
        enemyCollider.enabled = false;

        // Coroutine để quái rơi xuống và mở khóa nền tảng
        StartCoroutine(FallAndUnlockPlatform());
    }

    IEnumerator FallAndUnlockPlatform()
    {
        float fallSpeed = 2f; // Tốc độ rơi
        float fallDistance = 3f; // Khoảng cách rơi
        float startY = transform.position.y;
        float endY = startY - fallDistance;

        while (transform.position.y > endY)
        {
            transform.position -= new Vector3(0, fallSpeed * Time.deltaTime, 0);
            yield return null;
        }

        // Mở khóa nền tảng
        if (linkedPlatform != null)
        {
            linkedPlatform.UnlockPlatform();
        }

        // Hiển thị thông báo mở khóa nền tảng
        if (notificationUI != null)
        {
            notificationUI.ShowNotification(platformUnlockedMessage);
        }

        // Hủy đối tượng sau khi hoàn thành
        Destroy(gameObject);
    }
}
