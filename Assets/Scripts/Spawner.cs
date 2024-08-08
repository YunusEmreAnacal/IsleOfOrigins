using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject sheepPrefab;
    public GameObject zombiePrefab;
    public Transform[] spawnPoints;
    private List<GameObject> spawnedSheep = new List<GameObject>();
    private List<GameObject> spawnedZombie = new List<GameObject>();

    public int maxSheepCount = 5;
    public int maxZombieCount = 5;

    void Start()
    {
        for (int i = 0; i < maxSheepCount; i++)
        {
            SpawnSheep();
            SpawnZombie();
        }
    }

    void SpawnSheep()
    {
        if (spawnedSheep.Count >= maxSheepCount) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject newSheep = Instantiate(sheepPrefab, spawnPoint.position, spawnPoint.rotation);
        Sheep_Data sheepHealth = newSheep.GetComponent<Sheep_Data>();

        if (sheepHealth != null)
        {
            sheepHealth.OnDeathEvent += OnSheepDeath;
        }

        spawnedSheep.Add(newSheep);
    }

    void SpawnZombie()
    {
        if (spawnedZombie.Count >= maxZombieCount) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject newZombie = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);
        Zombie_Data zombieHealth = newZombie.GetComponent<Zombie_Data>();

        if (zombieHealth != null)
        {
            zombieHealth.OnDeathEvent += OnZombieDeath;
        }

        spawnedZombie.Add(newZombie);
    }

    void OnSheepDeath(GameObject deadSheep)
    {
        spawnedSheep.Remove(deadSheep);
        SpawnSheep(); // Ölen koyunun yerine yenisini spawn et
    }
    void OnZombieDeath(GameObject deadZombie)
    {
        spawnedZombie.Remove(deadZombie);
        SpawnZombie(); 
    }
}
