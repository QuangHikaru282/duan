using UnityEngine;
using TMPro;

public class UIUpdateLogic : MonoBehaviour
{
    public static UIUpdateLogic Instance;

    [Header("UI Elements")]
    [Tooltip("Text hiển thị HP của player (có thể chỉnh sửa font, căn lề qua Inspector)")]
    public TextMeshProUGUI healthText;
    [Tooltip("Text hiển thị số mũi tên của player (có thể chỉnh sửa font, căn lề qua Inspector)")]
    public TextMeshProUGUI arrowText;
    [Tooltip("Text hiển thị số chìa khóa của player (có thể chỉnh sửa font, căn lề qua Inspector)")]
    public TextMeshProUGUI keyText;

    // Các prefix được lấy từ text ban đầu (đã được định dạng qua Inspector)
    private string healthPrefix;
    private string arrowPrefix;
    private string keyPrefix;

    void Awake()
    {
        // Thiết lập singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Lưu lại giá trị prefix ban đầu từ các TMP element
        if (healthText != null)
        {
            healthPrefix = healthText.text; // ví dụ: "HP: " được đặt sẵn trong Inspector
        }
        if (arrowText != null)
        {
            arrowPrefix = arrowText.text; // ví dụ: "Arrow: " được đặt sẵn
        }
        if (keyText != null)
        {
            keyPrefix = keyText.text; // ví dụ: "Key: " được đặt sẵn
        }
    }

    /// <summary>
    /// Cập nhật hiển thị HP theo định dạng từ Inspector.
    /// </summary>
    /// <param name="currentHealth">Giá trị HP hiện tại</param>
    public void UpdateHealthUI(int currentHealth)
    {
        if (healthText != null)
        {
            healthText.text = healthPrefix + currentHealth;
        }
    }

    /// <summary>
    /// Cập nhật hiển thị số mũi tên theo định dạng từ Inspector.
    /// </summary>
    /// <param name="arrowCount">Số mũi tên hiện tại</param>
    public void UpdateArrowUI(int arrowCount)
    {
        if (arrowText != null)
        {
            arrowText.text = arrowPrefix + arrowCount;
        }
    }

    /// <summary>
    /// Cập nhật hiển thị số chìa khóa theo định dạng từ Inspector.
    /// </summary>
    /// <param name="keyCount">Số chìa khóa hiện tại</param>
    public void UpdateKeyUI(int keyCount)
    {
        if (keyText != null)
        {
            keyText.text = keyPrefix + keyCount;
        }
    }

    // Sau này bạn có thể bổ sung thêm các hàm UpdateManaUI, UpdateCoinUI, v.v.
}
