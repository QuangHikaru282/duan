﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    [SerializeField]
    private GameObject prefabToSpawn;
    private GameObject spawnedObject; // Lưu trữ tham chiếu đến prefab đã được tạo
    [SerializeField]
    private bool isColliding = false; // Biến kiểm tra trạng thái va chạm
    [SerializeField]
    private bool isSpawning = false;
    [SerializeField]
    private Vibrate Vibrate;
    [SerializeField]
    private ParticleSystem particleSystem;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("aaaa");
        // Kiểm tra nếu prefab đã được gán
        if (prefabToSpawn != null && !isSpawning)
        {
            Debug.Log("bbbb");
            // Lấy BoxCollider2D từ đối tượng hiện tại
            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            Debug.Log("cccc");
            if (boxCollider != null)
            {
                // Tính toán vị trí trung tâm của BoxCollider2D
                Vector3 spawnPosition = boxCollider.bounds.center;

                // Hiển thị prefab tại vị trí trung tâm và lưu tham chiếu
                spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                isColliding = true; // Đánh dấu đang va chạm
            }
            else
            {
                Debug.LogWarning("Không tìm thấy BoxCollider2D trên GameObject này!");
            }
        }
        else
        {
            Debug.LogWarning("Prefab chưa được gán!");
        }
    }
    private void Update()
    {
        if (isColliding && Input.GetKeyDown(KeyCode.F))
        {
            RemoveSpawnedObject();
            particleSystem.Play();
            StartCoroutine(Vibrate.Shake());
            isSpawning = true;
            Debug.LogWarning("oke");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Xóa prefab khi thoát khỏi va chạm
        RemoveSpawnedObject();
        isColliding = false; // Đánh dấu không còn va chạm
    }

    private void RemoveSpawnedObject()
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
            spawnedObject = null;
        }
    }
}
