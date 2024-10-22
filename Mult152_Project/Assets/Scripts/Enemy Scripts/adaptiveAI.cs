using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class adaptiveAI : MonoBehaviour, IDamageable
{
    public float maxHealth = 100;
    private float currentHealth;
    public float detectionRange = 6f;
    public float damage = 10f;
    public Transform player;
    protected NavMeshAgent agent; // Changed to protected

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(AdaptDifficulty());
    }

    protected virtual void Update()
    {
        if (agent.isOnNavMesh)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= detectionRange)
            {
                agent.SetDestination(player.position);
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

    private IEnumerator AdaptDifficulty()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f); // Increase difficulty every 60 seconds
            maxHealth += 10;
            damage += 2f;
        }
    }
}