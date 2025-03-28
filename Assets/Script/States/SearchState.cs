using UnityEngine;

public class SearchState : State
{

    [Header("Search Settings")]
    [Tooltip("Thời gian search trước khi quái bỏ cuộc.")]
    public float searchDuration = 3f;
    private float searchTimer = 0f;

    [Tooltip("Phạm vi quái 'thấy lại' player khi đang search.")]
    public float detectRangeDuringSearch = 3f;

    [Tooltip("Góc tầm nhìn trong search (0 = không check).")]
    public float visionAngle = 0f;

    [Tooltip("Layer obstacles để raycast.")]
    public LayerMask obstacleMask;

    public override void Enter()
    {
        base.Enter();
        isComplete = false;
        searchTimer = 0f;
    }

    public override void Do()
    {
        base.Do();

        searchTimer += Time.deltaTime;

        // 1) Thử check line-of-sight lại
        bool canSee = Helpers.CheckLineOfSight2D(
            from: core.transform,
            target: core.player,
            maxRange: detectRangeDuringSearch,
            obstacleMask: obstacleMask,
            maxAngle: visionAngle,
            debugRay: true
        );

        if (canSee)
        {
            // Thấy lại player => quái 'xong' search => isComplete => quay lên cha
            isComplete = true;
            return;
        }

        if (searchTimer >= searchDuration)
        {
            isComplete = true;
        }
    }

    public override void Exit()
    {
        base.Exit();
        // Reset logic
    }
}
