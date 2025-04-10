using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField]
    private float speed; // Tốc độ di chuyển
    [SerializeField]
    private float resetPosition; // Vị trí khi hình chữ nhật quay lại
    [SerializeField]
    private float startPosition = 0; // Vị trí bắt đầu
    [SerializeField]
    private bool onLeft;
    [SerializeField]
    private bool onRight;

    void Update()
    {
        if (onLeft)
        {
            resetPosition = -25;
            transform.Translate(Vector3.left * speed * Time.deltaTime);
            if (transform.position.x <= resetPosition)
            {
                // Đặt lại vị trí về đầu
                transform.position = new Vector3(startPosition, transform.position.y, transform.position.z);
            }
        }
        if (onRight)
        {
            resetPosition = 25;
            transform.Translate(Vector3.right * speed * Time.deltaTime);
            if (transform.position.x >= resetPosition)
            {
                // Đặt lại vị trí về đầu
                transform.position = new Vector3(startPosition, transform.position.y, transform.position.z);
            }
        }
    }
}
