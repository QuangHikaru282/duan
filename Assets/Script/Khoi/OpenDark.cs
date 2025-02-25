using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;

public class OpenDark : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private float transparentAlpha = 0.3f; // Alpha khi Player bước vào
    [SerializeField] private float normalAlpha = 0.7f; // Alpha bình thường
    [SerializeField] private float fadeDuration = 0.5f; // Thời gian chuyển đổi

    private Coroutine fadeCoroutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeTilemapAlpha(transparentAlpha));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeTilemapAlpha(normalAlpha));
        }
    }

    private IEnumerator FadeTilemapAlpha(float targetAlpha)
    {
        if (tilemap != null)
        {
            float startAlpha = tilemap.color.a;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
                SetTilemapAlpha(newAlpha);
                yield return null;
            }

            SetTilemapAlpha(targetAlpha); // Đảm bảo alpha đạt giá trị chính xác sau khi kết thúc
        }
    }

    private void SetTilemapAlpha(float alpha)
    {
        Color currentColor = tilemap.color;
        tilemap.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
    }
}
