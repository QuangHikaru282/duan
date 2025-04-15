using UnityEngine;
using System.Collections;

public class IceBlast : MonoBehaviour
{
    [Header("IceBlast Settings")]
    public float speed = 12f;
    public float lifeTime = 4f;
    public int damage = 2;

    private Rigidbody2D rb;
    private bool hasTriggered = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Vector2 direction = new Vector2(Mathf.Sign(transform.localScale.x), 0f);
        rb.velocity = direction * speed;

        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            var ps = other.GetComponent<playerScript>();
            if (ps != null)
            {
                ps.TakeDamage(damage);
                HitStopManager.Instance.TriggerHitStop(0.25f);

                var freezeHandler = other.GetComponent<FreezeStatusHandler>();
                if (freezeHandler != null)
                {
                    freezeHandler.ApplyFreeze();
                }

                TriggerDestroy();
            }
        }
    }

    private void TriggerDestroy()
    {
        hasTriggered = true;
        rb.velocity = Vector2.zero;

        var anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("ExplodeTrigger");
            StartCoroutine(DelayedDestroy());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(0.6f); 
        Destroy(gameObject);
    }
}
