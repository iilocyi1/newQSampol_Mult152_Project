using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float health = 100f;
    private bool isBlocking = false;

    public void TakeDamage(float damage)
    {
        if (isBlocking)
        {
            damage *= 0.5f; // Reduce damage by 50% when blocking
        }

        health -= damage;
        Debug.Log("Player took damage! Current health: " + health);
        if (health <= 0)
        {
            Die();
        }
    }

    public void SetBlockingState(bool blocking)
    {
        isBlocking = blocking;
    }

    public void Die()
    {
        // Handle player death
        Debug.Log("Player died!");
        GameManager.instance.TriggerGameOver();
        Destroy(gameObject);
        // Add additional logic for player death, such as respawning or game over screen
    }
}
