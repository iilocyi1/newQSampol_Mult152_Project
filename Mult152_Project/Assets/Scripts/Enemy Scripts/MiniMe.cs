using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class MiniMe : MonoBehaviour, IDamageable
{
    public float maxHealth = 400f;
    private float currentHealth;

    public GameObject deathPrefab;
    public TMP_Text dialogueText;
    public GameObject dialoguePanel;
    public GameObject projectilePrefab;  // Rock prefab
    public Transform rockSpawnPoint;     // Where the rock spawns

    public float attackRange = 1.5f;
    public float throwRange = 5f;
    public float throwCooldown = 8f;
    public float attackCooldown = 3f;

    private bool isEnemy = false;
    private bool isDialogueActive = false;
    private bool isAttacking = false;
    private bool canAttack = true;
    private bool canThrow = true;
    private bool wasInAttackAnimation = false;  // Track previous animation state

    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;
    private BossEnemyDamagePlayer damagePlayerScript;

    void Start()
    {
        currentHealth = maxHealth;
        gameObject.tag = "NPC";
        dialogueText.text = "I carry the key to your final destination. Do you want to fight me? Press Y for Yes, N for No.";
        dialoguePanel.SetActive(false);

        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();

        damagePlayerScript = GetComponentInChildren<BossEnemyDamagePlayer>();
        if (damagePlayerScript != null)
        {
            damagePlayerScript.enabled = false;
        }
    }

    void Update()
    {
        HandleDialogue();

        if (isEnemy && agent.isOnNavMesh)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange && canAttack)
            {
                StopAndAttack();
            }
            else if (distanceToPlayer <= throwRange && canThrow)
            {
                PerformThrow();
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
                animator.SetBool("isWalking", true);  // Play walking animation
            }

            animator.SetFloat("Speed", agent.velocity.magnitude);
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("NavMeshAgent is not on the NavMesh.");
        }

        HandleAnimationState();  // Check and handle attack animation state
    }

    private void StopAndAttack()
    {
        agent.isStopped = true;
        animator.SetBool("isWalking", false);
        animator.SetTrigger("Attack");
        StartCoroutine(AttackCooldownCoroutine());
    }

    private void PerformThrow()
    {
        agent.isStopped = true;
        animator.SetBool("isWalking", false);
        animator.SetTrigger("Throw");
        StartCoroutine(ThrowCooldownCoroutine());
    }

    public void RockThrow()  // Called by animation event
    {
        if (projectilePrefab != null && rockSpawnPoint != null && player != null)
        {
            GameObject rock = Instantiate(projectilePrefab, rockSpawnPoint.position, Quaternion.identity);
            Rigidbody rockRb = rock.GetComponent<Rigidbody>();

            if (rockRb != null)
            {
                // Calculate direction towards the player
                Vector3 directionToPlayer = (player.position - rockSpawnPoint.position).normalized;
                rockRb.AddForce(directionToPlayer * 10f, ForceMode.Impulse);  // Adjust force as needed
            }
        }
    }

    private void HandleAnimationState()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isInAttackAnimation = stateInfo.IsName("editable Melee");

        if (damagePlayerScript != null && isInAttackAnimation)
        {
            if (!wasInAttackAnimation)
            {
                damagePlayerScript.enabled = true;
                Debug.Log("BossEnemyDamagePlayer script activated.");
            }

            if (stateInfo.normalizedTime >= 1);
            {
                damagePlayerScript.enabled = false;
                Debug.Log("BossEnemyDamagePlayer script deactivated (animation complete).");
            }
        }
        else if (damagePlayerScript.enabled)
        {
            damagePlayerScript.enabled = false;
            Debug.Log("BossEnemyDamagePlayer script forcefully deactivated.");
        }

        wasInAttackAnimation = isInAttackAnimation;
    }

    private IEnumerator AttackCooldownCoroutine()
    {
        isAttacking = true;
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
        canAttack = true;
    }

    private IEnumerator ThrowCooldownCoroutine()
    {
        canThrow = false;
        animator.SetBool("canThrow", false);
        yield return new WaitForSeconds(throwCooldown);
        canThrow = true;
        animator.SetBool("canThrow", true);
    }

    private void HandleDialogue()
    {
        if (isDialogueActive)
        {
            if (Input.GetKeyDown(KeyCode.Y)) BecomeEnemy();
            else if (Input.GetKeyDown(KeyCode.N)) StayAsNPC();
        }

        if (Input.GetKeyDown(KeyCode.T) && !isEnemy)
        {
            if (Vector3.Distance(transform.position, player.position) <= agent.stoppingDistance)
            {
                dialoguePanel.SetActive(true);
                isDialogueActive = true;
            }
        }
    }

    void BecomeEnemy()
    {
        isEnemy = true;
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        gameObject.tag = "Enemy";
    }

    void StayAsNPC()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"MiniMe health: {currentHealth}/{maxHealth}");
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        Instantiate(deathPrefab, transform.position, transform.rotation);
        gameObject.SetActive(false);
    }

    void OnAnimatorMove()
    {
        if (isAttacking)
        {
            agent.velocity = Vector3.zero;
            transform.position = agent.nextPosition;
        }
    }
}
