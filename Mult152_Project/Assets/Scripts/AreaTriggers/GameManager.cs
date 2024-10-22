using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private bool isGameOver = false;
    public GameObject[] enemyPrefabs;
    private float zPosRange = 18;
    private int amount = 7;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Initialize game state
        isGameOver = false;
        InvokeRepeating("SpawnRandomEnemies", 1.0f, 300.0f);
    }

    public void TriggerGameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            StopAllSpawning();
            StopPlayerMovement();
            StopEnemyMovement();
            StopAnimationsAndParticles();
            StopSounds();
            Debug.Log("Game Over!");
        }
    }

    private void StopAllSpawning()
    {
        // Implement logic to stop all spawning of game objects
        CancelInvoke("SpawnRandomEnemies");
    }

    private void StopPlayerMovement()
    {
        // Implement logic to stop player movement
        playerController playerController = FindObjectOfType<playerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }
    }

    private void StopEnemyMovement()
    {
        // Implement logic to stop all enemy movement
        UnityEngine.AI.NavMeshAgent[] enemies = FindObjectsOfType<UnityEngine.AI.NavMeshAgent>();
        foreach (UnityEngine.AI.NavMeshAgent enemy in enemies)
        {
            enemy.isStopped = true;
        }
    }

    private void StopAnimationsAndParticles()
    {
        // Implement logic to stop animations and particle systems
        Animator[] animators = FindObjectsOfType<Animator>();
        foreach (Animator animator in animators)
        {
            animator.enabled = false;
        }

        ParticleSystem[] particleSystems = FindObjectsOfType<ParticleSystem>();
        foreach (ParticleSystem particleSystem in particleSystems)
        {
            particleSystem.Stop();
        }
    }

    private void StopSounds()
    {
        // Implement logic to stop sounds
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.Stop();
        }
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
        for (int i = 0; i < amount; i++)
        {
            SpawnRandomEnemy();
        }
    }
}
