using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_LogoSpawner : MonoBehaviour
{
    // This class is responsible for spawning the logo
    
    [Header("Spawner Attributes")]
    public List<GameObject> logoPrefabs;
    public bool canSpawn;
    public int spawnLimit, spawnCount, iconSpawnLimit, iconSpawnCount, direction;

    [Header("Fun Spawn Variables")]
    [Range(0.0f, 10.0f)] [SerializeField] private float timerCooldown;

    [Header("Pooling")]
    public bool hitSpawnLimit;
    public List<GameObject> pooledLogos;

    // Update is called once per frame
    void Update()
    {
        if (canSpawn && !hitSpawnLimit)
        {
            SpawnLogo();
        }
    }

    private void SpawnLogo()
    {
        if (spawnCount < spawnLimit && canSpawn)
        {
            GameObject spawnedLogo = Instantiate(ChooseLogoToSpawn(), SetRandomSpawnPos(), transform.rotation, transform);
            spawnedLogo.GetComponent<Menu_LogoDuplicate>().SetSpawnDirection(direction);
            ModifySpawnCount(1);
            StartCoroutine("SpawnTimer");
        }
        else
        {
            // No more logos can spawn, only will pool for now on
            if (canSpawn)
            {
                canSpawn = false;
                hitSpawnLimit = true;
            }
        }
    }

    private IEnumerator SpawnTimer()
    {
        canSpawn = false;
        yield return new WaitForSeconds(timerCooldown);
        canSpawn = true;
    }

    private Vector3 SetRandomSpawnPos()
    {
        Vector3 spawnPos = transform.position;
        spawnPos.y += Random.Range(-10, 20);
        spawnPos.z += Random.Range(-5, 20);

        return spawnPos;
    }

    public void ModifySpawnCount(int amount)
    {
        spawnCount += amount;
    }

    public void AddToPool(GameObject pooledLogo)
    {
        // Add to pool
        pooledLogos.Add(pooledLogo);

        // Send back to random spawn pos
        Vector3 newSpawnPos = SetRandomSpawnPos();
        pooledLogo.transform.position = newSpawnPos;
        pooledLogo.GetComponent<Menu_LogoDuplicate>().SetNewSpawnPosition(newSpawnPos);

        // Remove from pool
        pooledLogos.Remove(pooledLogo);
    }

    public GameObject ChooseLogoToSpawn()
    {
        GameObject chosenLogo = logoPrefabs[0];
        int randomInt = Random.Range(0, 3);

        // Spawn an icon with a 1/4 in chance as long as less than 4, but if no icons have still spawned by halfway through, spawn one anyway
        if ((randomInt == 0 && iconSpawnCount < iconSpawnLimit) || (spawnCount >= (spawnLimit / 2) && iconSpawnCount == 0))
        {
            chosenLogo = logoPrefabs[1];
            iconSpawnCount++;
        }

        return chosenLogo;
    }
}
