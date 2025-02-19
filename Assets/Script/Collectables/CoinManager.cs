using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("Coin Settings")]
    [Tooltip("Lượng coin hiện tại của player")]
    public int coinCount = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Thêm coin vào tổng lượng coin và cập nhật UI.
    /// </summary>
    /// <param name="amount">Số lượng coin cần thêm</param>
    public void AddCoin(int amount)
    {
        coinCount += amount;
        UpdateCoinUI();
    }

    /// <summary>
    /// Nếu cần dùng: giảm coin (ví dụ khi mua vật phẩm).
    /// </summary>
    public bool SpendCoin(int amount)
    {
        if (coinCount >= amount)
        {
            coinCount -= amount;
            UpdateCoinUI();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Cập nhật giao diện coin thông qua UIUpdateLogic.
    /// </summary>
    void UpdateCoinUI()
    {
        if (UIUpdateLogic.Instance != null)
            UIUpdateLogic.Instance.UpdateCoinUI(coinCount);
    }
}
