using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GoblinNormal : MonoBehaviour, IEnemy
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;
    public Slider healthBar;

    [Header("Patrol Settings")]
    public bool enablePatrol = true;
    public float speed = 2f;
    public float patrolDistance = 5f;
    private Vector2 pointA, pointB;
    private Vector2 targetPosition;

    [Header("Chase Settings")]
    public bool isChasing = false;
    public float chaseSpeed = 3f;
    public float detectRange = 5f;
    private Transform player;

    [Header("Ground & Wall Detection")]
    public Transform groundDetector;
    public float groundDetectDistance = 0.2f;
    public float wallDetectDistance = 0.2f;
    public LayerMask groundLayer;

    [Header("Attack Settings")]
    public Transform atkPoint;
    public float attackRange = 0.5f;
    public LayerMask playerLayer;
    public int goblinDamage = 2;
    public float attackCooldown = 1.5f;
    private float lastAttackTime = -999f;

    private int facingDirection = 1;
    private bool isDead = false;
    private bool isHurt = false;
    private bool isAttacking = false;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [Header("Misc Settings")]
    public float killBelowY = -20f;  // nếu rơi xuống dưới này thì chết

    [Header("Prefabs & Effects")]
    public GameObject meleeEffect, rangeEffect;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        // Thiết lập pointA, pointB đối xứng quanh vị trí ban đầu
        Vector2 startPos = transform.position;
        pointA = new Vector2(startPos.x - patrolDistance, startPos.y);
        pointB = new Vector2(startPos.x + patrolDistance, startPos.y);
        targetPosition = pointB;

        facingDirection = 1;
        spriteRenderer.flipX = false;
        if (atkPoint != null)
        {
            atkPoint.localPosition = new Vector3(0.3f, atkPoint.localPosition.y, 0f);
        }
    }

    void FixedUpdate()
    {
        if (isDead || isHurt) return;

        // Rơi khỏi map
        if (transform.position.y < killBelowY)
        {
            Die();
            return;
        }

        if (isChasing)
            ChaseMovement();
        else
        {
            CheckForFlip();
            PatrolMovement();
        }
    }

    // ------------------- PATROL -------------------
    void PatrolMovement()
    {
        Vector2 currentPos = rb.position;
        Vector2 dir = (targetPosition - currentPos).normalized;
        float moveX = dir.x * speed;
        rb.velocity = new Vector2(moveX, rb.velocity.y);

        animator.SetFloat("moveSpeed", Mathf.Abs(moveX));

        if (moveX > 0.11f)
            SetFacingDirection(1);
        else if (moveX < -0.11f)
            SetFacingDirection(-1);

        if (Vector2.Distance(currentPos, targetPosition) < 0.1f)
        {
            targetPosition = (targetPosition == pointA) ? pointB : pointA;
        }
    }

    // ------------------- CHASE -------------------
    void ChaseMovement()
    {
        if (player == null)
        {
            StopChase();
            return;
        }

        float playerX = player.position.x;
        float goblinX = transform.position.x;
        float moveX = (playerX < goblinX) ? -chaseSpeed : chaseSpeed;
        rb.velocity = new Vector2(moveX, rb.velocity.y);
        animator.SetFloat("moveSpeed", Mathf.Abs(moveX));

        SetFacingDirection((moveX > 0f) ? 1 : -1);

        if (Mathf.Abs(playerX - goblinX) < 1.0f)
            GoblinAttack();
    }

    // ------------------- CHECK FLIP -------------------
    void CheckForFlip()
    {
        if (groundDetector == null) return;

        // Xác định vị trí của groundDetector
        Vector2 detectorPos = groundDetector.position;
        // Thiết lập một offset nhỏ theo trục X (điều chỉnh giá trị này cho phù hợp với sprite của bạn)
        float horizontalOffset = 0.2f;

        // Tạo 2 điểm bắt tia: bên trái và bên phải của groundDetector
        Vector2 leftRayOrigin = detectorPos + Vector2.left * horizontalOffset;
        Vector2 rightRayOrigin = detectorPos + Vector2.right * horizontalOffset;

        // Bắn tia xuống (theo hướng Vector2.down)
        RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.down, groundDetectDistance, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.down, groundDetectDistance, groundLayer);

        // Vẽ Debug rays để kiểm tra trong Scene view
        Debug.DrawRay(leftRayOrigin, Vector2.down * groundDetectDistance, Color.yellow);
        Debug.DrawRay(rightRayOrigin, Vector2.down * groundDetectDistance, Color.yellow);

        // Nếu một trong hai tia không phát hiện ground, thì đổi hướng
        if (leftHit.collider == null || rightHit.collider == null)
        {
            ReverseDirection();
        }

        // Bạn có thể bổ sung kiểm tra tia ray theo hướng ngang nếu cần (ví dụ để phát hiện tường)
        // (Phần kiểm tra này giữ nguyên nếu bạn muốn)
        Vector2 sideDir = new Vector2(facingDirection, 0f);
        RaycastHit2D wallHit = Physics2D.Raycast(detectorPos, sideDir, wallDetectDistance, groundLayer);
        Debug.DrawRay(detectorPos, sideDir * wallDetectDistance, Color.yellow);
        if (wallHit.collider != null)
        {
            if (wallHit.collider.CompareTag("Ground") || wallHit.collider.CompareTag("MovingPlaform"))
            {
                ReverseDirection();
            }
        }
    }

    void ReverseDirection()
    {
        SetFacingDirection(-facingDirection);
        if (!isChasing)
        {
            targetPosition = (targetPosition == pointA) ? pointB : pointA;
        }
    }

    void SetFacingDirection(int dir)
    {
        Debug.Log("Set face direction called!");
        if (facingDirection == dir) return;

        facingDirection = dir;
        spriteRenderer.flipX = (dir < 0);

        if (atkPoint != null)
        {
            atkPoint.localPosition = new Vector3(0.3f * dir, atkPoint.localPosition.y, 0f);
        }
    }

    // ------------------- CHASE CONTROL -------------------
    public void StartChase(Transform p)
    {
        if (isDead) return;
        isChasing = true;
        player = p;
        rb.velocity = Vector2.zero;
    }

    public void StopChase()
    {
        if (isDead) return;
        isChasing = false;
        player = null;
    }

    // ------------------- ATTACK -------------------
    void GoblinAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;
        if (isAttacking) return;

        isAttacking = true;
        lastAttackTime = Time.time;
        rb.velocity = new Vector2(0f, rb.velocity.y);

        // Tạm thời bỏ qua va chạm với Player khi attack
        TemporarilyIgnorePlayerCollision(true);

        animator.SetTrigger("AttackTrigger");
    }

    public void GoblinDealDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(atkPoint.position, attackRange, playerLayer);
        foreach (Collider2D c in hits)
        {
            playerScript ps = c.GetComponent<playerScript>();
            if (ps != null)
                ps.TakeDamage(goblinDamage);
        }
    }

    public void ResetAttack()
    {
        isAttacking = false;
        // Khôi phục lại va chạm giữa goblin và player
        TemporarilyIgnorePlayerCollision(false);
    }

    // ------------------- TAKE DAMAGE (IEnemy) -------------------
    public void TakeDamage(int dmg, string dmgType, int attackDir)
    {
        if (isDead) return;

        if (isAttacking)
        {
            isAttacking = false;
            animator.ResetTrigger("AttackTrigger");
        }

        currentHealth -= dmg;
        if (healthBar != null)
            healthBar.value = currentHealth;

        ShowHitEffect(dmgType, attackDir);
        animator.SetTrigger("HurtTrigger");
        isHurt = true;

        if (currentHealth <= 0)
            Die();
        else
            StartCoroutine(EndHurt(0.5f));
    }

    void ShowHitEffect(string dmgType, int dir)
    {
        GameObject effectPrefab = null;
        if (dmgType == "Melee" && meleeEffect != null)
            effectPrefab = meleeEffect;
        else if (dmgType == "Range" && rangeEffect != null)
            effectPrefab = rangeEffect;

        if (effectPrefab != null)
        {
            Vector3 offset = new Vector3(0.3f * dir, 0f, 0f);
            Vector3 effectPos = transform.position + offset;
            GameObject effect = Instantiate(effectPrefab, effectPos, Quaternion.identity);

            Vector3 scale = effect.transform.localScale;
            scale.x = (dir < 0) ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            effect.transform.localScale = scale;
        }
    }

    IEnumerator EndHurt(float dur)
    {
        yield return new WaitForSeconds(dur);
        isHurt = false;
    }

    // ------------------- DIE -------------------
    void Die()
    {
        if (isDead) return;
        isDead = true;
        animator.SetBool("isDead", true);
        if (healthBar != null)
            Destroy(healthBar.gameObject);
        rb.velocity = new Vector2(-facingDirection * 4f, 3f);
        StartCoroutine(DieVanish(0.5f));
    }

    IEnumerator DieVanish(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    // ------------------- TRAP -------------------
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isDead && other.CompareTag("Trap"))
            Die();
    }

    // ------------------- TemporarilyIgnorePlayerCollision -------------------
    // Tạm thời bỏ qua va chạm giữa collider của goblin và collider của Player
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

    // ------------------- GIZMOS -------------------
    void OnDrawGizmosSelected()
    {
        if (groundDetector != null)
        {
            Gizmos.color = Color.yellow;
            Vector2 sideDir = new Vector2(facingDirection, 0f);
            Gizmos.DrawRay(groundDetector.position, Vector2.down * groundDetectDistance);
            Gizmos.DrawRay(groundDetector.position, sideDir * wallDetectDistance);
        }
        if (atkPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(atkPoint.position, attackRange);
        }
    }
}
