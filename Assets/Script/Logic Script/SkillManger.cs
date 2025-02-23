using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    [Header("Mana Settings")]
    public int maxMana = 100;
    public int currentMana = 100;

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
    }

    void Start()
    {
        // Cập nhật UI ban đầu
        ManaUIManager.Instance.UpdateManaUI(currentMana, maxMana);
    }

    public bool UseMana(int amount)
    {
        // Kiểm tra đủ mana
        if (currentMana < amount) return false;
        // Trừ mana
        currentMana -= amount;
        if (currentMana < 0) currentMana = 0;

        // Cập nhật UI
        ManaUIManager.Instance.UpdateManaUI(currentMana, maxMana);
        return true;
    }

    public void AddMana(int amount)
    {
        currentMana += amount;
        if (currentMana > maxMana) currentMana = maxMana;
        ManaUIManager.Instance.UpdateManaUI(currentMana, maxMana);
    }
}
