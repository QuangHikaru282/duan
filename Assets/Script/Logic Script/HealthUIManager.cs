// HealthUIManager.cs
using UnityEngine;
using TMPro;

public class HealthUIManager : MonoBehaviour
{
    public static HealthUIManager Instance;

    [Header("Health Bar Settings")]
    public Transform healthBar;
    public TextMeshProUGUI heartCountText; 

    public int slotCount = 3; 

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public void InitializeHealthBar(int initialHP)
    {
        UpdateHealthUI(initialHP);
    }
    public void UpdateHealthUI(int currentHP)
    {
        int totalRows = (currentHP > 0) ? Mathf.CeilToInt(currentHP / 3f) : 0;
        int fillCount = 0;
        if (currentHP > 0)
            fillCount = (currentHP % 3 == 0) ? 3 : (currentHP % 3);

        for (int i = 0; i < slotCount; i++)
        { 
            Transform heartFill = healthBar.GetChild(i);
            heartFill.gameObject.SetActive(i < fillCount);
        }

        if (heartCountText != null)
            heartCountText.text = "x" + totalRows.ToString();
    }
}
