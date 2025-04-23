using System.Collections;
using UnityEngine;
using TMPro;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public float spawnDuration = 15f;
    public float spawnInterval = 3f;
    public int monstersPerInterval = 2;
    public int maximumSpawn = 10;

    [Header("Spawn Area Settings")]
    public float spawnAreaWidth = 15f;
    public float spawnAreaHeight = 6f;
    public LayerMask groundLayer;

    [Header("Spawnable Monster Prefabs and Weights")]
    public GameObject[] monsterPrefabs;
    public float[] spawnWeights;

    [Header("UI Notification")]
    public GameObject monsterZoneUIPrefab;
    public float notificationDuration = 3f;

    private bool spawnerActivated = false;
    private Coroutine spawnRoutine;
    private int spawnedCount = 0;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!spawnerActivated && other.CompareTag("Player"))
        {
            spawnerActivated = true;
            StartCoroutine(ShowNotification());
            spawnRoutine = StartCoroutine(SpawnMonsters());
        }
    }

    IEnumerator ShowNotification()
    {
        GameObject notificationObj = Instantiate(monsterZoneUIPrefab);
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            notificationObj.transform.SetParent(canvas.transform, false);
        }
        yield return new WaitForSeconds(notificationDuration);
        Destroy(notificationObj);
    }

    IEnumerator SpawnMonsters()
    {
        float elapsed = 0f;
        while (elapsed < spawnDuration)
        {
            if (spawnedCount >= maximumSpawn)
            {
                break;
            }

            for (int i = 0; i < monstersPerInterval; i++)
            {
                if (spawnedCount >= maximumSpawn)
                    break;

                SpawnMonster();
                spawnedCount++;
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(spawnInterval);
            elapsed += spawnInterval;
        }
        Destroy(gameObject);
    }

    void SpawnMonster()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null || monsterPrefabs == null || monsterPrefabs.Length == 0)
            return;

        Vector2 spawnPos = GetRandomSpawnPosition();

        GameObject monsterPrefab = ChooseWeightedMonsterPrefab();
        GameObject monster = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
    }

    GameObject ChooseWeightedMonsterPrefab()
    {
        if (spawnWeights == null || spawnWeights.Length != monsterPrefabs.Length)
        {
            int index = Random.Range(0, monsterPrefabs.Length);
            return monsterPrefabs[index];
        }

        float totalWeight = 0f;
        foreach (float w in spawnWeights)
            totalWeight += w;

        float randomValue = Random.Range(0, totalWeight);
        float cumulative = 0f;
        for (int i = 0; i < monsterPrefabs.Length; i++)
        {
            cumulative += spawnWeights[i];
            if (randomValue <= cumulative)
                return monsterPrefabs[i];
        }

        return monsterPrefabs[monsterPrefabs.Length - 1];
    }

    Vector2 GetRandomSpawnPosition()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
            return transform.position;

        Vector2 playerPos = (Vector2)playerObj.transform.position;
        float minX = playerPos.x - spawnAreaWidth / 2f;
        float maxX = playerPos.x + spawnAreaWidth / 2f;
        float minY = playerPos.y - spawnAreaHeight / 2f;
        float maxY = playerPos.y + spawnAreaHeight / 2f;

        const int maxAttempts = 10;
        int attempts = 0;
        Vector2 candidate = playerPos;
        bool found = false;

        while (attempts < maxAttempts && !found)
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = playerPos.y;
            candidate = new Vector2(randomX, randomY);

            Collider2D col = Physics2D.OverlapPoint(candidate, LayerMask.GetMask("Ground", "MovingPlaform"));
            if (col != null)
            {
                if (candidate.y < col.bounds.max.y)
                    candidate.y = col.bounds.max.y + 0.5f;
            }

            if (Mathf.Abs(candidate.y - playerPos.y) <= spawnAreaHeight / 2f)
            {
                found = true;
                break;
            }
            attempts++;
        }
        return candidate;
    }
}
