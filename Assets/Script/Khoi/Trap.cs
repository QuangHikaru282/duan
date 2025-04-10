using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private Vector2 startPosition;
    [SerializeField] private bool resetRotation = false;
    private float startRotation;
    private Rigidbody2D rb; // Thêm tham chiếu đến Rigidbody2D
    [SerializeField]
    private float time;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation.eulerAngles.z;
        rb = GetComponent<Rigidbody2D>(); // Lấy component Rigidbody2D
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(ResetWithKinematic()); // Bắt đầu coroutine khi va chạm
    }

    // Coroutine để xử lý reset với kinematic
    private IEnumerator ResetWithKinematic()
    {

        yield return new WaitForSeconds(time);
        // Đặt vị trí về ban đầu
        transform.position = startPosition;

        // Reset rotation nếu được bật
        if (resetRotation)
        {
            transform.rotation = Quaternion.Euler(0, 0, startRotation);
        }

        // Chuyển sang kinematic
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        

        // Chuyển về trạng thái bình thường
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }

    public void ResetToStart()
    {
        StartCoroutine(ResetWithKinematic()); // Sử dụng cùng logic cho reset thủ công
    }
}