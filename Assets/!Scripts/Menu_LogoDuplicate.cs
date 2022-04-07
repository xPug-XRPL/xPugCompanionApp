using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_LogoDuplicate : MonoBehaviour
{
    // This class is responsible for controlling the spawned logo
    
    private Menu_LogoSpawner localLogoSpawner;

    [Header("Logo Attributes")]
    public bool canFly;
    public float logoFlySpeed;

    [Header("Spawn Attributes")]
    public float distanceFromSpawn;
    private Vector3 spawnPosition;
    private int localDirection = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        RandomSpawnSpeed();
        SetNewSpawnPosition(transform.position);
        localLogoSpawner = transform.parent.GetComponent<Menu_LogoSpawner>();

        // Get event camera
        GetComponent<Canvas>().worldCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (canFly)
        {
            FlyToDirection(localDirection);

            // Start despawn sequence
            SpawnDistance();
        }
    }

    private void FlyToDirection(int flyDir)
    {
        switch (flyDir)
        {
            case 0:
                transform.Translate(Vector3.right * logoFlySpeed * Time.deltaTime);
                break;
            case 1:
                transform.Translate(-Vector3.right * logoFlySpeed * Time.deltaTime);
                break;
        }
    }

    private void SpawnDistance()
    {
        // Get spawn distance
        distanceFromSpawn = Vector3.Distance(spawnPosition, transform.position);

        if (distanceFromSpawn >= 70.0f)
        {
            RepoolLogo();
        }
    }

    private void RepoolLogo()
    {
        localLogoSpawner.AddToPool(gameObject);
        RandomSpawnSpeed();
    }

    private void RandomSpawnSpeed()
    {
        logoFlySpeed = Random.Range(5.0f, 8.0f);
    }

    public void SetNewSpawnPosition(Vector3 newPos)
    {
        spawnPosition = newPos;
    }

    public void SetSpawnDirection(int dir)
    {
        localDirection = dir;
    }
}
