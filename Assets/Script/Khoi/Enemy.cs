using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Thuộc tính sức khỏe
    [SerializeField]
    private int maxHealth = 20;
    private int currentHealth;

    // Hiệu ứng
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private Rigidbody2D rbd;

    public float moveSpeed = 2f;
    public float moveDistance = 5f;  
    public float attackRange = 1f;

    [SerializeField]
    private Transform player;
    [SerializeField]
    private bool isMovingRight = false;
    [SerializeField]
    private bool canAttack = true;
    [SerializeField]
    private bool isAttacking = false;
    [SerializeField]
    private bool isDead = false;
    [SerializeField]
    private Vector2 startPosition;

    // Tấn công
    [SerializeField]
    private int attackDamage = 10;
    [SerializeField]
    private Transform attackPoint;

    [SerializeField]
    private GameObject diamond;

    private Heal heal;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        startPosition = transform.position;
        currentHealth = maxHealth; // Khởi tạo máu ban đầu
    }

    // Hàm nhận sát thương
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Kiểm tra nếu máu giảm về 0
        if (currentHealth <= 0)
        {
            isDead = true;
            Die();
        }
        else
        {
            // Chạy hiệu ứng trúng đòn nếu cần
            animator.SetTrigger("Hurt");
        }
    }

    // Hàm xử lý khi Enemy chết
    private void Die()
    {
        // Chạy hiệu ứng chết
        animator.SetTrigger("Die");
        rb.bodyType = RigidbodyType2D.Kinematic;
        // Vô hiệu hóa collider để không thể tương tác nữa
        GetComponent<Collider2D>().enabled = false;

        // Ngừng di chuyển của Enemy (nếu có Rigidbody2D)
        if (GetComponent<Rigidbody2D>() != null)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        // Vô hiệu hóa đối tượng này sau một khoảng thời gian
        Destroy(gameObject, 0.6f);

        GameObject newDiamond = Instantiate(diamond, transform.position, Quaternion.identity);
        Rigidbody2D rbd = newDiamond.GetComponent<Rigidbody2D>();
        if (rbd != null)
        {
            // Thêm một lực ngẫu nhiên để tạo hiệu ứng nảy
            Vector2 bounceDirection = new Vector2(Random.Range(-1f, 1f), 1f);
            float bounceForce = Random.Range(2f, 5f);
            rbd.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);
        }

    }
    private void Update()
    {
        Move();
        CheckAttack();
    }

    private void Move()
    {
        
        if(!isAttacking && currentHealth >= 0 && isDead == false)
        {
            // Di chuyển trong phạm vi cố định
            float direction = isMovingRight ? 1 : -1;
            transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);

            // Kiểm tra nếu Enemy đã đi quá khoảng cách di chuyển
            float distanceFromStart = Vector2.Distance(transform.position, startPosition);
            if (distanceFromStart >= moveDistance)
            {
                Flip();
                startPosition = transform.position; // Cập nhật lại điểm bắt đầu
            }
            animator.SetFloat("Run", Mathf.Abs(distanceFromStart));
        }
        else
        {
            animator.SetFloat("Run", 0);
        }
    }

    private void CheckAttack()
    {
        // Kiểm tra khoảng cách tới Player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && canAttack)
        {
            canAttack = false;
            isAttacking = true;
            // Hiển thị hiệu ứng tấn công
            animator.SetTrigger("Attack");

            // Kiểm tra các đối tượng trong phạm vi tấn công
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);
            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.CompareTag("Player"))
                {
                    enemy.GetComponent<Player>().TakeDamage(attackDamage);
                }
            }

            // Bắt đầu Coroutine để thiết lập lại canAttack sau một khoảng thời gian
            StartCoroutine(ResetAttackCooldown());
        }
    }

    // Coroutine để thiết lập lại canAttack
    private IEnumerator ResetAttackCooldown()
    {
        // Thời gian chờ trước khi cho phép tấn công lại
        yield return new WaitForSeconds(0.8f); // Thay đổi 2f thành số giây bạn muốn

        canAttack = true;
        isAttacking = false;

    }
    private void Flip()
    {
        isMovingRight = !isMovingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
