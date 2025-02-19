using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Pathfinding;

public class BatNormal : MonoBehaviour, IEnemy
{
    [Header("Health Settings")]
    private int currentHealth;
    public Slider healthBar;

    [Header("Particle Effects")]
    public GameObject meleeEffect;
    public GameObject rangeEffect;

    [Header("Patrol Settings")]
    public float speed = 2f;
    // [NEW] Transform của object con chỉ tọa độ B
    public Transform pointBTransform;

    private Vector2 pointA;            // Vị trí gốc (Point A)
    private Vector2 currentTarget;     // Mục tiêu di chuyển (có thể là pointA hoặc pointB)

    private Rigidbody2D rb;
    private Collider2D enemyCollider;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isDead = false;
    private bool isHurt = false;
    private bool isAttacking = false;
    private int facingDirection = -1; // Mặc định quái nhìn trái

    [Header("Platform Unlock Settings")]
    public CustomMovingPlatform linkedPlatform;
    public NotiUIScript notificationUI;
    public string platformUnlockedMessage = "Platform has been unlocked!";

    [Header("Chase (A* Pathfinding)")]
    public bool isChasing = false;
    public AIPath aiPath;
    public AIDestinationSetter aiDestination;

    [Header("Bat Attack Settings")]
    public Transform atk_point;
    public float atkRange = 0.5f;
    public LayerMask playerLayer;
    public int batDamage = 1;
    public float attackCooldown = 1.5f;
    private float lastAttackTime = -999f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // [NEW] Nếu pointBTransform đang là con của Bat, ta detach nó
        if (pointBTransform != null)
        {
            Vector3 wPos = pointBTransform.position;
            pointBTransform.SetParent(null);    // Tách khỏi Bat
            pointBTransform.position = wPos;    // Giữ nguyên vị trí thế giới
        }

        // pointA = vị trí ban đầu (transform.position)
        pointA = transform.position;

        // Nếu pointBTransform != null => currentTarget = pointBTransform.position
        // Không thì quái đứng yên tại A
        if (pointBTransform != null)
        {
            currentTarget = pointBTransform.position;
        }
        else
        {
            currentTarget = pointA;
        }

        // Mặc định quái nhìn trái => flipX = true
        facingDirection = -1;
        spriteRenderer.flipX = true;

        if (healthBar != null)
        {
            currentHealth = (int)healthBar.maxValue;
            healthBar.value = healthBar.maxValue;
        }
        else
        {
            currentHealth = 5;
        }

        if (notificationUI != null) notificationUI.gameObject.SetActive(false);

        if (aiPath != null)
        {
            aiPath.enabled = false;
            aiPath.canMove = false;
        }

        // atk_point ban đầu (nếu quái nhìn trái => localPosition.x âm)
        if (atk_point != null)
        {
            atk_point.localPosition = new Vector3(-0.3f, 0, 0);
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;
        if (isHurt) return;

        if (isChasing)
        {
            FlipSpriteByAiPath();
            float distToPlayer = 999f;
            if (aiDestination != null && aiDestination.target != null)
            {
                distToPlayer = Vector2.Distance(transform.position, aiDestination.target.position);
            }
            if (distToPlayer < 1.0f)
            {
                BatAttack();
            }
        }
        else
        {
            // Bay tuần tra
            PatrolMovement();
        }
    }

    // ----------------------------- 
    // CƠ CHẾ TUẦN TRA
    // -----------------------------
    void PatrolMovement()
    {
        Vector2 currentPos = rb.position;
        Vector2 dir = (currentTarget - currentPos).normalized;
        float distanceThisFrame = speed * Time.fixedDeltaTime;

        Vector2 newPosition = currentPos + dir * distanceThisFrame;
        rb.MovePosition(newPosition);

        // Lật sprite
        if (dir.x > 0.01f)
        {
            spriteRenderer.flipX = false;
            facingDirection = 1;
            if (atk_point != null) atk_point.localPosition = new Vector3(+0.3f, 0, 0);
        }
        else if (dir.x < -0.01f)
        {
            spriteRenderer.flipX = true;
            facingDirection = -1;
            if (atk_point != null) atk_point.localPosition = new Vector3(-0.3f, 0, 0);
        }

        // Khi gần tới mục tiêu => đổi mục tiêu
        if (Vector2.Distance(newPosition, currentTarget) < 0.1f)
        {
            if (pointBTransform != null && (Vector2)currentTarget == (Vector2)pointBTransform.position)
            {
                // Nếu đang ở B => quay về A
                currentTarget = pointA;
            }
            else
            {
                // Nếu đang ở A => sang B (nếu có)
                if (pointBTransform != null)
                    currentTarget = pointBTransform.position;
            }
        }
    }

