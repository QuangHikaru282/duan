using UnityEngine;

public class BringerOfDeath : EnemyCore
{
    public BodSpellState bodSpellState;
    private LOSController los;

    void Awake()
    {
        los = GetComponent<LOSController>();
    }

    public void OnPlayerDetected()
    {
        if (state == bodSpellState || state.priority >= bodSpellState.priority)
            return;

         if (!los.isSeeingTarget)
            return;

        if (!bodSpellState.IsSpellReady())
            return;

        machine.TrySet(bodSpellState, true);
    }
}
