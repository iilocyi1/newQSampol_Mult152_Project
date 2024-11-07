using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager instance;
    public float difficultyIncreaseInterval = 90f; // Time in seconds to increase difficulty
    public float healthMultiplier = 1.1f;
    public float speedMultiplier = 1.05f;

    private List<IDifficultyAdjustable> enemies = new List<IDifficultyAdjustable>();

    private void Awake()
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

    private void Start()
    {
        StartCoroutine(IncreaseDifficultyOverTime());
    }

    private IEnumerator IncreaseDifficultyOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(difficultyIncreaseInterval);
            IncreaseDifficulty();
        }
    }

    private void IncreaseDifficulty()
    {
        foreach (var enemy in enemies)
        {
            enemy.IncreaseDifficulty(healthMultiplier, speedMultiplier);
        }
    }

    public void RegisterEnemy(IDifficultyAdjustable enemy)
    {
        enemies.Add(enemy);
    }

    public void UnregisterEnemy(IDifficultyAdjustable enemy)
    {
        enemies.Remove(enemy);
    }
}

public interface IDifficultyAdjustable
{
    void IncreaseDifficulty(float healthMultiplier, float speedMultiplier);
}
