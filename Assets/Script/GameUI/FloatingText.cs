using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float destroyDelay = 2f;
    public Vector2 offset = new Vector2(0, 30f); 
    public Vector2 randomizeIntensity = new Vector2(15f, 10f);

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogWarning("FloatingText requires RectTransform");
            return;
        }

        Destroy(gameObject, destroyDelay);

        Vector2 finalOffset = offset + new Vector2(
            Random.Range(-randomizeIntensity.x, randomizeIntensity.x),
            Random.Range(-randomizeIntensity.y, randomizeIntensity.y)
        );

        rectTransform.anchoredPosition += finalOffset;
    }

    public void SetText(string content)
    {
        var tmp = GetComponent<TextMeshProUGUI>();
        if (tmp != null)
            tmp.text = content;
        else
            Debug.LogWarning("Missing TMP component on FloatingText!");
    }
}
