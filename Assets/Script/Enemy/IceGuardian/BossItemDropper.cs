using UnityEngine;
using System.Collections.Generic;

public class BossItemDropper : ItemDropper
{
    public override void DropItems()
    {
        if (dropItemPrefabs == null || dropItemPrefabs.Count == 0)
            return;

        if (Random.value > dropChance)
            return;

        int index = Random.Range(0, dropItemPrefabs.Count);
        GameObject itemPrefab = dropItemPrefabs[index];

        Vector2 randomOffset = Random.insideUnitCircle * spreadRadius;
        randomOffset.y = Mathf.Abs(randomOffset.y); // đảm bảo không spawn dưới đất

        Vector2 spawnPos = (Vector2)transform.position + new Vector2(randomOffset.x, spawnOffsetY + randomOffset.y);

        Instantiate(itemPrefab, spawnPos, Quaternion.identity);
    }
}
