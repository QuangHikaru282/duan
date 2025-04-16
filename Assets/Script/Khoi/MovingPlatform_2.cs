using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform_2 : MonoBehaviour
{
    public float speed = 2f; // Tốc độ di chuyển
    public float moveDistance = 3f; // Khoảng cách di chuyển lên xuống

    private Vector3 startPosA;

    void Start()
    {
        startPosA = transform.position; // Lưu vị trí ban đầu của A
    }

    void Update()
    {
        // Tính toán vị trí mới của A theo trục Y
        float offset = Mathf.PingPong(Time.time * speed, moveDistance * 2) - moveDistance;
        transform.position = new Vector3(startPosA.x, startPosA.y + offset, startPosA.z);
    }
}