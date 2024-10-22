using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LargeEnemy : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    private float currentHealth;
    public float attackRange = 1f;
    public int attackDamage = 20;
    public Transform attackPoint;
    public LayerMask playerLayer;
    public Light attackFlashLight;
    public float flashDuration = .75f;
    public float attackCooldown = 2.5f; // Updated cooldown duration
    private bool canAttack = true;
    private NavMeshAgent agent;
    private Transform player;

    void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= 12f)
        {
            agent.SetDestination(player.position);
            if (distanceToPlayer <= attackRange && canAttack)
            {
                StartCoroutine(MeleeAttack());
            }
        }
    }

    private IEnumerator MeleeAttack()
    {
        canAttack = false;
        StartCoroutine(FlashLight());

        Collider[] hitPlayers = Physics.OverlapSphere(attackPoint.position, attackRange, playerLayer);
        foreach (Collider player in hitPlayers)
        {
            player.GetComponent<Player>().TakeDamage(attackDamage);
            Debug.Log("Melee attack hit!");
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
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
        Debug.Log($"LargeEnemy health: {currentHealth}/{maxHealth}");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("LargeEnemy died!");
        gameObject.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
