using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManaUIManager : MonoBehaviour
{
    public static ManaUIManager Instance { get; private set; }

    [Header("UI References")]
    public Slider manaSlider;
    public TextMeshProUGUI manaText;

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
        // DontDestroyOnLoad(gameObject); // tùy
    }

    public void UpdateManaUI(int currentMana, int maxMana)
    {
        if (manaSlider != null)
        {
            manaSlider.maxValue = maxMana;
            manaSlider.value = currentMana;
        }

        if (manaText != null)
        {
            float percent = (float)currentMana / maxMana * 100f;
            manaText.text = $"{(int)percent}%";
            // Hoặc hiển thị currentMana: manaText.text = $"{currentMana}/{maxMana}";
        }
    }
}
