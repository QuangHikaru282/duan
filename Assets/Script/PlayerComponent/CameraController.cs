using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera vCam;
    public float lookOffset = 2f;        // Khoảng cách nhìn lên/xuống khi nhấn phím
    public float transitionSpeed = 2f;   // Tốc độ chuyển đổi vị trí camera

    private CinemachineFramingTransposer framingTransposer;
    private float originalYOffset;
    private float targetYOffset;

    void Start()
    {
        if (vCam != null)
        {
            framingTransposer = vCam.GetCinemachineComponent<CinemachineFramingTransposer>();
            originalYOffset = framingTransposer.m_TrackedObjectOffset.y;
            targetYOffset = originalYOffset;
        }
    }

    void Update()
    {
        if (framingTransposer != null)
        {
            
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                targetYOffset = originalYOffset + lookOffset;
            }
            
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                targetYOffset = originalYOffset - lookOffset;
            }
            else
            {
                targetYOffset = originalYOffset;
            }

            // Thay đổi giá trị YOffset một cách mượt mà
            float newYOffset = Mathf.Lerp(framingTransposer.m_TrackedObjectOffset.y, targetYOffset, Time.deltaTime * transitionSpeed);
            framingTransposer.m_TrackedObjectOffset = new Vector3(framingTransposer.m_TrackedObjectOffset.x, newYOffset, framingTransposer.m_TrackedObjectOffset.z);
        }
    }
}
