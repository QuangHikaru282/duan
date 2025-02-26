using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPlatform : MonoBehaviour
{
    public float increasedGravity = 5f; // Giá trị trọng lực khi chạm vào
    private float originalGravity; // Lưu trọng lực ban đầu

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Kiểm tra nếu là Player
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                originalGravity = rb.gravityScale; // Lưu trọng lực ban đầu
                rb.gravityScale = increasedGravity; // Tăng trọng lực
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Khi Player rời khỏi platform
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = originalGravity; // Trả về trọng lực ban đầu
            }
        }
    }

}
