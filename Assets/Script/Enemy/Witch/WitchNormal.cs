using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum WitchState { Idle, Alert, Attack, Teleport, Reset }

public class WitchNormal : MonoBehaviour, IEnemy
{
    [Header("Health Settings")]
    public Slider hpSlider; // HP Canvas Slider (đã được gán qua Inspector)
    private int currentHealth;
    
    [Header("Detection & State Settings")]
    public float resetTime = 10f;
    private float timeSinceLastDetection = 0f;

    [Header("Attack Settings")]
    public float attackCooldown = 5.0f;
    public Transform firePoint; // Vị trí spawn đạn
    public GameObject witchBulletPrefab; // Prefab đạn
    public GameObject meleeEffect, rangeEffect;

    [Header("Teleport Settings")]
    public float detectionAreaWidth = 15f;
    public float detectionAreaHeight = 6f;
    public float teleportDelay = 1.0f;
    public int emergencyDamageThreshold = 5;
    private int accumulatedDamage = 0;
    private Vector2 initialPosition;

    [Header("Alert Mark Settings")]
    public GameObject alertMarkPrefab; // Prefab AlertMark
    private GameObject currentAlertMark;

    // Các flag và state
    private WitchState currentState = WitchState.Idle;
    private bool playerDetected = false;
    private bool isDead = false;

    // Để hỗ trợ flip sprite (và các object con như FirePoint)
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private int facingDirection = 1; // 1: hướng phải, -1: hướng trái
    public AudioSource audioSource; // Âm thanh bắn
    public AudioClip shootSound;    // File âm thanh

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // HP từ Slider (giả sử maxValue đã được set trong Inspector)
        if (hpSlider != null)
        {
            currentHealth = (int)hpSlider.maxValue;
            hpSlider.value = hpSlider.maxValue;
        }
        else currentHealth = 10;

