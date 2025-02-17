using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    public GameObject imagePrefab;
    private GameObject imageA;
    private GameObject imageB;

    void Start()
    {
        // Lấy các Image hiện tại
        Transform parent = transform;
        imageA = parent.GetChild(0).gameObject;
        imageB = parent.GetChild(1).gameObject;
    }
    protected internal void AddImageHeart()
    {
        // Tạo Image mới từ Prefab
        GameObject newImage = Instantiate(imagePrefab, transform);

        // Xác định vị trí giữa a và b
        newImage.transform.SetSiblingIndex(1);
    }
}
