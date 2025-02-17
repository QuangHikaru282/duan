using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Vibrate : MonoBehaviour
{
    [SerializeField]
    private GameObject secondaryObject;
    [SerializeField]
    private Tilemap tilemap;
    [SerializeField]
    private float shakeDuration = 0.3f; // Thời gian rung
    [SerializeField]
    private float shakeMagnitude = 0.1f; // Độ mạnh của rung

    private Vector3 originalPositionCurrent;
    private Vector3 originalPositionSecondary;

   
    [SerializeField]
    private GameObject Ground;
    [SerializeField]
    private Sprite newSprite;

    public float duration = 0.2f; // Thời gian để alpha giảm
    private Color targetColor = new Color(1f, 1f, 1f, 230f / 255f); // Màu đích với alpha = 233
    private Color initialColor;
    void Start()
    {
        // Lưu vị trí ban đầu của GameObject
        originalPositionCurrent = transform.localPosition;
        if (secondaryObject != null)
        {
            originalPositionSecondary = secondaryObject.transform.localPosition;
        }
        // Lưu màu sắc ban đầu của Tilemap
        initialColor = tilemap.color;
    }
    protected internal System.Collections.IEnumerator Shake()
    {
        // Lấy component SpriteRenderer
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        // Thay đổi sprite
        spriteRenderer.sprite = newSprite;
        yield return new WaitForSeconds(0.3f);

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // Rung đối tượng hiện tại
            float offsetX = Random.Range(-shakeMagnitude, shakeMagnitude);
            float offsetY = Random.Range(-shakeMagnitude, shakeMagnitude);
            transform.localPosition = new Vector3(originalPositionCurrent.x + offsetX, originalPositionCurrent.y + offsetY, originalPositionCurrent.z);

            // Rung đối tượng khác (nếu có)
            if (secondaryObject != null)
            {
                float secondaryOffsetX = Random.Range(-shakeMagnitude, shakeMagnitude);
                float secondaryOffsetY = Random.Range(-shakeMagnitude, shakeMagnitude);
                secondaryObject.transform.localPosition = new Vector3(originalPositionSecondary.x + secondaryOffsetX, originalPositionSecondary.y + secondaryOffsetY, originalPositionSecondary.z);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Trả về vị trí ban đầu của cả hai đối tượng
        transform.localPosition = originalPositionCurrent;
        if (secondaryObject != null)
        {
            secondaryObject.transform.localPosition = originalPositionSecondary;
        }

        float elapsedTime = 0f;
        Destroy(Ground, 0.3f);
        spriteRenderer.enabled = false;
        // Chạy Coroutine trong khoảng thời gian nhất định

        yield return new WaitForSeconds(1f);
        while (elapsedTime < duration)
        {
            // Tính toán giá trị alpha hiện tại (giảm từ 255 xuống 233)
            float currentAlpha = Mathf.Lerp(initialColor.a, targetColor.a, elapsedTime / duration);
            // Thay đổi màu sắc của Tilemap
            tilemap.color = new Color(initialColor.r, initialColor.g, initialColor.b, currentAlpha);

            // Cập nhật thời gian đã trôi qua
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Đảm bảo Tilemap có màu chính xác khi kết thúc Coroutine
        tilemap.color = targetColor;

        Destroy(gameObject, 0.2f);
        // Thực hiện trì hoãn 2 giây trước khi cập nhật màu của Tilemap

    }
}
