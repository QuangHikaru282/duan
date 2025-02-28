using UnityEngine;

public class MoveUp : MonoBehaviour
{
    public float initialSpeed = 0.5f; // Tốc độ ban đầu
    public float maxSpeed = 5f; // Tốc độ tối đa
    public float accelerationTime = 1f; // Thời gian tăng tốc
    public float distance = 5f; // Khoảng cách cần di chuyển

    private Vector3 startPosition;
    private bool movingUp = true;
    private float elapsedTime = 0f;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (movingUp)
        {
            elapsedTime += Time.deltaTime;
            float currentSpeed = Mathf.Lerp(initialSpeed, maxSpeed, Mathf.Clamp01(elapsedTime / accelerationTime));

            transform.position += Vector3.up * currentSpeed * Time.deltaTime;

            if (Vector3.Distance(startPosition, transform.position) >= distance)
            {
                movingUp = false; // Dừng di chuyển sau khi đạt khoảng cách
            }
        }
    }
}