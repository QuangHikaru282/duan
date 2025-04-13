using System.Collections;
using UnityEngine;

public class DarkHand : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damage = 2;
    public float enableColliderDelay = 0.7f;   
    public float disableColliderDelay = 0.9f;  
    public float autoDestroyDelay = 1.7f;      // Tổng thời gian tồn tại

    private Collider2D triggerCollider;

    void Start()
    {
        triggerCollider = GetComponent<Collider2D>();
        if (triggerCollider != null)
            triggerCollider.enabled = false;

        StartCoroutine(HandleColliderLifecycle());
    }

    IEnumerator HandleColliderLifecycle()
    {
        yield return new WaitForSeconds(enableColliderDelay);

        if (triggerCollider != null)
            triggerCollider.enabled = true;

        yield return new WaitForSeconds(disableColliderDelay - enableColliderDelay);

        if (triggerCollider != null)
            triggerCollider.enabled = false;

        yield return new WaitForSeconds(autoDestroyDelay - disableColliderDelay);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerScript ps = collision.GetComponent<playerScript>();
            if (ps != null)
            {
                ps.TakeDamage(damage);
            }
        }
    }
}
