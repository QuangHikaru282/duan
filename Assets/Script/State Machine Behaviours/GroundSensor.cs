using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    public BoxCollider2D groundCheck;
    public LayerMask groundMask;      // layer cho Ground
    public LayerMask platformMask;    // layer cho Platform

    public bool grounded { get; private set; }

    void FixedUpdate()
    {
        CheckGround();
    }

    void CheckGround()
    {
        LayerMask combinedMask = groundMask | platformMask;

        // Check vùng
        grounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, combinedMask).Length > 0;
    }
}
