using UnityEngine;

public class IceGuardian : BossCore
{
    public IGIntroState introState;
    private LOSController los;

    void Awake()
    {
        los = GetComponent<LOSController>();
    }

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator != null)
            animator.enabled = false;
    }


    public void OnPlayerDetected()
    {
        Debug.Log("Goi dc ham!");

        if (introState == null || machine == null)
            return;

        if (state == introState)
            return;

        Debug.Log("Player detected by Ice Guardian!");

        machine.TrySet(introState, true);

    }

}
