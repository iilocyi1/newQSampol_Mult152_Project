using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class adaptiveAISmallEnemy : MonoBehaviour, IDamageable
{
    public float maxHealth = 50f;
    private float currentHealth;
    public float detectionRange = 10f;
    public float damage = 10f;
    public Transform player;
    protected NavMeshAgent agent;
    public float maintainDistance = 5f; // Distance to maintain from the player

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent.stoppingDistance = maintainDistance; // Set stopping distance to maintain distance
    }

    protected virtual void Update()
    {
        if (agent.isOnNavMesh)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= detectionRange)
            {
                if (distanceToPlayer > maintainDistance)
                {
                    agent.SetDestination(player.position);
                }
                else
                {
                    // Move away from the player to maintain distance
                    Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
                    Vector3 newPosition = transform.position + directionAwayFromPlayer * maintainDistance;
                    agent.SetDestination(newPosition);

                    // Rotate to face the player
                    Vector3 directionToPlayer = (player.position - transform.position).normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // Smooth rotation
                }
                OnPlayerDetected();
            }
        }
        else
        {
            Debug.LogWarning("NavMeshAgent is not on the NavMesh.");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died!");
        gameObject.SetActive(false);
    }

    protected virtual void OnPlayerDetected()
    {
        // Override in derived classes for specific behaviors
    }
}
