using UnityEngine;

public class FlameThrowerZone : MonoBehaviour
{
    // Các tham số cho continuous damage khi enemy ở trong vùng
    [Header("FlameThrower Settings")]
    public float burnDPS = 10f;            // Ví dụ: 5 DPS
    // Các tham số cho DOT mode sau khi enemy rời khỏi vùng
    public float burnTickInterval = 1f;   // Thời gian giữa các tick DOT
    public int burnDamagePerTick = 10;       // Sát thương mỗi tick DOT
    public float burnRemainTime = 3f;       // Thời gian DOT áp dụng sau khi rời vùng

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            StatusEffectHandler seh = other.GetComponent<StatusEffectHandler>();
            if (seh != null)
            {
                // Khi enemy bước vào vùng, bắt đầu chế độ continuous burn
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
            // Khi enemy rời khỏi vùng, chuyển sang chế độ DOT burn (tick damage theo cơ chế gốc)
            StatusEffectHandler seh = other.GetComponent<StatusEffectHandler>();
            if (seh != null)
            {
                seh.StopContinuousBurnAndStartDOT();
            }
        }
    }
}
