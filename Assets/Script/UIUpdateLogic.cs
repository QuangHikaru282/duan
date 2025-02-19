using UnityEngine;
using TMPro;

public class UIUpdateLogic : MonoBehaviour
{
    public static UIUpdateLogic Instance;

    [Header("UI Elements")]
    [Tooltip("Text hiển thị số mũi tên của player (có thể chỉnh sửa font, căn lề qua Inspector)")]
    public TextMeshProUGUI arrowText;
    [Tooltip("Text hiển thị số chìa khóa của player (có thể chỉnh sửa font, căn lề qua Inspector)")]
    public TextMeshProUGUI keyText;

    // Các prefix được lấy từ text ban đầu (đã được định dạng qua Inspector)
    private string arrowPrefix;
    private string keyPrefix;

    void Awake()
    {
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
        if (arrowText != null)
        {
            arrowPrefix = arrowText.text; // ví dụ: "Arrow: " được đặt sẵn qua Inspector
        }
        if (keyText != null)
        {
            keyPrefix = keyText.text; // ví dụ: "Key: " được đặt sẵn
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

    // Lưu ý: Hiển thị HP được quản lý riêng qua HealthUIManager, do đó
    // UIUpdateLogic không còn xử lý cập nhật HP.
}
