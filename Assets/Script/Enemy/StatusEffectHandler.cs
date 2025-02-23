// =========================== StatusEffectHandler.cs (mới) ===========================
using UnityEngine;
using System.Collections;

public class StatusEffectHandler : MonoBehaviour
{
    IEnemy enemy;
    bool isBurning = false;
    float burnTickInterval = 1f;
    int burnDamagePerTick = 3;
    float burnRemainTime = 2f;
    float burnTimer = 0f;
    float tickTimer = 0f;
    bool burnCountdown = false;
    public GameObject burnEffectPrefab;
    private GameObject burnEffectInstance;

    void Awake()
    {
        enemy = GetComponent<IEnemy>();
    }

    public void ApplyBurn(float tickInterval, int dmgPerTick, float remainTime)
    {
        if (enemy == null) return;
        burnTickInterval = tickInterval;
        burnDamagePerTick = dmgPerTick;
        burnRemainTime = remainTime;
        burnTimer = remainTime;
        tickTimer = 0f;
        isBurning = true;
        burnCountdown = false;
        // Gọi hàm để hiển thị effect burn
        StartBurnEffect();
    }

    public void ResetBurnTimer()
    {
        burnTimer = burnRemainTime;
        burnCountdown = false;
    }

    public void StartBurnCountdown()
    {
        burnCountdown = true;
    }

    void Update()
    {
        if (!isBurning) return;
        if (enemy == null) return;

        tickTimer += Time.deltaTime;
        if (tickTimer >= burnTickInterval)
        {
            tickTimer = 0f;
            enemy.TakeDamage(burnDamagePerTick, "DOT", 0);
        }

        if (burnCountdown)
        {
            burnTimer -= Time.deltaTime;
            if (burnTimer <= 0f)
            {
                StopBurnEffect();
            }
        }
    }

    void StartBurnEffect()
    {
        isBurning = true;
        burnCountdown = false;

        // Spawn burn effect if not existing
        if (burnEffectPrefab != null && burnEffectInstance == null)
        {
            burnEffectInstance = Instantiate(burnEffectPrefab, transform.position, Quaternion.identity);
            burnEffectInstance.transform.SetParent(this.transform, true);
        }
    }

    void StopBurnEffect()
    {
        isBurning = false;
        burnCountdown = false;

        // Destroy effect instance
        if (burnEffectInstance != null)
        {
            Destroy(burnEffectInstance);
            burnEffectInstance = null;
        }
    }
}
