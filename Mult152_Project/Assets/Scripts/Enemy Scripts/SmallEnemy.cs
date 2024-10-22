using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class SmallEnemy : MonoBehaviour, IDamageable
{
    public float maxHealth = 50f;
    private float currentHealth;
    public GameObject knifePrefab;
    public float throwCooldown = 2f;
    private bool canThrow = true;
    private NavMeshAgent agent;
    private Transform player;
    public Transform spawnPoint; // Transform for the knife spawn point

    void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Ignore collisions with other enemies tagged as "Enemy"
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            Collider[] enemyColliders = GameObject.FindGameObjectsWithTag("Enemy")
                .Select(go => go.GetComponent<Collider>())
                .Where(c => c != null)
                .ToArray();

            foreach (Collider enemyCollider in enemyColliders)
            {
                Physics.IgnoreCollision(collider, enemyCollider);
            }
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= 12f)
        {
            agent.SetDestination(player.position);
            if (canThrow)
            {
                StartCoroutine(ThrowKnife());
            }
        }
    }

    private IEnumerator ThrowKnife()
    {
        canThrow = false;
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion knifeRotation = Quaternion.LookRotation(direction);
        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position + direction * 1.5f;
        GameObject knife = Instantiate(knifePrefab, spawnPosition, knifeRotation);

        // Ignore collisions between the knife and other enemies tagged as "Enemy"
        Collider knifeCollider = knife.GetComponent<Collider>();
        Collider[] enemyColliders = GameObject.FindGameObjectsWithTag("Enemy")
            .Select(go => go.GetComponent<Collider>())
            .Where(c => c != null)
            .ToArray();

        foreach (Collider enemyCollider in enemyColliders)
        {
            Physics.IgnoreCollision(knifeCollider, enemyCollider);
        }

        yield return new WaitForSeconds(throwCooldown);
        canThrow = true;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"SmallEnemy health: {currentHealth}/{maxHealth}");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("SmallEnemy died!");
        gameObject.SetActive(false);
    }
}
