using UnityEngine;

public class CameraPoint : MonoBehaviour
{
    [Header("Tham chiếu đến player")]
    // Gán đối tượng player qua Inspector
    public Transform player;

    [Header("Thiết lập di chuyển")]
    // Tốc độ di chuyển của CameraPoint khi chuyển động
    public float followSpeed = 6f;
    // Khoảng cách CameraPoint sẽ đi trước player khi player đang chạy
    public float leadDistance = 3f;
    // Ngưỡng tốc độ của player để xác định trạng thái "đang chạy"
    public float moveThreshold = 0.2f;

    void Start()
    {
        // Nếu CameraPoint đang là con của Player, tách ra để di chuyển độc lập
        if (transform.parent == player)
        {
            transform.SetParent(null);
        }
    }

    void Update()
    {
        if (player != null)
        {
            // Lấy vị trí hiện tại của player
            Vector3 playerPos = player.position;
            // Đảm bảo z của CameraPoint không thay đổi (để ổn định virtual camera)
            playerPos.z = transform.position.z;

            // Lấy component Rigidbody2D của player để kiểm tra vận tốc
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            Vector3 targetPos = playerPos;

            // Nếu player đang chạy (vận tốc theo x vượt ngưỡng) thì CameraPoint sẽ di chuyển về phía trước
            if (playerRb != null && Mathf.Abs(playerRb.velocity.x) > moveThreshold)
            {
                // Nếu chạy sang phải
                if (playerRb.velocity.x > moveThreshold)
                {
                    targetPos = playerPos + Vector3.right * leadDistance;
                }
                // Nếu chạy sang trái
                else if (playerRb.velocity.x < -moveThreshold)
                {
                    targetPos = playerPos + Vector3.left * leadDistance;
                }
            }

            // Di chuyển mượt CameraPoint từ vị trí hiện tại đến targetPos với tốc độ followSpeed
            transform.position = Vector3.MoveTowards(transform.position, targetPos, followSpeed * Time.deltaTime);
        }
    }
}
