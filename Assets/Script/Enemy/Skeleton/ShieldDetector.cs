using UnityEngine;

public class ShieldDetector : MonoBehaviour
{
    private Skeleton skeleton;

    private void Awake()
    {
        skeleton = GetComponentInParent<Skeleton>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Skill") && !other.CompareTag("AOE")) return;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null && skeleton != null)
        {
            float direction = rb.velocity.x;
            float skeletonFacing = skeleton.transform.localScale.x;

            // Nếu đạn bay tới từ hướng ngược lại → lật quái
            if ((direction < 0 && skeletonFacing < 0) || (direction > 0 && skeletonFacing > 0))
            {
                Vector3 scale = skeleton.transform.localScale;
                scale.x *= -1;
                skeleton.transform.localScale = scale;
            }
        }

        skeleton?.OnProjectileDetected();
    }
}
