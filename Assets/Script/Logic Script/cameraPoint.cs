using UnityEngine;
using Cinemachine;

public class CameraPoint : MonoBehaviour
{
    [Header("Tham chiếu Player")]
    // Gán đối tượng player qua Inspector
    public Transform player;
    // (Nếu cần theo dõi vận tốc, bạn cũng có thể gán Rigidbody2D của player)
    public Rigidbody2D playerRb;

    [Header("Thiết lập Follow & Lead")]
    // Tốc độ di chuyển (smoothing) của CameraPoint
    public float followSpeed = 6f;
    // Khoảng cách lead (đi trước) khi player chạy
    public float leadDistance = 3f;
    // Ngưỡng tốc độ của player để xác định trạng thái chạy
    public float moveThreshold = 0.2f;

    [Header("Dynamic Damping Settings")]
    // Tham số damping mặc định (điều chỉnh qua Inspector của Virtual Camera)
    public float normalXDamping = 2f;
    public float normalYDamping = 2f;
    // Tham số damping khi player chuyển động đột biến (giảm damping để camera nhanh hơn)
    public float fastXDamping = 0.5f;
    public float fastYDamping = 0.5f;
    // Ngưỡng vận tốc (theo trục X) để xác định chuyển động đột biến
    public float abruptVelocityThreshold = 10f;

    [Header("Cinemachine Reference")]
    // Gán Cinemachine Virtual Camera qua Inspector
    public CinemachineVirtualCamera virtualCamera;

    // Lưu giá trị damping mặc định để phục hồi
    private float currentXDamping;
    private float currentYDamping;

    void Start()
    {
        // Nếu CameraPoint được đặt làm con của player, tách ra
        /*if (transform.parent == player)
        {
            transform.SetParent(null);
        }*/

        // Kiểm tra tham chiếu
        if (player == null)
        {
            Debug.LogError("CameraPoint: Player reference is not assigned!");
        }
        if (playerRb == null && player != null)
        {
            playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb == null)
            {
                Debug.LogError("CameraPoint: Could not find Rigidbody2D on Player!");
            }
        }
        if (virtualCamera == null)
        {
            //Debug.LogError("CameraPoint: VirtualCamera reference is not assigned!");
        }
        else
        {
            // Lấy damping ban đầu từ CinemachineFramingTransposer của Virtual Camera
            CinemachineFramingTransposer transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            if (transposer != null)
            {
                currentXDamping = transposer.m_XDamping;
                currentYDamping = transposer.m_YDamping;
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        // Tính toán vị trí target cho CameraPoint
        Vector3 playerPos = player.position;
        // Giữ trục z của CameraPoint để không thay đổi
        playerPos.z = transform.position.z;

        // Lấy player velocity (nếu có) để tính lead
        float playerVelX = (playerRb != null) ? playerRb.velocity.x : 0f;
        Vector3 targetPos = playerPos;

        // Nếu player đang chạy (vận tốc vượt ngưỡng), thêm lead theo hướng
        if (Mathf.Abs(playerVelX) > moveThreshold)
        {
            if (playerVelX > 0)
            {
                targetPos += Vector3.right * leadDistance;
            }
            else if (playerVelX < 0)
            {
                targetPos += Vector3.left * leadDistance;
            }
        }

        // Di chuyển mượt CameraPoint từ vị trí hiện tại đến targetPos
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

        // --- Dynamic Damping Adjustment ---
        if (virtualCamera != null)
        {
            CinemachineFramingTransposer transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            if (transposer != null)
            {
                // Nếu player đang chuyển động đột biến, giảm damping
                if (Mathf.Abs(playerVelX) > abruptVelocityThreshold)
                {
                    transposer.m_XDamping = fastXDamping;
                    transposer.m_YDamping = fastYDamping;
                }
                else
                {
                    // Phục hồi damping dần dần về giá trị mặc định
                    transposer.m_XDamping = Mathf.Lerp(transposer.m_XDamping, normalXDamping, 5f * Time.deltaTime);
                    transposer.m_YDamping = Mathf.Lerp(transposer.m_YDamping, normalYDamping, 5f * Time.deltaTime);
                }
            }
        }
    }
}
