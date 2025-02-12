using UnityEngine;
using TMPro; // TextMeshPro
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
    public int maxHealth = 5;        // Máu tối đa của người chơi
    public int currentHealth;        // Máu hiện tại của người chơi

    [Header("Respawn Settings")]
    public Vector2 respawnPosition;  // Vị trí hồi sinh sau khi mất máu

    [Header("Jump Settings")]
    public float jumpForce = 15f;
    public int maxJumpCount = 2;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;       // Prefab của đạn
    public Transform firePoint;           // Vị trí bắn đạn
    public int arrowDamage = 3;
    public int bulletCount = 0;

    [Header("Attack Settings")]
    public int meleeDamage = 1;            // Sát thương cận chiến
    public float attackRange = 0.4f;       // Phạm vi tấn công cận chiến
    public float attackPointOffset = 0.5f;
    public Transform attackPoint;          // Điểm xuất phát của tấn công cận chiến
    public LayerMask enemyLayers;          // Layer của kẻ địch

    [Header("UI Settings")]
    public TextMeshProUGUI bulletText;    // Tham chiếu đến UI TextMeshPro hiển thị số đạn
    public TextMeshProUGUI healthText;    // Tham chiếu đến UI TextMeshPro hiển thị máu
    public TextMeshProUGUI keyText;

    [Header("Key Settings")]
    public int keyCount = 0;

    [Header("Force Settings")]
    public float fallThresholdY = -5f;
    public float knockbackForce = 5f;

    //private
    private int facingDirection = 1; // Hướng mà người chơi đang đối mặt (1: phải, -1: trái)
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private int jumpCount;
    private bool isGrounded;
    private Vector2 velocity = Vector2.zero;
    private float moveInput;
    private bool jumpRequest = false;
    private bool isDead = false;
    private bool isHurt = false;
    private Animator animator;
    private Vector2 checkpointPosition;
    [SerializeField]
    private GameOverManager gameOverManager; // Tham chiếu đến GameOverManager
    [HideInInspector]
    public bool isAttacking = false;
    [HideInInspector]
    public bool isBowAttacking = false;
    [HideInInspector]
    public bool isAirAttacking = false;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // Khởi tạo Animator
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        jumpCount = maxJumpCount;
        currentHealth = 3;
        UpdateBulletUI();
        UpdateHealthUI();
        UpdateKeyUI();

        if (PlayerPrefs.HasKey("HasCheckpoint") && PlayerPrefs.GetInt("HasCheckpoint") == 1)
        {
            float checkpointX = PlayerPrefs.GetFloat("CheckpointX");
            float checkpointY = PlayerPrefs.GetFloat("CheckpointY");
            checkpointPosition = new Vector2(checkpointX, checkpointY);

            // Di chuyển người chơi đến vị trí checkpoint
            transform.position = checkpointPosition;

            // Thiết lập lại các biến
            isDead = false;
            animator.SetBool("isDead", false);

            // Thiết lập lại máu và cập nhật UI
            currentHealth = maxHealth;
            UpdateHealthUI();

            // Thiết lập lại số lượng mũi tên
            bulletCount = 0;
            UpdateBulletUI();

            // Xóa checkpoint khỏi PlayerPrefs để tránh lặp lại
            PlayerPrefs.DeleteKey("HasCheckpoint");
            PlayerPrefs.DeleteKey("CheckpointX");
            PlayerPrefs.DeleteKey("CheckpointY");
        } // lay toa do checkpoint
        else
        {
            // Nếu không có checkpoint, thiết lập vị trí respawn mặc định
            checkpointPosition = transform.position;
        }
    }

    void Update()
    {
        if (isDead)
            return;

        HandleInput();

        // Cập nhật Animator parameters
        animator.SetFloat("moveSpeed", Mathf.Abs(moveInput));
        animator.SetBool("isGrounded", isGrounded);

        // Kiểm tra nếu người chơi đang nhảy
        if (!isGrounded && rb.velocity.y > 0)
        {
            animator.SetBool("isJumping", true);
        }
        else
        {
            animator.SetBool("isJumping", false);
        }

        UpdateAttackPointPosition();

        // Kiểm tra nếu người chơi bị rơi
        if (transform.position.y < fallThresholdY)
        {
            currentHealth = 0;
            UpdateHealthUI();
            Die();
        }
    }

    public void SetCheckpoint(Vector2 newCheckpointPosition)
    {
        checkpointPosition = newCheckpointPosition;
    }

    void UpdateAttackPointPosition()
    {
        Vector3 newPosition = transform.position;
        newPosition += Vector3.right * facingDirection * attackPointOffset;
        attackPoint.position = newPosition;
    } // Quản lý hướng của attack point

    public void TakeDamage(int damage)
    {
        if (isDead || isHurt)
            return;

        isHurt = true;

        currentHealth -= damage;

        // Cập nhật UI máu
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Đặt lại trạng thái tấn công
            isAttacking = false;
            isBowAttacking = false;
            isAirAttacking = false;

            // Đặt lại các Trigger khác
            animator.ResetTrigger("AttackTrigger");
            animator.ResetTrigger("ComboTrigger");
            animator.ResetTrigger("AirAttackTrigger");
            animator.ResetTrigger("BowAttackTrigger");

            // Kích hoạt Trigger Hurt
            animator.SetTrigger("HurtTrigger");

            // Thêm hiệu ứng giật lùi
            ApplyKnockback();

            // Bắt đầu Coroutine để kết thúc trạng thái Hurt
            StartCoroutine(EndHurt());
        }
    }

    IEnumerator EndHurt()
    {
        // Thời gian của animation Hurt (thay thế bằng thời gian thực tế)
        float hurtDuration = 0.6f; // Ví dụ: 0.6 giây

        yield return new WaitForSeconds(hurtDuration);

        animator.SetBool("isHurt", false);
        isHurt = false; // Đặt lại isHurt về false
    }

    void ApplyKnockback()
    {
        // Xác định hướng giật lùi dựa trên hướng tấn công
        int knockbackDirection = facingDirection * -1; // Ngược với hướng người chơi đang đối mặt

        // Áp dụng lực giật lùi
        rb.velocity = new Vector2(knockbackDirection * knockbackForce, rb.velocity.y);
    }

    public void Respawn()
    {
        // Di chuyển người chơi đến vị trí checkpoint
        transform.position = checkpointPosition;

        // Thiết lập lại vận tốc và trạng thái của người chơi
        rb.velocity = Vector2.zero;

        // Thiết lập lại các biến cần thiết
        isDead = false;
        animator.SetBool("isDead", false);

        // Thiết lập lại máu và cập nhật UI
        currentHealth = 3;
        UpdateHealthUI();

        // Thiết lập lại số lượng mũi tên
        bulletCount = 0;
        UpdateBulletUI();
    }

    public void AddHealth(int amount)
    {
        currentHealth += amount;

        // Giới hạn máu không vượt quá maxHealth
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        // Cập nhật UI máu
        UpdateHealthUI();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
    }

    void HandleInput()
    {
        // Đọc input di chuyển
        moveInput = Input.GetAxisRaw("Horizontal");

        // Lật sprite dựa trên hướng di chuyển
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

        // Xử lý nhảy
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount > 0)
        {
            jumpRequest = true;
        }

        // Xử lý bắn đạn
        if (Input.GetKeyDown(KeyCode.K) && bulletCount > 0)
        {
            Shoot();
        }

        // Xử lý tấn công cận chiến
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (isGrounded)
            {
                MeleeAttack();
            }
            else
            {
                AirAttack();
            }
        }
    }

    void HandleMovement()
    {
        // Nếu đang bị thương, không xử lý di chuyển
        if (isHurt)
        {
            return;
        }

        // Tính toán vận tốc mục tiêu
        float targetSpeed = moveInput * maxSpeed;
        float speedDifference = targetSpeed - rb.velocity.x;

        // Xác định lực cần thiết
        float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement = speedDifference * accelerationRate;

        // Áp dụng lực
        rb.AddForce(Vector2.right * movement);

        // Giới hạn tốc độ tối đa
        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
    }

    void HandleJump()
    {
        if (isHurt)
            return;

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
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayer);
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

        // Cập nhật Animator
        animator.SetBool("isGrounded", isGrounded);

        if (isGrounded && !wasGrounded)
        {
            jumpCount = maxJumpCount;
        }
    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log("Player has died.");

        // Kích hoạt Trigger Die
        animator.SetTrigger("DieTrigger");

        // Hiển thị màn hình Game Over
        if (gameOverManager != null)
        {
            gameOverManager.ShowGameOver();
        }

        // Vô hiệu hóa các hành động của người chơi
        this.enabled = false;
    }

    void MeleeAttack()
    {

        if (!isAttacking)
        {
            isAttacking = true;

            // Kích hoạt Trigger Attack
            animator.SetTrigger("AttackTrigger");
        }
        else
        {
            // Nếu đang ở MeleeAttack1, cho phép kích hoạt combo
            animator.SetTrigger("ComboTrigger");
        }
    }

    void AirAttack()
    {
        if (isGrounded)
            return; // Chỉ cho phép Air Attack khi đang ở trên không
        if (isAttacking || isBowAttacking || isAirAttacking)
            return;

            isAirAttacking = true;

            // Kích hoạt Trigger AirAttack
            animator.SetTrigger("AirAttackTrigger");
    }

    void Shoot()
    {
        if (isAttacking || isBowAttacking || bulletCount <= 0)
            return;

        isBowAttacking = true;

        // Kích hoạt Trigger BowAttack
        animator.SetTrigger("BowAttackTrigger");

        // Giảm số lượng đạn
        bulletCount--;

        // Cập nhật UI
        UpdateBulletUI();
    }

    public void DealDamage()
    {
        // Phát hiện các kẻ địch trong phạm vi tấn công
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Gây sát thương cho kẻ địch
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyMovement enemyScript = enemy.GetComponent<EnemyMovement>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(meleeDamage, "Melee", facingDirection);
            }
        }
    }

    public void AddBullets(int amount)
    {
        bulletCount += amount;

        // Cập nhật UI
        UpdateBulletUI();
    }

    public void SpawnArrow()
    {
        // Tạo đạn tại vị trí bắn
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Thiết lập hướng di chuyển và sát thương cho đạn
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(new Vector2(facingDirection, 0));
            bulletScript.SetDamage(arrowDamage);
        }
    }

    public void AddKey()
    {
        keyCount++;
        UpdateKeyUI();
    }

    public bool UseKey()
    {
        if (keyCount > 0)
        {
            keyCount--;
            UpdateKeyUI();
            return true;
        }
        return false;
    }

    void UpdateKeyUI()
    {
        if (keyText != null)
        {
            keyText.text = " " + keyCount;
        }
    }

    void UpdateBulletUI()
    {
        if (bulletText != null)
        {
            bulletText.text = "Arrow: " + bulletCount;
        }
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "HP: " + currentHealth;
        }
    }


}
