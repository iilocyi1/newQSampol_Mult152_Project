using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    private float zPosRange = 18;
    int Amount = 8;

    void Start()
    {
        InvokeRepeating("SpawnRandomEnemies", 1.0f, 300.0f);
    }

    void SpawnRandomEnemy()
    {
        float randXPos = Random.Range(37.0f, 48f);
        float randZPos = Random.Range(-zPosRange, zPosRange);
        int enemyPrefabIndex = Random.Range(0, enemyPrefabs.Length);
        Vector3 randPos = new Vector3(randXPos, 25, randZPos);
        Instantiate(enemyPrefabs[enemyPrefabIndex], randPos, enemyPrefabs[enemyPrefabIndex].transform.rotation);
    }

    void SpawnRandomEnemies()
    {
        for (int i = 0; i < Amount; i++)
        {
            SpawnRandomEnemy();
        }
    }
}