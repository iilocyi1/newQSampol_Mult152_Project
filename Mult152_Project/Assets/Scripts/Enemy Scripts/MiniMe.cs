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
    public TMP_Text dialogueText; // Reference to the TextMeshPro dialogue text UI
    public GameObject dialoguePanel; // Reference to the dialogue panel UI
    private bool isDialogueActive = false; // Flag to check if dialogue is active
    private bool isEnemy = false; // Flag to check if MiniMe is an enemy
    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        gameObject.tag = "NPC"; // Set the tag to NPC
        dialogueText.text = "I carry the key to your final destination, through the gates of the afterlife or through the gates of destiny. Do you want to fight me? Press Y for Yes, N for No";
        dialoguePanel.SetActive(false); // Hide the dialogue panel initially
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDialogueActive)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                BecomeEnemy();
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                StayAsNPC();
            }
        }

        if (isEnemy)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= agent.stoppingDistance)
            {
                agent.isStopped = true;
                
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
                animator.SetFloat("Speed", agent.velocity.magnitude);
            }
        }

        // Check for interaction input
        if (Input.GetKeyDown(KeyCode.T) && !isEnemy)
        {
            if (Vector3.Distance(transform.position, player.position) <= agent.stoppingDistance)
            {
                dialoguePanel.SetActive(true); // Show the dialogue panel
                isDialogueActive = true; // Activate dialogue
               
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isEnemy)
        {
            // Player is in range, but dialogue is triggered by pressing 'T'
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isEnemy)
        {
            dialoguePanel.SetActive(false); // Hide the dialogue panel
            isDialogueActive = false; // Deactivate dialogue
            
        }
    }

    void BecomeEnemy()
    {
        isEnemy = true;
        isDialogueActive = false;
        dialoguePanel.SetActive(false); // Hide the dialogue panel
        gameObject.tag = "Enemy"; // Change tag to Enemy
        Debug.Log("MiniMe has become an enemy!");
    }

    void StayAsNPC()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false); // Hide the dialogue panel
        Debug.Log("MiniMe remains as an NPC.");
        
    }

    public void TakeDamage(int damage)
    {
        if (isEnemy)
        {
            currentHealth -= damage;
            Debug.Log($"MiniMe health: {currentHealth}/{maxHealth}");
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        Debug.Log("MiniMe died!");
        Instantiate(deathPrefab, transform.position, transform.rotation); // Instantiate the death prefab
        gameObject.SetActive(false); // Deactivate the MiniMe object
        
    }
}
