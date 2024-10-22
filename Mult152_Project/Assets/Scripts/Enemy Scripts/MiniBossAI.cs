using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MiniBossAI : MonoBehaviour
{
    public float attackRange = 1f;
    public int attackDamage = 20;
    public Transform attackPoint;
    public LayerMask playerLayer;
    public Light attackFlashLight;
    public float flashDuration = .75f;
    public float attackCooldown = 2.0f;
    private bool canAttack = true;
    private NavMeshAgent agent;
    private Transform player;
    private MiniBoss miniBoss;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        miniBoss = GetComponent<MiniBoss>();
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
            else if (miniBoss != null && miniBoss.canThrow)
            {
                StartCoroutine(miniBoss.ThrowProjectile());
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

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}

