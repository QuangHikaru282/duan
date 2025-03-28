using UnityEngine;

public class Skeleton : EnemyCore, IEnemy
{
    // Đầy đủ 8 state
    public PatrolState patrolState;
    public NavigateState navigateState;
    public IdleState idleState;
    public ChaseState chaseState;
    public SearchState searchState;
    public AttackState attackState;
    public HurtState hurtState;
    public DieState dieState;

    void Start()
    {
        // Khởi tạo các state gắn trong "behaviors"
        SetupInstances();

        // Cho quái bắt đầu ở PatrolState (hoặc Idle tuỳ ý)
        machine.Set(patrolState);
    }

    void Update()
    {
        // Nếu state hiện tại xong => tuỳ logic chuyển
        if (machine.state.isComplete)
        {
            // 1) Attack xong => quay về Patrol
            if (machine.state == attackState)
            {
                machine.Set(patrolState);
            }
            // 2) Hurt xong => quay về Patrol
            else if (machine.state == hurtState)
            {
                // Hoặc if HP <= 0 => Die, v.v.
                machine.Set(patrolState);
            }
            // 3) Chase xong => quay về Patrol (ví dụ)
            else if (machine.state == chaseState)
            {
                machine.Set(patrolState);
            }
            // 4) Search xong => quay về Chase (nếu logic “tìm xong => rượt tiếp”)
            else if (machine.state == searchState)
            {
                machine.Set(chaseState);
            }
            // 5) Navigate xong => quay về Patrol
            else if (machine.state == navigateState)
            {
                machine.Set(patrolState);
            }
            // 6) DieState thường không complete (nếu vanish?), 
            // nhưng nếu code cho isComplete => Destroy, 
            // ta không set state khác.
            else if (machine.state == dieState)
            {
                // Die => end
            }
            // 7) Idle xong => quay về Patrol
            else if (machine.state == idleState)
            {
                machine.Set(patrolState);
            }
        }

        // Cập nhật logic
        if (machine.state != null)
            machine.state.DoBranch();
    }

    void FixedUpdate()
    {
        if (machine.state != null)
            machine.state.FixedDoBranch();
    }

    // Hàm nhận sát thương
    public void TakeDamage(int damage, string damageType, int attackDirection)
    {
        Debug.Log("Skeleton nhận damage: " + damage);

        // Check HP <= 0 => Set(dieState, true); 
        // Hoặc code đơn giản: sang Hurt
        machine.Set(hurtState, true);
    }
}

