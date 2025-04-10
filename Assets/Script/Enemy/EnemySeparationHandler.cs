using UnityEngine;

public class EnemySeparationHandler : MonoBehaviour
{
    [Header("Separation Settings")]
    public float separationRadius = 0.5f;
    public float separationForce = 2f;
    public LayerMask enemyLayer;

    private Rigidbody2D body;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    public void ApplySeparation()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, separationRadius, enemyLayer);

        foreach (var hit in hits)
        {
            if (hit.gameObject == this.gameObject) continue;

            // Tránh hardcast IEnemy, lấy transform luôn
            Transform other = hit.transform;
            Vector2 toOther = (Vector2)(transform.position - other.position);
            float distance = toOther.magnitude;

            if (distance < separationRadius && distance > 0.01f)
            {
                Vector2 pushDir = toOther.normalized;
                float pushStrength = (separationRadius - distance) * separationForce;
                body.velocity += pushDir * pushStrength;
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
#endif
}
