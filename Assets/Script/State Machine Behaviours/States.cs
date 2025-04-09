using UnityEngine;

public enum StateExitReason
{
    None,
    NormalComplete,
    SawPlayer,
    LostPlayer
}

public abstract class State : MonoBehaviour
{
    [Header("Priority & Interrupt")]
    [SerializeField] protected int _priority = 0;
    [SerializeField] protected bool _forceInterrupt = false;

    public virtual int priority => _priority;
    public virtual bool forceInterrupt => _forceInterrupt;

    public bool isComplete { get; set; }
    public StateExitReason exitReason { get; set; } = StateExitReason.None;

    protected float startTime;
    public float time => Time.time - startTime;

    protected EnemyCore core;
    protected Rigidbody2D body => core.body;
    protected Animator animator => core.animator;

    public StateMachine machine;
    protected StateMachine parent;

    // Cung cấp truy cập nhanh đến Skeleton
    protected Skeleton skeleton => core as Skeleton; //can phai chu y them core moi khi them quai moi
    protected LOSController los => core.GetComponent<LOSController>();


    protected void Set(State newState, bool forceReset = false)
    {
        if (machine != null)
        {
            machine.TrySet(newState, forceReset);
        }
    }

    public virtual void SetCore(EnemyCore _core, bool createSubMachine = false)
    {
        core = _core;
        machine = createSubMachine ? new StateMachine() : null;
    }

    public void Initialise(StateMachine _parent)
    {
        parent = _parent;
        isComplete = false;
        exitReason = StateExitReason.None;
        startTime = Time.time;
    }

    public virtual void Enter() { }
    public virtual void Do() { }
    public virtual void FixedDo() { }
    public virtual void Exit() { }

    public void DoBranch()
    {
        Do();
        if (machine != null && machine.state != null)
        {
            machine.state.DoBranch();
        }
    }

    public void FixedDoBranch()
    {
        FixedDo();
        if (machine != null && machine.state != null)
        {
            machine.state.FixedDoBranch();
        }
    }

    public virtual State GetNextState() => null;
}
