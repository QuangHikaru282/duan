using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Spawnable Monster Prefabs")]
    public GameObject[] monsterPrefabs;

    [Header("UI Notification")]
    public GameObject monsterZoneUIPrefab;
    public float notificationDuration = 3f;

    [Header("Blocker Colliders")]
    public GameObject leftBlocker;
    public GameObject rightBlocker;

    private bool spawnerActivated = false;
    private Coroutine spawnRoutine;
    private List<GameObject> spawnedMonsters = new List<GameObject>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!spawnerActivated && other.CompareTag("Player"))
        {
            spawnerActivated = true;
            if (leftBlocker != null) leftBlocker.SetActive(true);
            if (rightBlocker != null) rightBlocker.SetActive(true);

            StartCoroutine(ShowNotification());
            spawnRoutine = StartCoroutine(SpawnMonsters());
        }
    }

    IEnumerator ShowNotification()
    {
        GameObject notificationObj = Instantiate(monsterZoneUIPrefab);
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
            notificationObj.transform.SetParent(canvas.transform, false);

        yield return new WaitForSeconds(notificationDuration);
        Destroy(notificationObj);
    }

    IEnumerator SpawnMonsters()
    {
        float elapsed = 0f;
        int spawnedCount = 0;

        while (elapsed < spawnDuration && spawnedCount < maximumSpawn)
        {
            for (int i = 0; i < monstersPerInterval && spawnedCount < maximumSpawn; i++)
            {
                GameObject monster = SpawnMonster();
                if (monster != null)
                    spawnedMonsters.Add(monster);

                spawnedCount++;
                yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitForSeconds(spawnInterval);
            elapsed += spawnInterval;
        }

        StartCoroutine(CheckMonstersDefeated());
    }

    IEnumerator CheckMonstersDefeated()
    {
        while (true)
        {
            spawnedMonsters.RemoveAll(m => m == null);
            if (spawnedMonsters.Count == 0)
                break;

            yield return new WaitForSeconds(0.5f);
        }

        if (leftBlocker != null) leftBlocker.SetActive(false);
        if (rightBlocker != null) rightBlocker.SetActive(false);

        Destroy(gameObject);
    }

    GameObject SpawnMonster()
    {
        if (monsterPrefabs == null || monsterPrefabs.Length == 0)
            return null;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
            return null;

        Vector2 spawnPos = GetRandomSpawnPosition(playerObj.transform.position);

        int index = Random.Range(0, monsterPrefabs.Length);
        return Instantiate(monsterPrefabs[index], spawnPos, Quaternion.identity);
    }

    Vector2 GetRandomSpawnPosition(Vector2 playerPos)
    {
        float minX = playerPos.x - spawnAreaWidth / 2f;
        float maxX = playerPos.x + spawnAreaWidth / 2f;
        float minY = playerPos.y - spawnAreaHeight / 2f;
        float maxY = playerPos.y + spawnAreaHeight / 2f;

        const int maxAttempts = 10;
        int attempts = 0;
        Vector2 candidate = playerPos;

        while (attempts < maxAttempts)
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = playerPos.y;
            candidate = new Vector2(randomX, randomY);

            Collider2D col = Physics2D.OverlapPoint(candidate, LayerMask.GetMask("Ground", "MovingPlaform"));
            if (col != null)
            {
                candidate.y = col.bounds.max.y + 0.5f;
                break;
            }

            attempts++;
        }

        return candidate;
    }
}
