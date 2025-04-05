using UnityEngine;

public class EnemyCore : MonoBehaviour
{
    [Header("Common Components")]
    public Rigidbody2D body;
    public Animator animator;
    public GroundSensor groundSensor;

    [Header("Player Reference")]
    public Transform player;
    public Vector2 lastKnownPosition;

    [Header("Shared States")]
    public IdleState idleState;
    public PatrolState patrolState;
    public NavigateState navigateState;
    public ChaseState chaseState;
    public SearchState searchState;
    public AttackState attackState;
    public HurtState hurtState;
    public DieState dieState;

    // State machine
    public StateMachine machine;
    public State state => machine.state;
    [HideInInspector] public float lastAttackTime = -Mathf.Infinity;

    void Start()
    {
        SetupInstances();
        machine.Set(idleState);
    }

    public void SetupInstances()
    {
        machine = new StateMachine();

        State[] allStates = GetComponentsInChildren<State>();
        foreach (var st in allStates)
        {
            bool hasSubMachine = st is ChaseState;
            st.SetCore(this, hasSubMachine);
        }
    }
    void Update()
    {
        if (machine.state != null)
        {
            if (machine.state.isComplete)
            {
                var next = machine.state.GetNextState();
                if (next != null)
                    machine.Set(next);
            }

            machine.state.DoBranch();
        }
    }


    void FixedUpdate()
    {
        if (machine.state != null)
            machine.state.FixedDoBranch();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying && state != null)
        {
            var activeStates = machine.GetActiveStateBranch();
            string path = "";
            foreach (var s in activeStates)
                path += s.GetType().Name + " > ";

            if (path.Length > 3)
                path = path.Substring(0, path.Length - 3);

            UnityEditor.Handles.Label(transform.position, "Active States: " + path);
        }
    }
#endif
}