        initialPosition = transform.position;
        currentState = WitchState.Idle;
        timeSinceLastDetection = 0f;
    }

    void Update()
    {
        // State machine update
        switch (currentState)
        {
            case WitchState.Idle:
                IdleState();
                break;
            case WitchState.Alert:
                // Trong Alert, Witch vẫn giữ Idle animation; AlertMark đã được hiển thị.
                break;
            case WitchState.Attack:
                // Attack animation được điều khiển qua Animation Event (SpawnBullet, OnAttackComplete)
                break;
            case WitchState.Teleport:
                // Trong Teleport, không cập nhật state
                break;
            case WitchState.Reset:
                // Reset được xử lý qua coroutine
                break;
        }

        // Cập nhật timer nếu không phát hiện player
        if (playerDetected)
            timeSinceLastDetection = 0f;
        else
        {
            timeSinceLastDetection += Time.deltaTime;
            if (timeSinceLastDetection >= resetTime && currentState != WitchState.Reset)
                StartCoroutine(ResetPositionAndHP());
        }

        // Cập nhật flip sprite theo vị trí player (nếu có)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            float diff = playerObj.transform.position.x - transform.position.x;
            if (diff < -0.1f && facingDirection != -1)
                SetFacingDirection(-1);
            else if (diff > 0.1f && facingDirection != 1)
                SetFacingDirection(1);
        }
    }

    void IdleState()
    {
        // Witch đứng yên, canh gác
        if (playerDetected)
            StartAlert();
    }

    // Các hàm gọi từ WitchDetectionArea:
    public void OnPlayerDetected()
    {
        playerDetected = true;
        timeSinceLastDetection = 0f;
        if (currentState == WitchState.Idle)
            StartAlert();
    }
    public void OnPlayerLost()
    {
        playerDetected = false;
    }

    // Khi phát hiện player, Witch không thay đổi animation của chính nó; chỉ instantiate AlertMark
    void StartAlert()
    {
        if (currentState != WitchState.Idle) return;
        currentState = WitchState.Alert;
        if (alertMarkPrefab != null && currentAlertMark == null)
        {
            currentAlertMark = Instantiate(alertMarkPrefab, transform);
            currentAlertMark.transform.localPosition = new Vector3(-0.15f, 1.5f, 0f);
        }
        // Sau khi AlertMark chạy xong (ví dụ 1.0 giây), chuyển sang Attack
        StartCoroutine(AlertDelay(1.0f));
    }

    IEnumerator AlertDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (currentAlertMark != null)
        {
            Destroy(currentAlertMark);
            currentAlertMark = null;
        }
        currentState = WitchState.Attack;
        animator.SetTrigger("AttackTrigger");
    }

    // Trong clip Attack, Animation Event gọi hàm này tại frame bắn
    public void SpawnBullet()
    {
        if (firePoint == null || witchBulletPrefab == null || !playerDetected)
            return;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return;
        Vector2 targetPos = playerObj.transform.position;
        Vector2 dir = (targetPos - (Vector2)firePoint.position).normalized;
        // Tính góc dựa trên vector direction (sprite mặc định hướng phải)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // Instantiate WitchBullet với Quaternion.Euler(0,0,angle)
        GameObject bulletObj = Instantiate(witchBulletPrefab, firePoint.position, Quaternion.Euler(0, 0, angle));
        WitchBullet bullet = bulletObj.GetComponent<WitchBullet>();
          if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }


    // Trong clip Attack, Animation Event ở cuối clip gọi hàm này
    public void OnAttackComplete()
    {
        animator.ResetTrigger("AttackTrigger");
        StartCoroutine(TeleportRoutine());
    }

    IEnumerator TeleportRoutine()
    {
        currentState = WitchState.Teleport;
        animator.SetTrigger("TeleportTrigger");
        yield return new WaitForSeconds(teleportDelay);
        Vector2 newPos = GetSafeTeleportPosition();
        transform.position = newPos;
        currentState = WitchState.Idle;
    }

    Vector2 GetSafeTeleportPosition()
    {
        float minX, maxX, minY, maxY;
        // Lấy đối tượng Player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            Vector2 playerPos = (Vector2)playerObj.transform.position;
            float dist = Vector2.Distance(transform.position, playerPos);
            // Nếu khoảng cách vượt quá 8f, sử dụng vùng quanh initialPosition
            if (dist > 8f)
            {
                minX = initialPosition.x - detectionAreaWidth / 2f;
                maxX = initialPosition.x + detectionAreaWidth / 2f;
                minY = initialPosition.y - detectionAreaHeight / 2f;
                maxY = initialPosition.y + detectionAreaHeight / 2f;
            }
            else
            {
                // Nếu player đang gần, lựa chọn vùng ưu tiên dựa trên vị trí của player
                if (playerPos.x >= transform.position.x)
                {
                    // Player ở bên phải: chọn vùng phía phải của player
                    minX = playerPos.x + 2f;
                    maxX = playerPos.x + 6f;
                }
                else
                {
                    // Player ở bên trái: chọn vùng phía trái của player
                    minX = playerPos.x - 6f;
                    maxX = playerPos.x - 2f;
                }
                float verticalRange = 1.5f;
                minY = playerPos.y - verticalRange;
                maxY = playerPos.y + verticalRange;
            }
        }
        else
        {
            // Nếu không tìm thấy player, sử dụng vùng quanh initialPosition
            minX = initialPosition.x - detectionAreaWidth / 2f;
            maxX = initialPosition.x + detectionAreaWidth / 2f;
            minY = initialPosition.y - detectionAreaHeight / 2f;
            maxY = initialPosition.y + detectionAreaHeight / 2f;
        }

        // Sinh candidate từ vùng đã xác định
        const int maxAttempts = 10;
        int attempts = 0;
        Vector2 candidate = transform.position;
        bool found = false;
        Vector2 playerPosFinal = Vector2.zero;
        if (playerObj != null)
            playerPosFinal = (Vector2)playerObj.transform.position;

        while (attempts < maxAttempts && !found)
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);
            candidate = new Vector2(randomX, randomY);

            // Kiểm tra xem candidate có nằm trong collider của ground hay MovingPlaform không
            Collider2D col = Physics2D.OverlapPoint(candidate, LayerMask.GetMask("Ground", "MovingPlaform"));
            if (col != null)
            {
                // Nếu candidate nằm trong collider, đặt candidate.y lên bằng bounds.max.y cộng offset
                candidate.y = col.bounds.max.y + 0.5f; // offset có thể điều chỉnh
            }

            // Kiểm tra khoảng cách theo chiều dọc so với player (nếu có)
            if (playerObj != null)
            {
                if (Mathf.Abs(candidate.y - playerPosFinal.y) <= 1.5f)
                {
                    found = true;
                    break;
                }
            }
            else
            {
                found = true;
                break;
            }
            attempts++;
        }

        return candidate;
    }


    IEnumerator ResetPositionAndHP()
    {
        currentState = WitchState.Reset;
        currentHealth = (int)hpSlider.maxValue;
        if (hpSlider != null)
            hpSlider.value = hpSlider.maxValue;
        transform.position = initialPosition;
        playerDetected = false;
        timeSinceLastDetection = 0f;
        yield return new WaitForSeconds(0.5f);
        currentState = WitchState.Idle;
    }

    // ------------------- TAKE DAMAGE (IEnemy) -------------------
    public void TakeDamage(int dmg, string dmgType, int attackDir)
    {
        if (isDead) return;
        currentHealth -= dmg;
        if (hpSlider != null)
            hpSlider.value = currentHealth;
        ShowHitEffect(dmgType, attackDir);
        if (dmgType == "DOT")
        {
            animator.SetTrigger("DotHurtTrigger");
            // Anim ngắn, quái vẫn di chuyển
        }
        else
        {
            animator.SetTrigger("HurtTrigger");
            // Quái bị cứng ngắn
        }
        accumulatedDamage += dmg;
        animator.ResetTrigger("AttackTrigger");

        if (accumulatedDamage >= emergencyDamageThreshold && currentState == WitchState.Attack)
        {
            accumulatedDamage = 0;
            Vector2 newPos = GetSafeTeleportPosition();
            transform.position = newPos;
            currentState = WitchState.Idle;
            return;
        }
        if (currentHealth <= 0)
            Die();
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

    void Die()
    {
        if (isDead) return;
        isDead = true;
        animator.SetBool("isDead", true);
        if (hpSlider != null)
            Destroy(hpSlider.gameObject);
        StartCoroutine(DieVanish(2f));
        ItemDropper dropper = GetComponent<ItemDropper>();
        if (dropper != null)
        {
            dropper.DropItems();
        }
    }

    IEnumerator DieVanish(float delay)
    {

        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isDead && other.CompareTag("Trap"))
            Die();
           
    }

    // Hàm cập nhật flip sprite và FirePoint
    void SetFacingDirection(int dir)
    {
        if (facingDirection == dir) return;
        facingDirection = dir;
        spriteRenderer.flipX = (dir < 0);
        if (firePoint != null)
            firePoint.localPosition = new Vector3(0.3f * dir, firePoint.localPosition.y, 0f);
    }
}
