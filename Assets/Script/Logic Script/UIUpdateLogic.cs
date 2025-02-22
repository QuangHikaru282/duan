using UnityEngine;
using TMPro;

public class UIUpdateLogic : MonoBehaviour
{
    public static UIUpdateLogic Instance;

    [Header("UI Elements")]
    [Tooltip("Text hiển thị số mũi tên của player (dạng: Số lượng x)")]
    public TextMeshProUGUI arrowText;
    [Tooltip("Text hiển thị số chìa khóa của player (dạng: Số lượng x)")]
    public TextMeshProUGUI keyText;
    [Tooltip("Text hiển thị số coin của player (dạng: 4 chữ số, ví dụ: 0010)")]
    public TextMeshProUGUI coinText;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Cập nhật hiển thị số mũi tên theo định dạng "x Số lượng"
    /// Ví dụ: nếu arrowCount là 5, hiển thị "x 5"
    /// </summary>
    public void UpdateArrowUI(int arrowCount)
    {
        if (arrowText != null)
        {
            arrowText.text = "x" + arrowCount.ToString();
        }
    }

    /// <summary>
    /// Cập nhật hiển thị số chìa khóa theo định dạng "x Số lượng"
    /// Ví dụ: nếu keyCount là 3, hiển thị "x 3"
    /// </summary>
    public void UpdateKeyUI(int keyCount)
    {
        if (keyText != null)
        {
            keyText.text = "x" + keyCount.ToString();
        }
    }

    /// <summary>
    /// Cập nhật hiển thị số coin theo định dạng 4 chữ số.
    /// Ví dụ: nếu coinCount là 10, hiển thị "0010"
    /// </summary>
    public void UpdateCoinUI(int coinCount)
    {
        if (coinText != null)
        {
            coinText.text = coinCount.ToString("D4");
        }
    }
}
