using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SmallEnemy : MonoBehaviour, IDamageable
{
    public float maxHealth = 50f;
    private float currentHealth;
    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;
    public GameObject projectilePrefab; // Reference to the projectile prefab
    public Transform knifeSpawnPoint; // Transform for the knife spawn point
    private bool isAttacking = false; // Flag to manage attack cooldown

    void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= 12f && !isAttacking)
        {
            agent.SetDestination(player.position);
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                StartCoroutine(PerformAttack());
            }
        }

        // Set the Speed parameter based on the agent's velocity
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true; // Set attacking state
        agent.isStopped = true; // Stop the agent
        animator.SetTrigger("Kunai"); // Trigger the Kunai attack animation
        yield return new WaitForSeconds(3f); // Wait for the attack cooldown
        isAttacking = false; // Reset attacking state
        agent.isStopped = false; // Resume the agent
    }

    // This method will be called by the animation event
    public void Throw()
    {
        Debug.Log("Kunai instantiated!");
        Vector3 spawnPosition = knifeSpawnPoint.position;
        Quaternion spawnRotation = Quaternion.LookRotation(transform.forward); // Use enemy's forward direction
        Instantiate(projectilePrefab, spawnPosition, spawnRotation);
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

    void OnAnimatorMove()
    {
        if (isAttacking)
        {
            // Override the agent's position to keep it rooted
            agent.velocity = Vector3.zero;
            transform.position = agent.nextPosition;
        }
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }
}
