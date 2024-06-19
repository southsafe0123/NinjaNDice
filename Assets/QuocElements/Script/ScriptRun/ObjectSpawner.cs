using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] objectsToSpawn; // Danh sách các object sẽ được spawn
    public Transform[] spawnPoints; // Danh sách các điểm spawn object
    public float spawnInterval = 2f; // Khoảng thời gian giữa các lần spawn
    public float objectSpeed = 5f; // Tốc độ di chuyển của object

    void Start()
    {
        // Bắt đầu spawn object
        StartCoroutine(SpawnObjects());
    }

    IEnumerator SpawnObjects()
    {
        while (true)
        {
            SpawnObject();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnObject()
    {
        // Chọn một object ngẫu nhiên từ danh sách
        int randomObjectIndex = Random.Range(0, objectsToSpawn.Length);
        // Chọn một vị trí ngẫu nhiên từ danh sách
        int randomSpawnPointIndex = Random.Range(0, spawnPoints.Length);

        GameObject spawnedObject = Instantiate(objectsToSpawn[randomObjectIndex], spawnPoints[randomSpawnPointIndex].position, Quaternion.identity);

        // Di chuyển object từ phải sang trái
        Rigidbody2D rb = spawnedObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(-objectSpeed, 0);
        }
        else
        {
            Debug.LogWarning("The spawned object does not have a Rigidbody2D component.");
        }
    }

    void Update()
    {

    }
}
