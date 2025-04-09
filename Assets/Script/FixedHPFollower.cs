using UnityEngine;

public class FixedHPFollower : MonoBehaviour
{
    private Transform parent;
    private Vector3 initialLocalScale;

    void Awake()
    {
        parent = transform.parent;
        initialLocalScale = transform.localScale;
    }

    void LateUpdate()
    {
        if (parent == null) return;

        Vector3 parentScale = parent.localScale;
        Vector3 newScale = initialLocalScale;

        // Nếu cha bị flip X (scale.x < 0), ta flip ngược lại để giữ hướng đúng
        newScale.x = (parentScale.x < 0f) ? -Mathf.Abs(initialLocalScale.x) : Mathf.Abs(initialLocalScale.x);

        transform.localScale = newScale;
    }
}
