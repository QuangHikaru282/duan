using UnityEngine;
using TMPro;

public class UIUpdateLogic : MonoBehaviour
{
    public static UIUpdateLogic Instance;

    [Header("UI Elements")]
    [Tooltip("Text hiển thị số mũi tên của player (dạng: Số lượng x)")]
    public TextMeshProUGUI arrowText;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void UpdateArrowUI(int arrowCount)
    {
        if (arrowText != null)
        {
            arrowText.text = "x" + arrowCount.ToString();
        }
    }
}
