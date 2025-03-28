using UnityEngine;

/// <summary>
/// Lớp trừu tượng đại diện cho một State trong mô hình HSM.
/// </summary>
public abstract class State : MonoBehaviour
{
    [Header("Priority & Interrupt")]
    [Tooltip("Độ ưu tiên của state. State mới với priority >= sẽ interrupt state hiện tại.")]
    [SerializeField] protected int _priority = 0;

    [Tooltip("Nếu true, state này sẽ chen ngang bất kỳ state nào.")]
    [SerializeField] protected bool _forceInterrupt = false;

    public virtual int priority => _priority;
    public virtual bool forceInterrupt => _forceInterrupt;

    public bool isComplete { get; set; }

    protected float startTime;
    public float time => Time.time - startTime;

    protected EnemyCore core;
    protected Rigidbody2D body => core.body;
    protected Animator animator => core.animator;

    public StateMachine machine;

    protected StateMachine parent;


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
        startTime = Time.time;
    }

    // Vòng đời cơ bản
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
}
