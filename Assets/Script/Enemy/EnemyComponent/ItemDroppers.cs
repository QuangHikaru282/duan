using UnityEngine;
using System.Collections.Generic;

public class ItemDropper : MonoBehaviour
{
    [Header("Item Drop Settings")]

    public List<GameObject> dropItemPrefabs;
    [Range(0f, 1f)]
    public float dropChance = 0.7f;
    public float spawnOffsetY = 0f;
    public float spreadRadius = 1.2f;
    private bool hasDropped = false;

    public virtual void DropItems()
    {
        if (hasDropped) return;
        hasDropped = true;

        if (dropItemPrefabs == null || dropItemPrefabs.Count == 0)
            return;

        if (Random.value > dropChance)
            return;

        int index = Random.Range(0, dropItemPrefabs.Count);
        GameObject itemPrefab = dropItemPrefabs[index];

        Vector2 randomOffset = Random.insideUnitCircle * spreadRadius;
        randomOffset.y = Mathf.Abs(randomOffset.y); // luôn dương để không spawn dưới đất

        Vector2 spawnPos = (Vector2)transform.position + new Vector2(randomOffset.x, spawnOffsetY + randomOffset.y);

        Instantiate(itemPrefab, spawnPos, Quaternion.identity);
    }
}
