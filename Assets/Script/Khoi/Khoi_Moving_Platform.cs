using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Khoi_Moving_Platform : MonoBehaviour
{
    public float speed = 2f; // Tốc độ di chuyển
    public float moveDistance = 3f; // Khoảng cách di chuyển lên xuống
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position; // Lưu vị trí ban đầu
    }

    void Update()
    {
        // Tính toán vị trí mới theo trục Y
        float newY = startPos.y + Mathf.PingPong(Time.time * speed, moveDistance * 2) - moveDistance;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}
