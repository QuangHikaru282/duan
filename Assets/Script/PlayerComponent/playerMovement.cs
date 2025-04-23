using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    private bool isJumping = false;
    public float jumpForce = 15f;
    public int maxJumpCount = 1;
    public bool UnlockMaxJumpCount =false;

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

    [Header("Skill HomingBullet")]
    public GameObject homingBulletPrefab;   
    public Transform skillShotPoint;        
    public string castTrigger = "CastTrigger";  
    public int homingBulletCost = 20;       

    [Header("Flamethrower")]
    public GameObject flameThrowerPrefab;  
    private Vector2 originalColliderOffset;
    private GameObject flameInstance = null;
    bool isFlamethrowerActive = false;
    private bool isCastLooping = false;
    public float manaCostPerSec = 15f;
    private float manaAccum = 0f;

    [Header("Dash Settings")]
    public float dashForce = 5f;
    public float dashCooldown = 0.25f;
    private bool isDashing = false;
    private float dashTimer = 0f;
    public int airDashCount = 1; // Số lần nhảy trên không

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
    private bool isDead = false;
    private bool isHurt = false;
    [HideInInspector] public bool isFrozen = false;
    private Animator animator;
    private Vector2 checkpointPosition;
    public bool cantBeDamaged = false;

    [SerializeField]
    private GameOverManager gameOverManager;

    [Header("Skill Manager Settings")]
    [SerializeField]
    private SkillUnlockManager skillUnlockManager;
    [Header("UI Settings")]
    [SerializeField]
    private GameObject Mana;
    [SerializeField]
    private GameObject skill_E;
    [SerializeField]
    private GameObject skill_Q;

    // ------------------------ Group 1: Start, Update, FixedUpdate ------------------------
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        jumpCount = maxJumpCount;

        HealthUIManager.Instance.InitializeHealthBar(currentHealth);

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
        if (GameState.isDialoguePlaying)
        {
            moveInput = 0f; // Reset input để không bị "dính" lại
            rb.velocity = new Vector2(0, rb.velocity.y); // Dừng ngang
            animator.SetFloat("moveSpeed", 0); // Nếu dùng animation
            return;
        }

        if (isDead) return;
        HandleInput();

        if (dashTimer > 0f)
        {
            dashTimer -= Time.deltaTime;
        }

        animator.SetFloat("moveSpeed", Mathf.Abs(moveInput));
        animator.SetBool("isGrounded", isGrounded);

        if (!isGrounded && rb.velocity.y > 0)
            animator.SetBool("isJumping", true);
        else
            animator.SetBool("isJumping", false);

        UpdateAttackPointPosition();

        if (flameInstance != null && skillShotPoint != null)
        {
            flameInstance.transform.position = skillShotPoint.position;

            bool flip = (skillShotPoint.localPosition.x < 0);
            SpriteRenderer flameSprite = flameInstance.GetComponent<SpriteRenderer>();
            if (flameSprite != null)
            {
                flameSprite.flipX = flip;
            }

            BoxCollider2D flameCollider = flameInstance.GetComponent<BoxCollider2D>();
            if (flameCollider != null)
            {
                flameCollider.offset = new Vector2(flip ? -originalColliderOffset.x : originalColliderOffset.x, originalColliderOffset.y);
            }
        }

        if (transform.position.y < fallThresholdY)
        {
            currentHealth = 0;
            HealthUIManager.Instance.UpdateHealthUI(currentHealth);
            Die();
        }
    }

    void FixedUpdate()
    {
        if (GameState.isDialoguePlaying)
        {
            return;
        }
        // Nếu đang dash, bỏ qua xử lý di chuyển thông thường
        if (!isDashing)
            HandleMovement();
        HandleJump();
    }

    // ------------------------ Group 2: Hàm liên quan đến xử lý Input ------------------------
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
        if (!UnlockMaxJumpCount && skillUnlockManager.isDoubleJumpUnlocked)
        {
            maxJumpCount = 2;
            jumpCount = maxJumpCount;
            UnlockMaxJumpCount =true;
        }
        // Nhận input nhảy
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount > 0)
        {
                jumpRequest = true;
                isJumping = true;
        }

        // Jump Cutting: nếu thả phím Space khi đang bay lên thì giảm lực nhảy
        if (Input.GetKeyUp(KeyCode.Space) && isJumping && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f); // cắt lực nhảy
            isJumping = false;
        }

        // Bắn tên (K)
        if (Input.GetKeyDown(KeyCode.K) && bulletCount > 0)
        {
            Shoot();
        }

        // Melee (J)
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (isGrounded)
            {
                rb.velocity = Vector2.zero;
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName("player_Meleeatk"))
                {
                    animator.ResetTrigger("MeleeTrigger3");
                    animator.SetTrigger("MeleeTrigger2");
                }
                else if (stateInfo.IsName("player_Meleeatk4"))
                {
                    animator.ResetTrigger("MeleeTrigger2");
                    animator.SetTrigger("MeleeTrigger3");
                }
                else if (stateInfo.IsName("player_Meleeatk2"))
                {
                    animator.ResetTrigger("MeleeTrigger3");
                    animator.SetTrigger("MeleeTrigger1");
                }
                else
                {
                    animator.ResetTrigger("MeleeTrigger2");
                    animator.ResetTrigger("MeleeTrigger3");
                    animator.SetTrigger("MeleeTrigger1");
                }
            }
            else
            {
                ResetAttackTriggers();
                animator.SetTrigger("AirAttackTrigger");
            }
        }
        if (skillUnlockManager.isSkillEUnlocked || skillUnlockManager.isSkillQUnlocked)
        {
            //Mana.SetActive(true);
            if (skillUnlockManager.isSkillEUnlocked)
            {
                skill_E.SetActive(true);
            }
            if (skillUnlockManager.isSkillQUnlocked)
            {
                skill_Q.SetActive(true);
            }
        }
            // Dash (L)
            if (Input.GetKeyDown(KeyCode.L) && skillUnlockManager.isSkillLUnlocked)
        {
            if (isFrozen || isHurt || isFlamethrowerActive) return;

            if (isGrounded && dashTimer <= 0f && !isDashing)
            {
                StartCoroutine(PerformDash());
            }
            else if (!isGrounded && airDashCount > 0 && dashTimer <= 0f && !isDashing)
            {
                StartCoroutine(PerformDash());
                airDashCount--;
            }
        }

        // HomingBullet (E)
        if (Input.GetKeyDown(KeyCode.E) && skillUnlockManager.isSkillEUnlocked)
        {
            if (SkillManager.Instance.UseMana(homingBulletCost))
            {
                animator.SetTrigger(castTrigger);
            }
        }

        // Flamethrower (Q)
        if (Input.GetKeyDown(KeyCode.Q) && skillUnlockManager.isSkillQUnlocked)
        {
            isFlamethrowerActive = true;
            animator.SetBool("isCastLooping", true);
        }

        if (isFlamethrowerActive && Input.GetKey(KeyCode.Q) && skillUnlockManager.isSkillQUnlocked)
        {
            if (!ConsumeFlameMana())
            {
                TurnOffFlamethrower();
            }
        }

        if (Input.GetKeyUp(KeyCode.Q) && skillUnlockManager.isSkillQUnlocked)
        {
            TurnOffFlamethrower();
        }
    }

    IEnumerator PerformDash()
    {
        cantBeDamaged = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        ResetAttackTriggers();
        isDashing = true;
        animator.SetBool("isDashing", true);

        float elapsed = 0f;
        float duration = 0.2f;
        float dashDistance = dashForce;
        Vector2 start = rb.position;
        Vector2 end = start + new Vector2(facingDirection * dashDistance, 0f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            Vector2 nextPos = Vector2.Lerp(start, end, t);
            rb.MovePosition(nextPos); // tương tác vật lý an toàn
            yield return new WaitForFixedUpdate(); // đảm bảo sync với physics engine
        }

        animator.SetBool("isDashing", false);
        isDashing = false;
        rb.gravityScale = originalGravity;
        dashTimer = dashCooldown;
    }



    public void ResetCantBeDamaged()
    {
        cantBeDamaged = false;
    }


    // ------------------------ Group 3: Các hàm liên quan đến tấn công (skill, shoot, dealdamage) ------------------------

    void ResetAttackTriggers()
    {
        animator.ResetTrigger("MeleeTrigger1");
        animator.ResetTrigger("MeleeTrigger2");
        animator.ResetTrigger("MeleeTrigger3");
        animator.ResetTrigger("AirAttackTrigger");
        animator.ResetTrigger("BowAttackTrigger");
    }

    void Shoot()
    {
        ResetAttackTriggers();
        animator.SetTrigger("BowAttackTrigger");
        bulletCount--;
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

    public void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayers
        );

        HashSet<IEnemy> damagedEnemies = new HashSet<IEnemy>();

        foreach (Collider2D col in hitEnemies)
        {
            IEnemy enemyScript = col.GetComponentInParent<IEnemy>();
            if (enemyScript != null && !damagedEnemies.Contains(enemyScript))
            {
                damagedEnemies.Add(enemyScript);
                enemyScript.TakeDamage(meleeDamage, "Melee", facingDirection);
                HitStopManager.Instance.TriggerHitStop(0.1f);
            }

        }
    }

    public void OnCastSpawnHomingBullet()
    {
        if (homingBulletPrefab == null || skillShotPoint == null) return;
        Instantiate(homingBulletPrefab, skillShotPoint.position, skillShotPoint.rotation);
    }

    public void OnCastSpawnFlamethrower()
    {
        if (flameThrowerPrefab == null) return;
        if (flameInstance != null) return;

        flameInstance = Instantiate(flameThrowerPrefab, skillShotPoint.position, Quaternion.identity);

        BoxCollider2D flameCollider = flameInstance.GetComponent<BoxCollider2D>();
        if (flameCollider != null)
        {
            originalColliderOffset = flameCollider.offset;
        }
    }

    bool ConsumeFlameMana()
    {
        float dt = Time.deltaTime;
        float manaToAdd = manaCostPerSec * dt;
        manaAccum += manaToAdd; // tích lũy dần

        int manaToConsume = Mathf.FloorToInt(manaAccum);

        if (manaToConsume > 0)
        {
            bool success = SkillManager.Instance.UseMana(manaToConsume);
            if (!success)
            {
                return false;
            }
            manaAccum -= manaToConsume;
        }

        return true;
    }

    void TurnOffFlamethrower()
    {
        animator.SetBool("isCastLooping", false);
        isFlamethrowerActive = false;
        if (flameInstance != null)
        {
            Destroy(flameInstance);
            flameInstance = null;
        }
    }

    // ------------------------ Group 4: Các hàm nhận sát thương, die và respawn ------------------------
    public void TakeDamage(int damage)
    {
        if (cantBeDamaged|| isDead || isHurt) return;

        isHurt = true;
        currentHealth -= damage;
        HealthUIManager.Instance.UpdateHealthUI(currentHealth);
        //HitStopManager.Instance.TriggerHitStop(0.25f); // Ví dụ: stop trong 0.08 giây


        if (currentHealth <= 0)
            Die();
        else
        {
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

    public void Respawn()
    {
        transform.position = checkpointPosition;
        rb.velocity = Vector2.zero;
        isDead = false;
        animator.SetBool("isDead", false);

        HealthUIManager.Instance.UpdateHealthUI(currentHealth);
        UIUpdateLogic.Instance.UpdateArrowUI(bulletCount);
    }

    public void SetCheckpoint(Vector2 newCheckpointPosition)
    {
        checkpointPosition = newCheckpointPosition;
    }

    // ------------------------ Group 5: Các hàm linh tinh ------------------------
    void HandleMovement()
    {
        if (isHurt || isFlamethrowerActive || isDashing) return;

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
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumping = true;
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
        {
            jumpCount = maxJumpCount;
            airDashCount = 1;
        }
    }

    void UpdateAttackPointPosition()
    {
        float sign = (facingDirection > 0) ? 1f : -1f;

        if (attackPoint != null)
        {
            Vector3 atkLocalPos = attackPoint.localPosition;
            atkLocalPos.x = Mathf.Abs(atkLocalPos.x) * sign;
            attackPoint.localPosition = atkLocalPos;
        }

        if (firePoint != null)
        {
            Vector3 fireLocalPos = firePoint.localPosition;
            fireLocalPos.x = Mathf.Abs(fireLocalPos.x) * sign;
            firePoint.localPosition = fireLocalPos;
        }

        if (skillShotPoint != null)
        {
            Vector3 skillLocalPos = skillShotPoint.localPosition;
            skillLocalPos.x = Mathf.Abs(skillLocalPos.x) * sign;
            skillShotPoint.localPosition = skillLocalPos;
        }
    }

    public void AddHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        HealthUIManager.Instance.UpdateHealthUI(currentHealth);
    }

    public void AddBullets(int amount)
    {
        bulletCount += amount;
        UIUpdateLogic.Instance.UpdateArrowUI(bulletCount);
    }
}