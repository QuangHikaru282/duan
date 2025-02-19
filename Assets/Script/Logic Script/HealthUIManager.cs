// HealthUIManager.cs
using UnityEngine;
using TMPro;

public class HealthUIManager : MonoBehaviour
{
    public static HealthUIManager Instance;

    [Header("Health Bar Settings")]
    // healthBar là đối tượng cha (Image) chứa 3 đối tượng con heartFill1, heartFill2, heartFill3.
    public Transform healthBar;
    public TextMeshProUGUI heartCountText;    // Text hiển thị multiplier "xN"

    public int slotCount = 3;  // Số slot cố định

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Khởi tạo thanh HP dựa trên HP ban đầu của player.
    /// </summary>
    public void InitializeHealthBar(int initialHP)
    {
        UpdateHealthUI(initialHP);
    }

    /// <summary>
    /// Cập nhật giao diện thanh HP:
    /// - Tính totalRows = ceil(currentHP / 3)
    /// - Tính fillCount = (currentHP % 3 == 0 && currentHP > 0) ? 3 : (currentHP % 3)
    /// - Duyệt qua 3 đối tượng con của healthBar để bật/tắt.
    /// - Cập nhật multiplier hiển thị "x" + totalRows.
    /// </summary>
    public void UpdateHealthUI(int currentHP)
    {
        int totalRows = (currentHP > 0) ? Mathf.CeilToInt(currentHP / 3f) : 0;
        int fillCount = 0;
        if (currentHP > 0)
            fillCount = (currentHP % 3 == 0) ? 3 : (currentHP % 3);

        // Duyệt qua các đối tượng con của healthBar (giả sử có đúng 3 con)
        for (int i = 0; i < slotCount; i++)
        {
            Transform heartFill = healthBar.GetChild(i);
            heartFill.gameObject.SetActive(i < fillCount);
        }

        if (heartCountText != null)
            heartCountText.text = "x" + totalRows.ToString();
    }
}
