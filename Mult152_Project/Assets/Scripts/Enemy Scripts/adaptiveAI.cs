using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class adaptiveAI : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    private float currentHealth;
    public float attackRange = 0.5f;
    public int attackDamage = 20;
    public Transform attackPoint;
    public LayerMask playerLayer;
    public Light attackFlashLight;
    public float flashDuration = .75f;
    public float attackCooldown = 2.5f;
    private bool canAttack = true;
    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;
    private HashSet<Collider> hitPlayers = new HashSet<Collider>(); // Track hit players
    public float detectionRange = 6f;

    void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        StartCoroutine(AdaptDifficulty());
    }

    void Update()
    {
        if (agent.isOnNavMesh)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= detectionRange)
            {
                agent.SetDestination(player.position);
                OnPlayerDetected();
            }

            if (distanceToPlayer <= 12f)
            {
                if (distanceToPlayer > attackRange)
                {
                    agent.SetDestination(player.position);
                }
                else if (canAttack)
                {
                    StartCoroutine(MeleeAttack());
                }
            }

            // Set the Speed parameter based on the agent's velocity
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
        else
        {
            Debug.LogWarning("NavMeshAgent is not on the NavMesh.");
        }
    }

    private IEnumerator MeleeAttack()
    {
        Debug.Log("Starting MeleeAttack");
        agent.isStopped = true; // Stop movement during attack
        animator.SetBool("canAttack", true); // Enable attack in Animator
        animator.SetTrigger("Attack"); // Trigger the attack animation
        canAttack = false;

        StartCoroutine(FlashLight());

        yield return new WaitForSeconds(0.1f); // Small delay to sync with animation

        Collider[] hitPlayers = Physics.OverlapSphere(attackPoint.position, attackRange, playerLayer);
        foreach (Collider player in hitPlayers)
        {
            if (player.CompareTag("Player"))
            {
                IDamageable damageable = player.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(attackDamage);
                    Debug.Log("Blade hit the player!");
                }
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        agent.isStopped = false;
        canAttack = true;
        animator.SetBool("canAttack", false); // Disable attack in Animator
        Debug.Log("Finished MeleeAttack");
    }

    private IEnumerator FlashLight()
    {
        attackFlashLight.enabled = true;
        yield return new WaitForSeconds(flashDuration);
        attackFlashLight.enabled = false;
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

    private void OnPlayerDetected()
    {
        // Specific behavior for adaptiveAI when the player is detected
    }

    private IEnumerator AdaptDifficulty()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f); // Increase difficulty every 60 seconds
            maxHealth += 10;
            attackDamage += 2;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
