using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs; // Prefab của enemy
    public List<Transform> spawnPoints; // Danh sách các vị trí spawn
    public float minSpawnInterval = 2f; // Thời gian tối thiểu giữa các lần spawn
    public float maxSpawnInterval = 5f; // Thời gian tối đa giữa các lần spawn
    public float speedEnemyClone = 5f;

    private void Start()
    {
        // Bắt đầu coroutine để spawn enemy
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        // Chờ 5 giây trước khi bắt đầu spawn enemy
        yield return new WaitForSeconds(5f);

        while (true)
        {
            // Lặp lại việc spawn enemy với khoảng thời gian ngẫu nhiên
            SpawnEnemy();
            float randomInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(randomInterval);
        }
    }

    private void SpawnEnemy()
    {
        // Kiểm tra nếu danh sách enemyPrefabs rỗng hoặc không có phần tử nào hợp lệ
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("No enemy prefabs available to spawn.");
            return;
        }

        // Chọn một vị trí spawn ngẫu nhiên từ danh sách
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        // Chọn một enemy prefab ngẫu nhiên từ danh sách
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

        // Kiểm tra xem enemyPrefab không phải là null
        if (enemyPrefab != null)
        {
            // Tạo một enemy tại vị trí spawn
            GameObject enemyClone = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            // Di chuyển enemy từ phải sang trái
            Rigidbody2D rb = enemyClone.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = new Vector2(-speedEnemyClone, 0);
            }
            else
            {
                Debug.LogWarning("The enemy object does not have a Rigidbody2D component.");
            }
        }
        else
        {
            Debug.LogWarning("Selected enemy prefab is null.");
        }
    }
}
