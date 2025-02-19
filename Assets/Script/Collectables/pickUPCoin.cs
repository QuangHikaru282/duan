using UnityEngine;
using System.Collections;

public class PickUpCoin : MonoBehaviour
{
    [Header("Coin Pickup Settings")]
    [Tooltip("Số lượng coin mà item này cung cấp khi nhặt")]
    public int coinValue = 1;
    [Tooltip("Thời gian chờ trước khi hủy prefab coin (phù hợp với độ dài của hiệu ứng pickUpEffect, ví dụ: 0.7 giây)")]
    public float destroyDelay = 0.8f;

    private Animator animator;
    private bool isPickedUp = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPickedUp) return;

        if (collision.CompareTag("Player"))
        {
            isPickedUp = true;

            // Kích hoạt hiệu ứng nhặt coin thông qua trigger
            if (animator != null)
            {
                animator.SetBool("isPickedUp", true);

            }

            // Thêm coin qua hệ thống CoinManager
            if (CoinManager.Instance != null)
            {
                CoinManager.Instance.AddCoin(coinValue);
            }


            // Hủy coin sau khi delay để hiệu ứng pickUpEffect được phát đủ thời gian
            StartCoroutine(DestroyAfterDelay(destroyDelay));
        }
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
       yield return new WaitForSeconds(delay);
       Destroy(gameObject);
    }
}
