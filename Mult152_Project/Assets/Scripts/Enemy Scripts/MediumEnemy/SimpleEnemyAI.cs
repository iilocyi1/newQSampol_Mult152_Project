using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleEnemyAI : MonoBehaviour, IDamageable, IDifficultyAdjustable
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
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found.");
        }

        if (player == null)
        {
            Debug.LogError("Player not found. Make sure there is a GameObject with the tag 'Player' in the scene.");
        }

        if (animator == null)
        {
            Debug.LogError("Animator component not found.");
        }

        if (DifficultyManager.instance != null)
        {
            DifficultyManager.instance.RegisterEnemy(this);
        }
        else
        {
            Debug.LogError("DifficultyManager instance not found. Make sure it is initialized.");
        }

        agent.speed = AgentSpeed; // Set initial speed
    }


    void OnDestroy()
    {
        if (DifficultyManager.instance != null)
        {
            DifficultyManager.instance.UnregisterEnemy(this);
        }
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

    public void IncreaseDifficulty(float healthMultiplier, float speedMultiplier)
    {
        maxHealth *= healthMultiplier;
        currentHealth = maxHealth; // Reset current health to new max health
        AgentSpeed *= speedMultiplier;
        agent.speed = AgentSpeed; // Update agent speed
    }
}
