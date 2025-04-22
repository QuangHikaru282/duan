using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;
    //==================================

    public CinemachineVirtualCamera virtualCamera;

    private float originalSoftZoneWidth;
    private float originalDeadZoneWidth;

    public bool isModified = false;

    private void Awake()
    {
        Instance = this;
    }

    // Gọi hàm này nếu muốn phục hồi lại giá trị ban đầu
    public void RestoreZoneSettings()
    {
        if (isModified && virtualCamera != null)
        {
            var composer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            if (composer != null)
            {
                composer.m_SoftZoneWidth = originalSoftZoneWidth;
                composer.m_DeadZoneWidth = originalDeadZoneWidth;
                isModified = false;
            }
        }
    }
    public IEnumerator Shake(float duration, float magnitude)
    {
        if (virtualCamera != null)
        {
            var composer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            if (composer != null)
            {
                // Lưu giá trị gốc
                originalSoftZoneWidth = composer.m_SoftZoneWidth;
                originalDeadZoneWidth = composer.m_DeadZoneWidth;

                composer.m_SoftZoneWidth = 0f;

                isModified = true;
            }
        }
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0f);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }

    public void TriggerShake(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }
}
