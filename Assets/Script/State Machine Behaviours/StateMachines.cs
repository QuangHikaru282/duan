using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý logic chuyển state, dùng TrySet() để so sánh priority, forceInterrupt, isComplete.
/// </summary>
public class StateMachine
{
    public State state; // State hiện tại


    public bool TrySet(State newState, bool forceReset = false)
    {
        if (state == null)
        {
            Set(newState, forceReset);
            return true;
        }

        // 1) Force interrupt
        if (newState.forceInterrupt)
        {
            Set(newState, forceReset);
            return true;
        }

        // 2) Current isComplete
        if (state.isComplete)
        {
            Set(newState, forceReset);
            return true;
        }

        // 3) So sánh priority
        if (newState.priority >= state.priority)
        {
            Set(newState, forceReset);
            return true;
        }

        return false;
    }


    public void Set(State newState, bool forceReset = false)
    {
        if (state != newState || forceReset)
        {
            if (state != null)
            {
                state.Exit();
            }
            state = newState;
            state.Initialise(this);
            state.Enter();
        }
    }


    public List<State> GetActiveStateBranch(List<State> list = null)
    {
        if (list == null)
        {
            list = new List<State>();
        }
        if (state == null)
            return list;

        list.Add(state);

        // Nếu state này có machine con
        if (state.machine != null)
        {
            state.machine.GetActiveStateBranch(list);
        }

        return list;
    }
}
