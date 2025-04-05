using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public State state;

    public bool TrySet(State newState, bool forceReset = false)
    {
        if (newState == null)
            return false;

        if (ReferenceEquals(state, newState) && !forceReset)
            return false;

        if (newState.forceInterrupt)
            return SetInternal(newState, forceReset);

        if (state == null || state.isComplete)
            return SetInternal(newState, forceReset);

        if (newState.priority >= state.priority)
            return SetInternal(newState, forceReset);

        return false;
    }

    private bool SetInternal(State newState, bool forceReset)
    {
        if (state != newState || forceReset)
        {
            state?.Exit();
            state = newState;
            state.Initialise(this);
            state.Enter();
        }
        return true;
    }

    public void Set(State newState, bool forceReset = false)
    {
        TrySet(newState, forceReset);
    }

    public List<State> GetActiveStateBranch(List<State> list = null)
    {
        list ??= new List<State>();
        if (state == null)
            return list;

        list.Add(state);
        if (state.machine != null)
            state.machine.GetActiveStateBranch(list);

        return list;
    }
}
