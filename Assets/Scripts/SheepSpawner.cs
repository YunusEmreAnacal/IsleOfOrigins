using System.Collections.Generic;
using UnityEngine;

public class SheepSpawner : MonoBehaviour
{
    public GameObject sheepPrefab;
    public Transform[] spawnPoints;
    private List<GameObject> spawnedSheep = new List<GameObject>();

    public int maxSheepCount = 10;

    void Start()
    {
        for (int i = 0; i < maxSheepCount; i++)
        {
            SpawnSheep();
        }
    }

    void SpawnSheep()
    {
        if (spawnedSheep.Count >= maxSheepCount) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject newSheep = Instantiate(sheepPrefab, spawnPoint.position, spawnPoint.rotation);
        SheepHealth sheepHealth = newSheep.GetComponent<SheepHealth>();

        if (sheepHealth != null)
        {
            sheepHealth.OnDeathEvent += OnSheepDeath;
        }

        spawnedSheep.Add(newSheep);
    }

    void OnSheepDeath(GameObject deadSheep)
    {
        spawnedSheep.Remove(deadSheep);
        SpawnSheep(); // Ölen koyunun yerine yenisini spawn et
    }
}
