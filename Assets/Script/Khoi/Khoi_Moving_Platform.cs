using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Khoi_Moving_Platform : MonoBehaviour
{
    public float speed = 2f; // Tốc độ di chuyển
    public float moveDistance = 3f; // Khoảng cách di chuyển lên xuống
    public Transform objectB; // Tham chiếu đến GameObject B
    public Transform objectC; // Tham chiếu đến GameObject C

    private Vector3 startPosA;
    private Vector3 startPosB;
    private Vector3 startPosC;

    void Start()
    {
        startPosA = transform.position; // Lưu vị trí ban đầu của A
        if (objectB != null) startPosB = objectB.position; // Lưu vị trí ban đầu của B
        if (objectC != null) startPosC = objectC.position; // Lưu vị trí ban đầu của C
    }

    void Update()
    {
        // Tính toán vị trí mới của A theo trục Y
        float offset = Mathf.PingPong(Time.time * speed, moveDistance * 2) - moveDistance;
        transform.position = new Vector3(startPosA.x, startPosA.y + offset, startPosA.z);

        // Nếu có B và C, di chuyển chúng theo hướng ngược lại
        if (objectB != null)
        {
            objectB.position = new Vector3(startPosB.x, startPosB.y - offset, startPosB.z);
        }
        if (objectC != null)
        {
            objectC.position = new Vector3(startPosC.x, startPosC.y - offset, startPosC.z);
        }
    }
}