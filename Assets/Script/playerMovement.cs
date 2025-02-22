using UnityEngine;
using System.Collections;

public class playerScript : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 10f;
    public float maxSpeed = 5f;
    public float movementSmoothing = 0.05f;

    [Header("Health Settings")]
    public int maxHealth = 15;
    public int currentHealth;

    [Header("Respawn Settings")]
    public Vector2 respawnPosition;

    [Header("Jump Settings")]
    public float jumpForce = 15f;
    public int maxJumpCount = 2;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public LayerMask platformLayer;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public int arrowDamage = 3;
    public int bulletCount = 0;

    [Header("Attack Settings")]
    public int meleeDamage = 1;
    public float attackRange = 0.4f;
    public float attackPointOffset = 0.5f;
    public Transform attackPoint;
    public LayerMask enemyLayers;

    [Header("Key Settings")]
    public int keyCount = 0;

    [Header("Force Settings")]
    public float fallThresholdY = -5f;
    public float knockbackForce = 5f;

    private int facingDirection = 1;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private int jumpCount;
    private bool isGrounded;
    private float moveInput;
    private bool jumpRequest = false;
    private bool isDead = false;     // Giữ vì logic Die() vẫn cần
    private bool isHurt = false;     // Giữ vì TakeDamage() vẫn cần
    private Animator animator;
    private Vector2 checkpointPosition;

    [SerializeField]
    private GameOverManager gameOverManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        jumpCount = maxJumpCount;

        // Khởi tạo thanh HP (HealthUIManager)
        HealthUIManager.Instance.InitializeHealthBar(currentHealth);

        // Kiểm tra checkpoint
        if (PlayerPrefs.HasKey("HasCheckpoint") && PlayerPrefs.GetInt("HasCheckpoint") == 1)
        {
            float checkpointX = PlayerPrefs.GetFloat("CheckpointX");
            float checkpointY = PlayerPrefs.GetFloat("CheckpointY");
            checkpointPosition = new Vector2(checkpointX, checkpointY);

            transform.position = checkpointPosition;
            isDead = false;
            animator.SetBool("isDead", false);

            HealthUIManager.Instance.UpdateHealthUI(currentHealth);

            bulletCount = 0;
            UIUpdateLogic.Instance.UpdateArrowUI(bulletCount);
            UIUpdateLogic.Instance.UpdateKeyUI(keyCount);

            PlayerPrefs.DeleteKey("HasCheckpoint");
            PlayerPrefs.DeleteKey("CheckpointX");
            PlayerPrefs.DeleteKey("CheckpointY");
        }
        else
        {
            checkpointPosition = transform.position;
        }
    }

    void Update()
    {
        if (isDead) return;
        HandleInput();

        animator.SetFloat("moveSpeed", Mathf.Abs(moveInput));
        animator.SetBool("isGrounded", isGrounded);

        // isJumping
        if (!isGrounded && rb.velocity.y > 0)
            animator.SetBool("isJumping", true);
        else
            animator.SetBool("isJumping", false);

        UpdateAttackPointPosition();

        // Kiểm tra rơi khỏi map
        if (transform.position.y < fallThresholdY)
        {
            currentHealth = 0;
            HealthUIManager.Instance.UpdateHealthUI(currentHealth);
            Die();
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
    }

    // ==================================================
    // =========== CHECKPOINT & RESPAWN =================
    // ==================================================
    public void SetCheckpoint(Vector2 newCheckpointPosition)
    {
        checkpointPosition = newCheckpointPosition;
    }

    public void Respawn()
    {
        transform.position = checkpointPosition;
        rb.velocity = Vector2.zero;
        isDead = false;
        animator.SetBool("isDead", false);

        HealthUIManager.Instance.UpdateHealthUI(currentHealth);

        bulletCount = 0;
        UIUpdateLogic.Instance.UpdateArrowUI(bulletCount);
        UIUpdateLogic.Instance.UpdateKeyUI(keyCount);
    }

    // ==================================================
    // =========== NHẬN & SỬ DỤNG HEALTH =================
    // ==================================================
    public void AddHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        HealthUIManager.Instance.UpdateHealthUI(currentHealth);
    }

    // ==================================================
    // =========== INPUT & COMBO LOGIC ==================
    // ==================================================
    void HandleInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput > 0)
        {
            facingDirection = 1;
            spriteRenderer.flipX = false;
        }
        else if (moveInput < 0)
        {
            facingDirection = -1;
            spriteRenderer.flipX = true;
        }

        // Nhảy
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount > 0)
            jumpRequest = true;

        // Bắn (K)
        if (Input.GetKeyDown(KeyCode.K) && bulletCount > 0)
        {
            Shoot();
        }

        // Tấn công cận chiến (J)
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (isGrounded)
            {
                // Lấy stateInfo của layer 0
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

                // Tên clip: "meleeatk" (đòn 1), "meleeatk4" (đòn 2), "meleeatk2" (đòn 3)
                if (stateInfo.IsName("player_Meleeatk"))
                {
                    // Đang ở đòn 1 => chuyển sang đòn 2
                    // Xóa trigger của đòn 3 nếu còn sót (tránh nhảy đòn 3 bất ngờ)
                    animator.ResetTrigger("MeleeTrigger3");
                    animator.SetTrigger("MeleeTrigger2");
                }
                else if (stateInfo.IsName("player_Meleeatk4"))
                {
                    // Đang ở đòn 2 => chuyển sang đòn 3
                    animator.ResetTrigger("MeleeTrigger2");
                    animator.SetTrigger("MeleeTrigger3");
                }
                else if (stateInfo.IsName("player_Meleeatk2"))
                {
                    // Đang ở đòn 3 => nếu bấm J nữa thì quay lại đòn 1 (hoặc bỏ qua tùy ý)
                    animator.ResetTrigger("MeleeTrigger3");
                    animator.SetTrigger("MeleeTrigger1");
                }
                else
                {
                    // Nếu không đang ở clip tấn công nào => bắt đầu đòn 1
                    // Ở đây có thể xóa trigger 2 & 3, tránh trường hợp lỡ set
                    animator.ResetTrigger("MeleeTrigger2");
                    animator.ResetTrigger("MeleeTrigger3");
                    animator.SetTrigger("MeleeTrigger1");
                }
            }
            else
            {
                // Đòn đánh trên không
                ResetAttackTriggers();
                animator.SetTrigger("AirAttackTrigger");
            }
        }

    }

    void ResetAttackTriggers()
    {
        animator.ResetTrigger("MeleeTrigger1");
        animator.ResetTrigger("MeleeTrigger2");
        animator.ResetTrigger("MeleeTrigger3");
        animator.ResetTrigger("AirAttackTrigger");
        animator.ResetTrigger("BowAttackTrigger");
    }

    // ==================================================
    // =========== DI CHUYỂN ============================
    // ==================================================
    void HandleMovement()
    {
        if (isHurt) return;

        float targetSpeed = moveInput * maxSpeed;
        float speedDifference = targetSpeed - rb.velocity.x;
        float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement = speedDifference * accelerationRate;
        rb.AddForce(Vector2.right * movement);

        // Giới hạn tốc độ
        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
    }

    void HandleJump()
    {
        if (isHurt) return;
        CheckGrounded();

        if (jumpRequest)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            jumpCount--;
            jumpRequest = false;
        }
    }

    void CheckGrounded()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            groundCheck.position,
            groundCheckRadius,
            groundLayer | platformLayer
        );
        bool wasGrounded = isGrounded;
        isGrounded = false;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                isGrounded = true;
                break;
            }
        }

        animator.SetBool("isGrounded", isGrounded);

        if (isGrounded && !wasGrounded)
            jumpCount = maxJumpCount;
    }

    void UpdateAttackPointPosition()
    {
        Vector3 newPosition = transform.position;
        newPosition += Vector3.right * facingDirection * attackPointOffset;
        attackPoint.position = newPosition;
    }

    // ==================================================
    // =========== TAKE DAMAGE & DIE ====================
    // ==================================================
    public void TakeDamage(int damage)
    {
        if (isDead || isHurt) return;

        isHurt = true;
        currentHealth -= damage;
        HealthUIManager.Instance.UpdateHealthUI(currentHealth);

        if (currentHealth <= 0)
            Die();
        else
        {
            // Reset triggers để hủy combo/attack
            ResetAttackTriggers();
            animator.SetTrigger("HurtTrigger");
            ApplyKnockback();
            StartCoroutine(EndHurt());
        }
    }

    IEnumerator EndHurt()
    {
        yield return new WaitForSeconds(0.6f);
        animator.SetBool("isHurt", false);
        isHurt = false;
    }

    void ApplyKnockback()
    {
        int knockbackDirection = (facingDirection != 0) ? facingDirection * -1 : 1;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(knockbackDirection * knockbackForce, 0f), ForceMode2D.Impulse);
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        animator.SetTrigger("DieTrigger");

        if (gameOverManager != null)
            gameOverManager.ShowGameOver();

        this.enabled = false;
    }

    // ==================================================
    // =========== BẮN CUNG =============================
    // ==================================================
    void Shoot()
    {
        ResetAttackTriggers();
        animator.SetTrigger("BowAttackTrigger");
        bulletCount--;
        UIUpdateLogic.Instance.UpdateArrowUI(bulletCount);
    }

    // ==================================================
    // =========== GỌI TỪ ANIM EVENT ====================
    // ==================================================
    public void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayers
        );
        foreach (Collider2D enemy in hitEnemies)
        {
            IEnemy enemyScript = enemy.GetComponent<IEnemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(meleeDamage, "Melee", facingDirection);
            }
        }
    }

    public void AddBullets(int amount)
    {
        bulletCount += amount;
        UIUpdateLogic.Instance.UpdateArrowUI(bulletCount);
    }

    public void SpawnArrow()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        if (bulletComponent != null)
        {
            bulletComponent.SetDirection(new Vector2(facingDirection, 0));
            bulletComponent.SetDamage(arrowDamage);
        }
    }

    // ==================================================
    // =========== KEY & UI =============================
    // ==================================================
    public void AddKey()
    {
        keyCount++;
        UIUpdateLogic.Instance.UpdateKeyUI(keyCount);
    }

    public bool UseKey()
    {
        if (keyCount > 0)
        {
            keyCount--;
            UIUpdateLogic.Instance.UpdateKeyUI(keyCount);
            return true;
        }
        return false;
    }
}
