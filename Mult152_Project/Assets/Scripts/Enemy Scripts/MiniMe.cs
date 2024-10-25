using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class MiniMe : MonoBehaviour, IDamageable
{
    public float maxHealth = 400f;
    private float currentHealth;

    public GameObject deathPrefab; // Prefab to instantiate upon death
    public TMP_Text dialogueText; // Reference to TextMeshPro dialogue text UI
    public GameObject dialoguePanel; // Reference to the dialogue panel UI
    public GameObject projectilePrefab;
    public Transform rockSpawnPoint;

    public float attackRange = 1.25f;  // Range for melee attack
    public float throwCooldown = 8f;  // Cooldown between throws
    public float attackCooldown = 3f;  // Cooldown between attacks

    private bool isEnemy = false;
    private bool isDialogueActive = false;
    private bool isAttacking = false;  // Tracks attack cooldown
    private bool canThrow = true;  // Tracks throw cooldown
    private bool canAttack = true;  // Tracks attack cooldown

    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;
    private Rigidbody rb;
    private BossEnemyDamagePlayer damagePlayerScript;
    private bool wasInAttackAnimation = false;  // Tracks previous animation state

    public bool IsAttacking
    {
        get { return isAttacking; }
    }

    void Start()
    {
        currentHealth = maxHealth;
        gameObject.tag = "NPC";  // Set tag to NPC
        dialogueText.text = "I carry the key to your final destination. Do you want to fight me? Press Y for Yes, N for No.";
        dialoguePanel.SetActive(false);  // Hide dialogue initially

        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;

        damagePlayerScript = GetComponentInChildren<BossEnemyDamagePlayer>();
        if (damagePlayerScript != null)
        {
            damagePlayerScript.enabled = false;  // Ensure the script is initially disabled
        }
    }

    void Update()
    {
        HandleDialogue();

        if (isEnemy && agent.isOnNavMesh && !isAttacking)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (canThrow)
            {
                Debug.Log("Preparing to throw");
                animator.SetFloat("ThrowCooldown", throwCooldown);
                animator.SetTrigger("Throw");
                StartCoroutine(ThrowCooldownCoroutine());
            }
            else
            {
                if (canAttack && distanceToPlayer <= attackRange)
                {
                    Debug.Log("Preparing to attack");
                    animator.SetBool("canAttack", true);
                    animator.SetTrigger("Attack");
                    StartCoroutine(AttackCooldownCoroutine());
                }
                else
                {
                    animator.SetBool("canAttack", false);
                    MoveTowardsPlayer();
                }
            }

            animator.SetFloat("Speed", agent.velocity.magnitude);
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("NavMeshAgent is not on the NavMesh.");
        }

        HandleAnimationState();  // Check if attack animation is active
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

    private void PerformAttack()
    {
        agent.ResetPath();  // Stop movement
        animator.SetTrigger("Attack");  // Trigger attack animation
        StartCoroutine(AttackCooldownCoroutine());  // Start attack cooldown
    }

    private void MoveTowardsPlayer()
    {
        agent.SetDestination(player.position);
    }

    private void HandleAnimationState()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);  // Get current animation state
        bool isInAttackAnimation = stateInfo.IsName("editable Great Sword Slash");

        // Enable or disable the damage player script based on the attack animation state
        if (damagePlayerScript != null && isInAttackAnimation != wasInAttackAnimation)
        {
            damagePlayerScript.enabled = isInAttackAnimation;
            if (isInAttackAnimation)
            {
                Debug.Log("Script activated");
            }
            else
            {
                Debug.Log("Script deactivated");
            }
            wasInAttackAnimation = isInAttackAnimation;
        }
    }

    void BecomeEnemy()
    {
        isEnemy = true;
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        gameObject.tag = "Enemy";
        animator.SetTrigger("Idle");
    }

    void StayAsNPC()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        animator.SetTrigger("Idle");
    }

    private IEnumerator AttackCooldownCoroutine()
    {
        isAttacking = true;
        canAttack = false;
        animator.SetBool("canAttack", false);
        yield return new WaitForSeconds(attackCooldown);  // Wait for attack cooldown
        isAttacking = false;
        canAttack = true;
        animator.SetBool("canAttack", true);  // Reset the parameter
    }

    private IEnumerator ThrowCooldownCoroutine()
    {
        canThrow = false;
        animator.SetBool("canThrow", false);
        float elapsedTime = 0f;
        while (elapsedTime < throwCooldown)
        {
            elapsedTime += Time.deltaTime;
            animator.SetFloat("ThrowCooldown", throwCooldown - elapsedTime);
            yield return null;
        }
        canThrow = true;
        animator.SetBool("canThrow", true);
    }

    void OnAnimatorMove()
    {
        if (isAttacking)
        {
            agent.velocity = Vector3.zero;
            transform.position = agent.nextPosition;
        }
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

    // Method to be called by the animation event
    public void RockThrow()
    {
        if (projectilePrefab != null && rockSpawnPoint != null && player != null)
        {
            GameObject rock = Instantiate(projectilePrefab, rockSpawnPoint.position, Quaternion.identity);
            Rigidbody rockRb = rock.GetComponent<Rigidbody>();
            if (rockRb != null)
            {
                // Calculate the direction from the enemy to the player
                Vector3 directionToPlayer = (player.position - rockSpawnPoint.position).normalized;
                rockRb.AddForce(directionToPlayer * 10f, ForceMode.Impulse); // Adjust the force as needed
            }
        }
    }
}
