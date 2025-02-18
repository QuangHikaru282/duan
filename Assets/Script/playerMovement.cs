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
    public int maxHealth = 10;
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
    private Vector2 velocity = Vector2.zero;
    private float moveInput;
    private bool jumpRequest = false;
    private bool isDead = false;
    private bool isHurt = false;
    private Animator animator;
    private Vector2 checkpointPosition;
    [SerializeField]
    private GameOverManager gameOverManager;
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
        animator = GetComponent<Animator>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        jumpCount = maxJumpCount;
        currentHealth = maxHealth;

        // Cập nhật UI ban đầu thông qua UIUpdateLogic singleton
        UIUpdateLogic.Instance.UpdateArrowUI(bulletCount);
        UIUpdateLogic.Instance.UpdateHealthUI(currentHealth);
        UIUpdateLogic.Instance.UpdateKeyUI(keyCount);

        if (PlayerPrefs.HasKey("HasCheckpoint") && PlayerPrefs.GetInt("HasCheckpoint") == 1)
        {
            float checkpointX = PlayerPrefs.GetFloat("CheckpointX");
            float checkpointY = PlayerPrefs.GetFloat("CheckpointY");
            checkpointPosition = new Vector2(checkpointX, checkpointY);

            transform.position = checkpointPosition;
            isDead = false;
            animator.SetBool("isDead", false);

            currentHealth = maxHealth;
            UIUpdateLogic.Instance.UpdateHealthUI(currentHealth);

            bulletCount = 0;
            UIUpdateLogic.Instance.UpdateArrowUI(bulletCount);

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

        if (!isGrounded && rb.velocity.y > 0)
        {
            animator.SetBool("isJumping", true);
        }
        else
        {
            animator.SetBool("isJumping", false);
        }

        UpdateAttackPointPosition();

        if (transform.position.y < fallThresholdY)
        {
            currentHealth = 0;
            UIUpdateLogic.Instance.UpdateHealthUI(currentHealth);
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
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Player nhận damage!");
        if (isDead || isHurt)
            return;

        isHurt = true;
        currentHealth -= damage;
        UIUpdateLogic.Instance.UpdateHealthUI(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            isAttacking = false;
            isBowAttacking = false;
            isAirAttacking = false;

            animator.ResetTrigger("AttackTrigger");
            animator.ResetTrigger("ComboTrigger");
            animator.ResetTrigger("AirAttackTrigger");
            animator.ResetTrigger("BowAttackTrigger");

            animator.SetTrigger("HurtTrigger");
            ApplyKnockback();

            StartCoroutine(EndHurt());
        }
    }

    IEnumerator EndHurt()
    {
        float hurtDuration = 0.6f;
        yield return new WaitForSeconds(hurtDuration);

        animator.SetBool("isHurt", false);
        isHurt = false;
    }

    void ApplyKnockback()
    {
        // Xác định hướng đẩy: nếu player đang nhìn sang một hướng, ta đẩy ngược lại.
        int knockbackDirection = (facingDirection != 0) ? facingDirection * -1 : 1;

        // Reset vận tốc hiện tại để đảm bảo không cộng dồn vận tốc cũ.
        rb.velocity = Vector2.zero;

        // Đẩy player theo chiều ngang ngay lập tức.
        rb.AddForce(new Vector2(knockbackDirection * knockbackForce, 0f), ForceMode2D.Impulse);
    }

    public void Respawn()
    {
        transform.position = checkpointPosition;
        rb.velocity = Vector2.zero;
        isDead = false;
        animator.SetBool("isDead", false);

        currentHealth = maxHealth;
        UIUpdateLogic.Instance.UpdateHealthUI(currentHealth);

        bulletCount = 0;
        UIUpdateLogic.Instance.UpdateArrowUI(bulletCount);
    }

    public void AddHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        UIUpdateLogic.Instance.UpdateHealthUI(currentHealth);
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
    }

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

        if (Input.GetKeyDown(KeyCode.Space) && jumpCount > 0)
        {
            jumpRequest = true;
        }

        if (Input.GetKeyDown(KeyCode.K) && bulletCount > 0)
        {
            Shoot();
        }

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
        if (isHurt) return;
        float targetSpeed = moveInput * maxSpeed;
        float speedDifference = targetSpeed - rb.velocity.x;
        float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement = speedDifference * accelerationRate;

        rb.AddForce(Vector2.right * movement);

        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
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
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayer | platformLayer);
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
        {
            jumpCount = maxJumpCount;
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Player đã chết.");
        animator.SetTrigger("DieTrigger");

        if (gameOverManager != null)
        {
            gameOverManager.ShowGameOver();
        }
        this.enabled = false;
    }

    void MeleeAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("AttackTrigger");
        }
        else
        {
            animator.SetTrigger("ComboTrigger");
        }
    }

    void AirAttack()
    {
        if (isGrounded) return;
        if (isAttacking || isBowAttacking || isAirAttacking) return;

        isAirAttacking = true;
        animator.SetTrigger("AirAttackTrigger");
    }

    void Shoot()
    {
        if (isAttacking || isBowAttacking || bulletCount <= 0)
            return;

        isBowAttacking = true;
        animator.SetTrigger("BowAttackTrigger");

        bulletCount--;
        UIUpdateLogic.Instance.UpdateArrowUI(bulletCount);
    }

    public void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

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
    void TemporarilyIgnorePlayerCollision(bool ignore)
    {
        Collider2D goblinCollider = GetComponent<Collider2D>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null && goblinCollider != null)
        {
            Collider2D playerCollider = playerObj.GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                Physics2D.IgnoreCollision(goblinCollider, playerCollider, ignore);
            }
        }
    }
}
