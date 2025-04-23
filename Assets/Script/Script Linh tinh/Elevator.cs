using UnityEngine;

public class Elevator : MonoBehaviour
{
    public float initialSpeed = 0.5f;
    public float maxSpeed = 5f;
    public float accelerationTime = 1f;
    public float distance = 5f;

    private Vector3 startPosition;
    private float elapsedTime = 0f;
    private bool isMoving = false;
    private bool isActivated = false;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (!isMoving) return;

        elapsedTime += Time.deltaTime;
        float currentSpeed = Mathf.Lerp(initialSpeed, maxSpeed, Mathf.Clamp01(elapsedTime / accelerationTime));
        transform.position += Vector3.up * currentSpeed * Time.deltaTime;
    }

    public void Activate()
    {
        if (isActivated) return;
        isActivated = true;
        isMoving = true;
        elapsedTime = 0f;
    }
}
