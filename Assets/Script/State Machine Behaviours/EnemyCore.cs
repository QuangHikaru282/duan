using UnityEngine;

public class EnemyCore : MonoBehaviour
{
    // Các thành phần chung (blackboard)
    public Rigidbody2D body;
    public Animator animator;
    public GroundSensor groundSensor;

    // State machine quản lý các state của đối tượng
    public StateMachine machine;

    // Wrapper để truy cập state hiện tại
    public State state => machine.state;

    [Header("Player Reference")]
    public Transform player;
    public Vector2 lastKnownPosition;

    void Start()
    {
        // Khởi tạo machine và gán cho các state con
        SetupInstances();

        // Tuỳ ý: Chọn state khởi đầu (vd: ChaseState, PatrolState,...)
        // Ở đây ví dụ load sẵn "ChaseState"
        ChaseState chase = GetComponentInChildren<ChaseState>();
        if (chase != null)
        {
            machine.Set(chase);
        }
        // Hoặc bạn có thể Set một state khác
    }

    // Thiết lập EnemyCore cho tất cả các state con
    public void SetupInstances()
    {
        machine = new StateMachine();

        // Lấy tất cả State trong children
        State[] allChildStates = GetComponentsInChildren<State>();
        foreach (State st in allChildStates)
        {
            // Nếu là ChaseState => tạo sub-machine
            if (st is ChaseState)
            {
                st.SetCore(this, true);
            }
            else
            {
                st.SetCore(this, false);
            }
        }
    }

    void Update()
    {
        if (machine.state != null)
        {
            machine.state.DoBranch();
        }
    }

    void FixedUpdate()
    {
        if (machine.state != null)
        {
            machine.state.FixedDoBranch();
        }
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (Application.isPlaying && state != null)
        {
            var activeStates = machine.GetActiveStateBranch();
            string stateNames = "";
            foreach (var s in activeStates)
            {
                stateNames += s.GetType().Name + " > ";
            }
            if (stateNames.Length > 3)
                stateNames = stateNames.Substring(0, stateNames.Length - 3);
            UnityEditor.Handles.Label(transform.position, "Active States: " + stateNames);
        }
#endif
    }
}
