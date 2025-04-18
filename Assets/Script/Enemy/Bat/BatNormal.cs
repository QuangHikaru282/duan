﻿using UnityEngine;
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
    public Transform pointBTransform;

    private Vector2 pointA;            // Vị trí gốc (Point A)
    private Vector2 currentTarget;     // Mục tiêu di chuyển (có thể là pointA hoặc pointB)

    private Transform chaseTarget;      // Target trung gian cho chase
    private Transform chasePlayerTarget;  // Lưu lại tham chiếu đến player mà bat đang theo dõi
    private Vector2 chaseOffset;          // Offset ngẫu nhiên
    private Vector2 offset;

    private Rigidbody2D rb;
    private Collider2D enemyCollider;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isDead = false;
    private bool isHurt = false;
    private bool isAttacking = false;
    private int facingDirection = -1; // Mặc định quái nhìn trái

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

        pointA = transform.position;

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

        // Nếu bat đang chase, cập nhật vị trí của chaseTarget
        if (isChasing && chaseTarget != null && chasePlayerTarget != null)
        {
            
            chaseTarget.position = new Vector3(chasePlayerTarget.position.x + offset.x, chasePlayerTarget.position.y, chaseTarget.position.z);

        }

        if (isChasing)
        {
            FlipSpriteByAiPath();
            float distToPlayer = Vector2.Distance(transform.position, chasePlayerTarget.position);
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

        if (animator != null && damageType == "DOT")
        {
            isHurt = true;
            animator.SetTrigger("DotHurtTrigger");
            // Anim ngắn, quái vẫn di chuyển
        }
        else
        {
            isHurt = true;
            animator.SetTrigger("HurtTrigger");
            // Quái bị cứng ngắn
        }

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
        StopChase();
     
        // Bật vật lý rơi
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 2f;

        ItemDropper dropper = GetComponent<ItemDropper>();
        if (dropper != null)
        {
            dropper.DropItems();
        }

    }

    void OnCollisionEnter2D(Collision2D col)
    {

        if (isDead && col.collider.CompareTag("Ground") || col.collider.CompareTag("MovingPlaform"))
        {
            StartCoroutine(DestroyAfter(0.5f));
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
        // Lưu lại tham chiếu đến player
        chasePlayerTarget = playerTransform;
        // Sinh một offset ngẫu nhiên, ví dụ trong khoảng (-1, 1) theo X và Y
        chaseOffset = new Vector2(Random.Range(-3f, 3f), 0f);

        // Tạo target trung gian
        GameObject chaseTargetObj = new GameObject("BatChaseTarget");
        chaseTargetObj.transform.position = playerTransform.position + (Vector3)chaseOffset;
        chaseTarget = chaseTargetObj.transform;

        if (aiDestination != null)
        {
            aiDestination.target = chaseTarget;
        }
    }


    public void StopChase()
    {
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

        if (chaseTarget != null)
        {
            Destroy(chaseTarget.gameObject);
            chaseTarget = null;
        }

        // Quay lại pointA (trong chế độ tuần tra)
        currentTarget = pointA;
    }

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

    void OnDrawGizmosSelected()
    {
        if (atk_point != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(atk_point.position, atkRange);
        }
    }
}
