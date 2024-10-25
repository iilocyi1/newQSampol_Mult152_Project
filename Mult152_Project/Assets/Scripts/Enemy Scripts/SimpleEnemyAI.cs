using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleEnemyAI : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    private float currentHealth;
    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;
    public float AgentSpeed = 2.5f;
    private bool isAttacking = false; // Flag to control attack state

    void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        agent.speed = AgentSpeed; // Set initial speed
    }

    void Update()
    {
        if (agent.isOnNavMesh && !isAttacking)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer < 8f)
            {
                if (distanceToPlayer > 1.25f)
                {
                    agent.SetDestination(player.position);
                }
                else
                {
                    agent.ResetPath(); // Stop moving when within 1.25 units of the player
                    animator.SetTrigger("Attack"); // Trigger the attack animation
                    StartCoroutine(AttackCooldown()); // Start cooldown coroutine
                }
            }

            // Set the Speed parameter based on the agent's velocity
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
        else if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("NavMeshAgent is not on the NavMesh.");
        }
    }

    private IEnumerator AttackCooldown()
    {
        isAttacking = true; // Set attacking state
        yield return new WaitForSeconds(3f); // Wait for 3 seconds
        isAttacking = false; // Reset attacking state
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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} health: {currentHealth}/{maxHealth}");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        gameObject.SetActive(false);
    }
}