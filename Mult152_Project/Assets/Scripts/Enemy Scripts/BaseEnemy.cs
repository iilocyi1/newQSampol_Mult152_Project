using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    protected float currentHealth;
    public float attackRange = 0.5f;
    public int attackDamage = 20;
    public Transform attackPoint;
    public LayerMask playerLayer;
    public Light attackFlashLight;
    public float flashDuration = .75f;
    public float attackCooldown = 2.5f;
    protected bool canAttack = true;
    protected NavMeshAgent agent;
    protected Transform player;
    protected Animator animator;
    private HashSet<Collider> hitPlayers = new HashSet<Collider>(); // Track hit players

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
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

    protected virtual IEnumerator MeleeAttack()
    {
        canAttack = false;
        agent.isStopped = true; // Stop movement during attack
        animator.SetBool("canAttack", true); // Enable attack in Animator
        animator.SetTrigger("Attack"); // Trigger the attack animation

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
        canAttack = true;
        animator.SetBool("canAttack", false); // Disable attack in Animator
        agent.isStopped = false; // Resume movement after attack
    }

    protected virtual IEnumerator FlashLight()
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

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        gameObject.SetActive(false);
    }

    protected virtual void OnPlayerDetected()
    {
        // Default behavior when the player is detected
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
