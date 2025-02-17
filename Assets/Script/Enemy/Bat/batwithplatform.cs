using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class BatWithPlatform : MonoBehaviour
{
    [Header("Health Settings")]
    private int currentHealth;             // Mau hien tai cua ke dich
    public Slider healthBar;               // Thanh mau cua ke dich

    [Header("Particle Effects")]
    public GameObject meleeEffect;         // Hieu ung hat khi bi tan cong canh chien
    public GameObject rangeEffect;         // Hieu ung hat khi bi tan cong tam xa

    public float speed = 2f;               // Toc do di chuyen
    public Vector2 pointB;                 // Vi tri ket thuc

    private Vector2 pointA;                // Vi tri bat dau
    private Vector2 targetPosition;        // Vi tri muc tieu hien tai
    private Rigidbody2D rb;

    [Header("Stun Settings")]
    public float stunDuration = 3f;        // Thoi gian con dai bi choang

    private Collider2D enemyCollider;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isStunned = false;        // Trang thai choang
    private bool isDead = false;           // Trang thai chet
    private float stunTimer = 0f;          // Bo dem thoi gian choang

    [Header("Platform Unlock Settings")]
    public CustomMovingPlatform linkedPlatform;     // Nen tang duoc lien ket de mo khoa
    public GameObject platformUnlockUI;            // UI thong bao khi platform duoc mo khoa
    public string platformUnlockedMessage = "Platform has been unlocked!"; // Thong diep khi platform duoc mo khoa
    public float unlockDisplayDuration = 2f;       // Thoi gian hien thi thong bao mo khoa

    void Start()
    {
        // Luu vi tri ban dau
        pointA = transform.position;

        // Muc tieu ban dau la pointB
        targetPosition = pointB;

        // Lay cac component can thiet
        rb = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Thiet lap thanh mau
        if (healthBar != null)
        {
            // Dat currentHealth bang gia tri max cua healthBar
            currentHealth = (int)healthBar.maxValue;
            healthBar.value = healthBar.maxValue;
        }
        else
        {
            // Neu khong co thanh mau, dat gia tri mac dinh cho currentHealth
            currentHealth = 5; // Gia tri mac dinh
        }

        // Tat UI thong bao mo khoa platform ban dau
        if (platformUnlockUI != null)
        {
            platformUnlockUI.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if (!isDead)
        {
            if (isStunned)
            {
                // Dem thoi gian choang
                stunTimer -= Time.fixedDeltaTime;
                if (stunTimer <= 0f)
                {
                    RecoverFromStun();
                }
            }
            else
            {
                // Di chuyen ve targetPosition
                Vector2 currentPosition = rb.position;
                Vector2 direction = (targetPosition - currentPosition).normalized;

                // Khoang cach can di chuyen trong frame
                float distanceThisFrame = speed * Time.fixedDeltaTime;

                // Di chuyen con dai
                Vector2 newPosition = currentPosition + direction * distanceThisFrame;
                rb.MovePosition(newPosition);

                // Liet ke huong di chuyen va lam xet flip sprite
                if (direction.x > 0.01f)
                {
                    spriteRenderer.flipX = false; // Di chuyen sang phai
                }
                else if (direction.x < -0.01f)
                {
                    spriteRenderer.flipX = true; // Di chuyen sang trai
                }

                // Kiem tra neu con dai da den gan vi tri muc tieu
                if (Vector2.Distance(newPosition, targetPosition) < 0.1f)
                {
                    // Doi huong di chuyen
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

    // Xu ly va cham voi nguoi choi
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Bounds playerBounds = collision.collider.bounds;
            Bounds enemyBounds = enemyCollider.bounds;

            // Kiem tra neu nguoi choi nhay len dau con dai
            if (playerBounds.min.y >= enemyBounds.max.y - 0.1f)
            {
                if (isStunned)
                {
                    // Con dai bi tan cong lan thu hai khi dang choang => chet
                    Die();

                    // Nguoi choi bat len
                    Rigidbody2D playerRb = collision.collider.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        playerRb.velocity = new Vector2(playerRb.velocity.x, 10f); // Dieu chinh gia tri y de thay doi do cao bat len
                    }
                }
                else
                {
                    // Con dai bi choang
                    Stun();

                    // Nguoi choi bat len
                    Rigidbody2D playerRb = collision.collider.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        playerRb.velocity = new Vector2(playerRb.velocity.x, 10f);
                    }
                }
            }
            else
            {
                // Lay script cua nguoi choi
                playerScript player = collision.collider.GetComponent<playerScript>();
                if (player != null)
                {
                    // Gay sat thuong cho nguoi choi
                    player.TakeDamage(1);
                }
            }
        }
    }

    void Stun()
    {
        isStunned = true;
        stunTimer = stunDuration;

        // Neu co Animator, chuyen sang trang thai choang
        if (animator != null)
        {
            animator.SetBool("isStunned", true);
        }

        // Dung di chuyen (da xu ly trong FixedUpdate)
    }

    void RecoverFromStun()
    {
        isStunned = false;

        // Neu co Animator, chuyen ve trang thai binh thuong
        if (animator != null)
        {
            animator.SetBool("isStunned", false);
        }
    }

    // Overloaded TakeDamage methods to ensure compatibility
    public void TakeDamage(int damage)
    {
        TakeDamage(damage, "Melee", 1); // Default damageType and attackDirection
    }

    public void TakeDamage(int damage, string damageType, int attackDirection)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        Debug.Log("Bat took damage: " + damage + ", current health: " + currentHealth);

        // Cap nhat thanh mau
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        // Hien thi hieu ung hat
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
            // Tao hieu ung hat tai vi tri ke dich
            GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);

            // Xoay hieu ung dua tren huong tan cong
            Vector3 scale = effect.transform.localScale;
            scale.x *= attackDirection;
            effect.transform.localScale = scale;

            // Huy hieu ung sau mot thoi gian (neu can)
            // Destroy(effect, 1f); // Thoi gian tuong ung voi hieu ung cua ban
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

        // Vo hieu hoa collider
        enemyCollider.enabled = false;

        // Coroutine de con dai roi xuong va mo khoa platform
        StartCoroutine(FallAndUnlockPlatform());
    }

    IEnumerator FallAndUnlockPlatform()
    {
        float fallSpeed = 2f; // Toc do roi
        float fallDistance = 3f; // Khoang cach roi
        float startY = transform.position.y;
        float endY = startY - fallDistance;

        while (transform.position.y > endY)
        {
            transform.position -= new Vector3(0, fallSpeed * Time.deltaTime, 0);
            yield return null;
        }

        // Mo khoa platform
        if (linkedPlatform != null)
        {
            linkedPlatform.UnlockPlatform();
        }

        // Hien thi thong bao mo khoa platform
        if (platformUnlockUI != null)
        {
            platformUnlockUI.SetActive(true);
            // Neu platformUnlockUI co TextMeshProUGUI, cap nhat thong diep
            TextMeshProUGUI textComponent = platformUnlockUI.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = platformUnlockedMessage;
            }

            // Tu dong tat thong bao sau thoi gian
            StartCoroutine(HideUnlockUIAfterDelay());
        }

        // Huy doi tuong sau khi roi
        Destroy(gameObject);
    }

    IEnumerator HideUnlockUIAfterDelay()
    {
        yield return new WaitForSeconds(unlockDisplayDuration);
        if (platformUnlockUI != null)
        {
            platformUnlockUI.SetActive(false);
        }
    }
}
