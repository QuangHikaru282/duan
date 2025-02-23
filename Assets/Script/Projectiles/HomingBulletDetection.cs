using UnityEngine;

public class HomingBulletDetection : MonoBehaviour
{
    private HomingBullet parentBullet;
    private bool targetLocked = false;

    void Awake()
    {
        parentBullet = GetComponentInParent<HomingBullet>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (targetLocked) return; // chỉ lock 1 lần
        if (other.CompareTag("Enemy"))
        {
            // kẻ địch đầu tiên
            targetLocked = true;
            parentBullet.SetTarget(other.transform);
        }
    }
}
