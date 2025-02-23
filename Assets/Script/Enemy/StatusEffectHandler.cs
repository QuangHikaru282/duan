using UnityEngine;
using System.Collections;

public class StatusEffectHandler : MonoBehaviour
{
    IEnemy enemy;

    // --- Continuous Burn Mode (khi enemy đang trong vùng)
    bool isContinuousBurning = false;
    float continuousDPS = 0f;
    float continuousDamageBuffer = 0f; // để tích lũy damage theo thời gian

    // --- DOT Burn Mode (sau khi enemy rời vùng)
    bool isDOTBurning = false;
    float dotTickInterval = 1f;  // khoảng thời gian giữa các tick
    int dotDamagePerTick = 5;      // sát thương mỗi tick
    float dotRemainTime = 3f;      // tổng thời gian DOT áp dụng sau khi rời vùng
    float dotTickTimer = 0f;       // để tính thời gian giữa các tick

    public GameObject burnEffectPrefab;
    private GameObject burnEffectInstance;

    void Awake()
    {
        enemy = GetComponent<IEnemy>();
    }

    public void StartContinuousBurn(float dps, float tickInterval, int dmgPerTick, float remainTime)
    {
        if (enemy == null) return;

        continuousDPS = dps;
        continuousDamageBuffer = 0f;
        isContinuousBurning = true;
        isDOTBurning = false; // đảm bảo chế độ DOT tắt khi enemy ở vùng

        // Lưu giá trị cho DOT mode
        dotTickInterval = tickInterval;
        dotDamagePerTick = dmgPerTick;
        dotRemainTime = remainTime;

        StartBurnEffect();
    }

    public void RefreshContinuousBurn()
    {
        if (!isContinuousBurning)
        {
            // Nếu enemy đã chuyển sang DOT mode, chuyển lại continuous
            isContinuousBurning = true;
            isDOTBurning = false;
            continuousDamageBuffer = 0f;
        }
        // Nếu enemy vẫn trong vùng, không cần thay đổi gì.
    }

    /// <summary>
    /// Gọi khi enemy rời khỏi vùng flame (hoặc player ngắt skill).
    /// Chuyển từ continuous mode sang DOT mode (tick damage theo chế độ gốc).
    /// </summary>
    public void StopContinuousBurnAndStartDOT()
    {
        isContinuousBurning = false;
        isDOTBurning = true;
        dotTickTimer = 0f;
        // dotRemainTime đã được thiết lập khi bắt đầu continuous burn.
    }

    /// <summary>
    /// Dừng hoàn toàn hiệu ứng burn.
    /// </summary>
    public void StopAllBurn()
    {
        isContinuousBurning = false;
        isDOTBurning = false;
        StopBurnEffect();
    }

    void Update()
    {
        if (enemy == null) return;

        // --- Continuous Burn Mode
        if (isContinuousBurning)
        {
            continuousDamageBuffer += continuousDPS * Time.deltaTime;
            if (continuousDamageBuffer >= 1f)
            {
                int dmg = Mathf.FloorToInt(continuousDamageBuffer);
                continuousDamageBuffer -= dmg;
                enemy.TakeDamage(dmg, "ContinuousBurn", 0);
                // Mỗi khi nhận damage với tag "ContinuousBurn", enemy có thể phát animation DotHurt (hoặc Hurt) theo thiết kế.
            }
        }
        // --- DOT Burn Mode
        else if (isDOTBurning)
        {
            dotTickTimer += Time.deltaTime;
            if (dotTickTimer >= dotTickInterval)
            {
                dotTickTimer = 0f;
                enemy.TakeDamage(dotDamagePerTick, "DOT", 0);
            }
            dotRemainTime -= Time.deltaTime;
            if (dotRemainTime <= 0f)
            {
                StopAllBurn();
            }
        }
    }

    void StartBurnEffect()
    {
        // Spawn burn effect prefab nếu chưa tồn tại
        if (burnEffectPrefab != null && burnEffectInstance == null)
        {
            burnEffectInstance = Instantiate(burnEffectPrefab, transform.position, Quaternion.identity);
            burnEffectInstance.transform.SetParent(this.transform, true);
        }
    }

    void StopBurnEffect()
    {
        if (burnEffectInstance != null)
        {
            Destroy(burnEffectInstance);
            burnEffectInstance = null;
        }
    }
}