    // -----------------------------
    // PHẦN TAKE DAMAGE
    // -----------------------------
    public void TakeDamage(int damage, string damageType, int attackDirection)
    {
        if (isDead) return;

        if (isAttacking)
        {
            isAttacking = false;
            animator.ResetTrigger("AttackTrigger");
            if (aiPath != null) aiPath.canMove = true;
        }

        currentHealth -= damage;
        if (healthBar != null) healthBar.value = currentHealth;

        ShowHitEffect(damageType, attackDirection);

        if (animator != null) animator.SetTrigger("HurtTrigger");
        isHurt = true;

        if (currentHealth <= 0) Die();
        else StartCoroutine(EndHurt(0.5f));
    }

    IEnumerator EndHurt(float duration)
    {
        yield return new WaitForSeconds(duration);
        isHurt = false;
    }

    void ShowHitEffect(string damageType, int attackDirection)
    {
        GameObject effectPrefab = null;
        if (damageType == "Melee" && meleeEffect != null) effectPrefab = meleeEffect;
        else if (damageType == "Range" && rangeEffect != null) effectPrefab = rangeEffect;

        if (effectPrefab != null)
        {
            Vector3 effectPos = transform.position + new Vector3(0.3f * attackDirection, 0f, 0f);
            GameObject effect = Instantiate(effectPrefab, effectPos, Quaternion.identity);
            Vector3 scale = effect.transform.localScale;
            if (attackDirection < 0) scale.x = -Mathf.Abs(scale.x);
            else scale.x = Mathf.Abs(scale.x);
            effect.transform.localScale = scale;
        }
    }

    // -----------------------------
    // DIE
    // -----------------------------
    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (animator != null)
        {
            animator.SetBool("isDead", true);
        }

        // Mở khoá platform (nếu có)
        if (linkedPlatform != null)
        {
            linkedPlatform.UnlockPlatform();
        }

        // Bật vật lý rơi
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        if (aiPath != null)
        {
            aiPath.canMove = false;
            aiPath.enabled = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {

        if (isDead && col.collider.CompareTag("Ground"))
        {
            StartCoroutine(DestroyAfter(0.5f));
        }

        ItemDropper dropper = GetComponent<ItemDropper>();
        if (dropper != null)
        {
            dropper.DropItems();
        }
    }


    IEnumerator DestroyAfter(float delay)
    {

        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    // -----------------------------
    // CHASE
    // -----------------------------
    public void StartChase(Transform playerTransform)
    {
        isChasing = true;
        if (aiPath != null)
        {
            aiPath.enabled = true;
            aiPath.canMove = true;
        }
        if (aiDestination != null)
        {
            aiDestination.target = playerTransform;
        }
    }

    public void StopChase()
    {
        Debug.Log("StopChase dc goi!");
        isChasing = false;
        if (aiPath != null)
        {
            aiPath.canMove = false;
            aiPath.enabled = false;
        }
        if (aiDestination != null)
        {
            aiDestination.target = null;
        }

        // Quay lại pointA
        currentTarget = pointA;
    }

    



    // -----------------------------
    // AI PATH FLIP
    // -----------------------------
    void FlipSpriteByAiPath()
    {
        if (aiPath == null) return;
        Vector2 velocity = aiPath.desiredVelocity;
        if (velocity.x > 0.01f)
        {
            spriteRenderer.flipX = false;
            facingDirection = 1;
            if (atk_point != null) atk_point.localPosition = new Vector3(+0.3f, 0, 0);
        }
        else if (velocity.x < -0.01f)
        {
            spriteRenderer.flipX = true;
            facingDirection = -1;
            if (atk_point != null) atk_point.localPosition = new Vector3(-0.3f, 0, 0);
        }
    }

    // -----------------------------
    // ATTACK
    // -----------------------------
    public void BatAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;
        if (isAttacking) return;

        isAttacking = true;
        lastAttackTime = Time.time;

        if (aiPath != null) aiPath.canMove = false;
        if (animator != null) animator.SetTrigger("AttackTrigger");
    }

    public void BatDealDamage()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(atk_point.position, atkRange, playerLayer);
        foreach (Collider2D c in hitPlayers)
        {
            playerScript ps = c.GetComponent<playerScript>();
            if (ps != null)
            {
                ps.TakeDamage(batDamage);
            }
        }
    }

    public void ResetAttack()
    {
        isAttacking = false;
        if (aiPath != null) aiPath.canMove = true;
    }

    // -----------------------------
    // GIZMOS
    // -----------------------------
    void OnDrawGizmosSelected()
    {
        if (atk_point != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(atk_point.position, atkRange);
        }
    }
}
