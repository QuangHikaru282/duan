using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // Hiệu ứng
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Animator animatorUI;

    // Di chuyển
    [SerializeField]
    private float moveSpeed = 6f;

    // Nhảy
    [SerializeField]
    private float jumpForce = 10f;

    // Vật lý
    [SerializeField]
    private Rigidbody2D rb;

    // Xác định trạng thái
    [SerializeField]
    private bool isGrounded = false;
    private bool facingRight = true;
    private bool isDie = false;

    // Kiểm tra mặt đất
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private float groundCheckRadius = 1f;

    // Tấn công
    [SerializeField]
    private float attackRange = 1f;
    [SerializeField]
    private int attackDamage = 10;
    [SerializeField]

    // máu
    private Transform attackPoint;

    [SerializeField]
    private int maxHealth = 20;
    [SerializeField]
    private int currentHealth;
    [SerializeField]
    private int diamond = 0;
    [SerializeField]
    protected internal int currentDiamond;

    [SerializeField]
    private TextMeshProUGUI diamondText;
    [SerializeField]
    private Heal heal;
    [SerializeField]
    private Healthbar healthbar;
    [SerializeField]
    private Destroy Destroy;

    [SerializeField]
    private CinemachineVirtualCamera virtualCamera1;
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera2;
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera3;
    [SerializeField]
    private GameObject youdieUI;
    [SerializeField]
    private GameObject darkUI;
    [SerializeField]
    private static Player instance;

    public void SavePlayerData()
    {
 
    }

    public void LoadPlayerData()
    {
        
    }
    private void Awake()
    {
        
    }

   




    // Hiển thị phạm vi tấn công trong Scene view để tiện chỉnh sửa
    private void OnDrawGizmosSelected()
    {
        
    }



    // Hàm nhận sát thương
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        // Kiểm tra nếu máu giảm về 0
        if (currentHealth <= 0)
        {
            //animatorUI.SetBool("Hurt", currentHealth >= 0);
            //StartCoroutine(WaitAndRemoveHeart());
            heal.RemoveHeart();
            Die();
        }
        else
        {
            //animatorUI.SetBool("Hurt", currentHealth >= 0);
            //StartCoroutine(WaitAndRemoveHeart());
            // Chạy hiệu ứng trúng đòn nếu cần
            heal.RemoveHeart();
            animator.SetTrigger("Hit");
        }
    }
    private IEnumerator WaitAndRemoveHeart()
    {
        // Chờ cho đến khi hoạt động animatorUI.SetBool hoàn thành (hoặc bạn có thể chờ một thời gian cụ thể)
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Sau khi chờ, thực hiện RemoveHeart
        heal.RemoveHeart();
    }


    // Hàm xử lý khi Player chết
    private void Die()
    {
        // Chạy hiệu ứng chết
        animator.SetTrigger("Die");
        rb.bodyType = RigidbodyType2D.Kinematic;
        // Vô hiệu hóa collider để không thể tương tác nữa
        GetComponent<Collider2D>().enabled = false;

        // Ngừng di chuyển của Player (nếu có Rigidbody2D)
        if (GetComponent<Rigidbody2D>() != null)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

        // Vô hiệu hóa đối tượng này sau một khoảng thời gian
        
        // Thay đổi sprite   
        //Destroy(gameObject, 0.6f);
        StartCoroutine(WaitAndPauseGame(0.58f));
    }
    private IEnumerator WaitAndPauseGame(float waitTime)
    {
        // Chờ trong khoảng thời gian được chỉ định
        yield return new WaitForSeconds(waitTime);
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(0.28f);
        darkUI.SetActive(true);
        youdieUI.gameObject.SetActive(true);
        // Dừng game
        Time.timeScale = 0;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Diamond"))
        {
            currentDiamond += 1;
            // Xóa đối tượng Diamond
            Destroy(collision.gameObject);
            diamondText.text = currentDiamond.ToString();
            Debug.Log("mmm" +currentDiamond);
        }
        if (collision.gameObject.CompareTag("Heart") && currentHealth < maxHealth)
        {
                currentHealth = currentHealth + 10;
                heal.AddHeart();
                // Xóa đối tượng Diamond
                Destroy(collision.gameObject);
                Debug.Log("Player đã va chạm với Heal!");
        }
        if (collision.gameObject.CompareTag("AddHeart"))
        {
            maxHealth += 10;
            healthbar.AddImageHeart();
            // Xóa đối tượng Diamond
            Destroy(collision.gameObject);
            Debug.Log("Player đã va chạm với Add Heart!");
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            isDie = true;
            Die();
        }
        if (collision.gameObject.CompareTag("Scene transition"))
        {
            virtualCamera1.gameObject.SetActive(false);
            virtualCamera2.gameObject.SetActive(true);
            virtualCamera3.gameObject.SetActive(false);
            Destroy.removeAll();
            Destroy(gameObject, 1f);
            Debug.Log("Scene transition");
        }
    }
}
