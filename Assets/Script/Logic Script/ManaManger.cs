using UnityEngine;

public class ManaManager : MonoBehaviour
{
    public static ManaManager Instance { get; private set; }

    [Header("Mana Settings")]
    public int maxMana = 100;
    public int currentMana = 100; // Hoặc 0 nếu muốn

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        // Giữ qua scene khác nếu muốn
         DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Khởi tạo UI hiển thị ban đầu
        ManaUIManager.Instance.UpdateManaUI(currentMana, maxMana);
    }

    public void AddMana(int amount)
    {
        currentMana += amount;
        if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }

        // Cập nhật UI
        ManaUIManager.Instance.UpdateManaUI(currentMana, maxMana);
    }

    public bool UseMana(int amount)
    {
        if (currentMana < amount)
        {
            return false; // Không đủ mana
        }

        currentMana -= amount;
        if (currentMana < 0) currentMana = 0;

        // Cập nhật UI
        ManaUIManager.Instance.UpdateManaUI(currentMana, maxMana);

        return true;
    }
}
