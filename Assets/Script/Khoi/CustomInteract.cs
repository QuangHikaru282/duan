using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomInteract : Interact
{
    protected GameObject prefabToSpawn;
    protected GameObject spawnedObject; 
    protected bool isColliding = false; 
    protected bool isSpawning = false;

    // Ghi ?� h�m Update ?? th�m h�nh vi m?i
    protected override void Update()
    {
        if (isColliding && Input.GetKeyDown(KeyCode.F))
        {
            RemoveSpawnedObject();
            isSpawning = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Xóa prefab khi thoát khỏi va chạm
        RemoveSpawnedObject();
        isColliding = false; // Đánh dấu không còn va chạm
    }

    protected override void RemoveSpawnedObject()
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
            spawnedObject = null;
        }
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {

        if (prefabToSpawn != null && !isSpawning)
        {

            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
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
}

