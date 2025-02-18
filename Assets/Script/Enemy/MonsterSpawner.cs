using System.Collections;
using UnityEngine;
using TMPro;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Thời gian spawner sẽ hoạt động (s)")]
    public float spawnDuration = 15f;
    [Tooltip("Khoảng thời gian giữa các chu kỳ spawn (s)")]
    public float spawnInterval = 3f;
    [Tooltip("Số lượng monster spawn ra trong mỗi chu kỳ")]
    public int monstersPerInterval = 2;
    [Tooltip("Số lượng tối đa monster được spawn ra từ spawner này")]
    public int maximumSpawn = 10;

    [Header("Spawn Area Settings")]
    [Tooltip("Chiều rộng của vùng spawn quanh player (dựa trên vị trí của player làm center)")]
    public float spawnAreaWidth = 15f;
    [Tooltip("Chiều cao của vùng spawn quanh player")]
    public float spawnAreaHeight = 6f;

    [Header("Spawnable Monster Prefabs and Weights")]
    [Tooltip("Danh sách các prefab monster có thể spawn (ví dụ: bat, goblin, witch, …)")]
    public GameObject[] monsterPrefabs;
    [Tooltip("Trọng số tương ứng cho từng monster prefab (để điều chỉnh tỉ lệ spawn). Các mảng phải có cùng độ dài với monsterPrefabs.")]
    public float[] spawnWeights;

    [Header("UI Notification")]
    [Tooltip("Prefab MonsterZoneUI đã được thiết lập (có image và TMP)")]
    public GameObject monsterZoneUIPrefab;
    [Tooltip("Thời gian hiển thị thông báo UI (s)")]
    public float notificationDuration = 3f;

    // Nội bộ
    private bool spawnerActivated = false;
    private Coroutine spawnRoutine;
    private int spawnedCount = 0;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!spawnerActivated && other.CompareTag("Player"))
        {
            spawnerActivated = true;
            // Hiển thị thông báo MonsterZoneUI
            StartCoroutine(ShowNotification());
            // Bắt đầu spawn monster
            spawnRoutine = StartCoroutine(SpawnMonsters());
        }
    }

    IEnumerator ShowNotification()
    {
        GameObject notificationObj = Instantiate(monsterZoneUIPrefab);
        // Đảm bảo đặt vào Canvas (thay đổi tên "Canvas" nếu cần)
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
            // Kiểm tra số lượng đã spawn
            if (spawnedCount >= maximumSpawn)
            {
                break;
            }

            for (int i = 0; i < monstersPerInterval; i++)
            {
                // Nếu đạt maximumSpawn thì dừng spawn
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

        // Tính vị trí spawn an toàn xung quanh player
        Vector2 spawnPos = GetRandomSpawnPosition();
        // Chọn prefab theo trọng số
        GameObject monsterPrefab = ChooseWeightedMonsterPrefab();
        Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
    }

    // Hàm lựa chọn monster prefab dựa trên trọng số
    GameObject ChooseWeightedMonsterPrefab()
    {
        // Nếu không có mảng spawnWeights hoặc độ dài không khớp, chọn ngẫu nhiên đơn giản
        if (spawnWeights == null || spawnWeights.Length != monsterPrefabs.Length)
        {
            int index = Random.Range(0, monsterPrefabs.Length);
            return monsterPrefabs[index];
        }

        // Tính tổng trọng số
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

        // Fallback
        return monsterPrefabs[monsterPrefabs.Length - 1];
    }

    // Hàm lấy vị trí spawn an toàn xung quanh Player.
    // Nếu candidate rơi vào collider của Ground/MovingPlaform và candidate.y < col.bounds.max.y thì điều chỉnh candidate.y.
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
            float randomY = Random.Range(minY, maxY);
            candidate = new Vector2(randomX, randomY);

            // Kiểm tra nếu candidate nằm trong collider của Ground/MovingPlaform
            Collider2D col = Physics2D.OverlapPoint(candidate, LayerMask.GetMask("Ground", "MovingPlaform"));
            if (col != null)
            {
                // Nếu candidate nằm dưới bề mặt ground, điều chỉnh candidate.y lên
                if (candidate.y < col.bounds.max.y)
                    candidate.y = col.bounds.max.y + 0.5f; // offset 0.5f, điều chỉnh nếu cần
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
