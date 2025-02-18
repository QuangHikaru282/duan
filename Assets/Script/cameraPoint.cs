using UnityEngine;

public class CameraPoint : MonoBehaviour
{
    [Header("Tham chiếu đến player")]
    // Gán đối tượng player qua Inspector
    public Transform player;

    [Header("Thiết lập di chuyển")]
    // Tốc độ di chuyển của cameraPoint khi chuyển động
    public float followSpeed = 3f;
    // Khoảng cách cameraPoint sẽ đi trước player khi player đang chạy
    public float leadDistance = 3f;
    // Ngưỡng tốc độ của player để xác định trạng thái "đang chạy"
    public float moveThreshold = 0.1f;

    void Update()
    {
        if (player != null)
        {
            // Lấy vị trí hiện tại của player
            Vector3 playerPos = player.position;
            // Đảm bảo z của cameraPoint không thay đổi (để ổn định virtual camera)
            playerPos.z = transform.position.z;

            // Lấy component Rigidbody2D của player để kiểm tra vận tốc
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            Vector3 targetPos = playerPos;

            // Nếu player đang chạy (vận tốc theo x vượt ngưỡng) thì cameraPoint sẽ di chuyển về phía trước
            if (playerRb != null && Mathf.Abs(playerRb.velocity.x) > moveThreshold)
            {
                // Chỉ xét trường hợp chạy sang phải (trong game side-scroller thường hướng phải)
                if (playerRb.velocity.x > moveThreshold)
                {
                    targetPos = playerPos + Vector3.right * leadDistance;
                }
                // Nếu cần hỗ trợ chạy sang trái, có thể thêm điều kiện:
                else if (playerRb.velocity.x < -moveThreshold)
                {
                    targetPos = playerPos + Vector3.left * leadDistance;
                }
            }
            // Nếu player dừng lại, targetPos vẫn giữ nguyên là vị trí của player

            // Di chuyển mượt cameraPoint từ vị trí hiện tại đến targetPos với tốc độ followSpeed
            transform.position = Vector3.MoveTowards(transform.position, targetPos, followSpeed * Time.deltaTime);
        }
    }
}
