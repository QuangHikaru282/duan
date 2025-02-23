// =========================== FlameThrowerZone.cs (mới) ===========================
using UnityEngine;

public class FlameThrowerZone : MonoBehaviour
{
    public float burnTickInterval = 1f;
    public int burnDamagePerTick = 3;
    public float burnRemainTime = 2f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            StatusEffectHandler seh = other.GetComponent<StatusEffectHandler>();
            if (seh != null)
            {
                seh.ApplyBurn(burnTickInterval, burnDamagePerTick, burnRemainTime);
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            StatusEffectHandler seh = other.GetComponent<StatusEffectHandler>();
            if (seh != null)
            {
                seh.ResetBurnTimer();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            StatusEffectHandler seh = other.GetComponent<StatusEffectHandler>();
            if (seh != null)
            {
                seh.StartBurnCountdown();
            }
        }
    }
}
