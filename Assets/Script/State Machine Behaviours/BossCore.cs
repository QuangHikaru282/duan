using UnityEngine;

public class BossCore : EnemyCore
{
    public int currentPhase = 1;
    public bool hasIntroStarted = false;

    public override void Start()
    {
        Debug.Log("[BossCore] Start() CALLED");
        InitCore();         // G�n player, HP, slider, canvas, v.v
        SetupInstances();   // Bind core v�o state
    }

    public virtual void EnterPhase(int phaseIndex)
    {
        currentPhase = phaseIndex;
    }

    public virtual bool IsInCombatPhase()
    {
        return hasIntroStarted;
    }
}
