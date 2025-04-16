using UnityEngine;

public class IceGuardian : EnemyCore
{
    [Header("Private States")]
    public IGIntroState introState;
    public IGSkill1State skill1State;
    public IGSkill2State skill2State;
    public IGPhase2State phase2State;
    public StaggerState staggerState;
    private float lastStaggerHPThreshold = 1f;


    private LOSController los;
    private bool hasEnteredPhase2 = false;

    public override void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator != null)
            animator.enabled = false;

        InitCore();
        SetupInstances();
    }

    void Awake()
    {
        los = GetComponent<LOSController>();
    }

    public override void Update()
    {
        base.Update();

        if (!hasEnteredPhase2 && hpSlider != null && hpSlider.value <= maxHP * 0.5f)
        {
            if (phase2State != null && machine != null)
            {
                machine.TrySet(phase2State, true);
                hasEnteredPhase2 = true;
                return;
            }
        }

        if (staggerState != null && machine != null && hpSlider != null)
        {
            float currentHPPercent = hpSlider.value / maxHP;
            if (currentHPPercent <= lastStaggerHPThreshold - 0.25f)
            {
                lastStaggerHPThreshold -= 0.25f;
                machine.TrySet(staggerState, true);
                return;
            }
        }

        if (skill1State == null || machine == null || !los.isSeeingTarget)
            return;

        if (state != null && (state == skill1State || state.priority >= skill1State.priority))
            return;

        if (!skill1State.IsCooldownReady())
            return;

        if (!skill1State.IsPlayerInTriggerRange())
            return;

        machine.TrySet(skill1State, true);
    }

    public void OnPlayerDetected()
    {
        if (introState == null || !los.isSeeingTarget || machine == null)
            return;

        if (state != null && (state == introState || state.priority >= introState.priority))
            return;

        if (hpCanvasObj != null && !hpCanvasObj.activeSelf)
            hpCanvasObj.SetActive(true);
        hpTimer = 0f;

        machine.TrySet(introState, true);
    }

    public void OnSkill2Detected()
    {
        if (!hasEnteredPhase2 || skill2State == null || machine == null)
            return;

        if (state != null && (state == skill2State || state.priority >= skill2State.priority))
            return;

        if (!skill2State.IsCooldownReady())
            return;

        machine.TrySet(skill2State, true);
    }
}
