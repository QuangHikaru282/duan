using UnityEngine;

public class FlameThrowerZone : MonoBehaviour
{
    [Header("FlameThrower Settings")]
    public float burnDPS = 10f;            // Ví dụ: 5 DPS
    // Các tham số cho DOT mode sau khi enemy rời khỏi vùng
    public float burnTickInterval = 1f;   // Thời gian giữa các tick DOT
    public int burnDamagePerTick = 10;       // Sát thương mỗi tick DOT
    public float burnRemainTime = 3f;       // Thời gian DOT áp dụng sau khi rời vùng

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Shield"))
        {
            Debug.Log("Phat hien dc tag shield");
            StatusEffectHandler seh = other.GetComponentInParent<StatusEffectHandler>();
            if (seh != null)
            {
                Debug.Log("FlameThrowerZone: Shield detected, stopping all burn effects.");
                seh.StopAllBurn(); // Dừng mọi hiệu ứng
            }
            return;
        }

        if (other.CompareTag("Enemy"))
        {
            StatusEffectHandler seh = other.GetComponentInParent<StatusEffectHandler>();
            if (seh != null)
            {
                seh.StartContinuousBurn(burnDPS, burnTickInterval, burnDamagePerTick, burnRemainTime);
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {

        if (other.CompareTag("Enemy"))
        {
            // Nếu enemy vẫn trong vùng, làm mới trạng thái continuous burn
            StatusEffectHandler seh = other.GetComponent<StatusEffectHandler>();
            if (seh != null)
            {
                seh.RefreshContinuousBurn();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            StatusEffectHandler seh = other.GetComponentInParent<StatusEffectHandler>();
            if (seh != null)
            {
                seh.StopContinuousBurnAndStartDOT();
            }
        }
    }
}
