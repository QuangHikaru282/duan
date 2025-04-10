using UnityEngine;

public class LOSSettings : MonoBehaviour
{
    public float detectRange = 5f;
    [Range(0f, 360f)] public float visionAngle = 180f;
    public LayerMask obstacleMask;
}
