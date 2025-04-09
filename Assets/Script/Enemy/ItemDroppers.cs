using UnityEngine;
using System.Collections.Generic;

public class ItemDropper : MonoBehaviour
{
    [Header("Item Drop Settings")]
    [Tooltip("Danh sách các prefab item có thể rớt. Chỉ có 1 item được rớt mỗi lần enemy chết.")]
    public List<GameObject> dropItemPrefabs;

    [Tooltip("Tỉ lệ rớt của enemy (từ 0 đến 1). Nếu Random.value <= dropChance, enemy sẽ rớt item.")]
    [Range(0f, 1f)]
    public float dropChance = 0.5f;

    [Tooltip("Offset theo trục Y để spawn item, đảm bảo item được spawn ở trên mặt ground.")]
    public float spawnOffset = 1f;

    /// <summary>
    /// Gọi hàm này khi enemy chết để kiểm tra và rớt item.
    /// </summary>
    public void DropItems()
    {
        // Nếu danh sách prefab rớt item trống thì không làm gì.
        if (dropItemPrefabs == null || dropItemPrefabs.Count == 0)
            return;

        // Kiểm tra tỉ lệ rớt.
        if (Random.value <= dropChance)
        {
            // Chọn ngẫu nhiên 1 prefab từ danh sách.
            int index = Random.Range(0, dropItemPrefabs.Count);
            GameObject itemPrefab = dropItemPrefabs[index];

            // Tính vị trí spawn: vị trí enemy cộng với offset theo trục Y.
            Vector3 spawnPosition = transform.position + Vector3.up * spawnOffset;

            // Instantiate item tại vị trí tính được, không có xoay (Quaternion.identity).
            Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
