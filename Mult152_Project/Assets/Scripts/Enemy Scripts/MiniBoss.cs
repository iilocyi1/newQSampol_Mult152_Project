using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MiniBoss : MonoBehaviour, IDamageable
{
    public float maxHealth = 200f;
    private float currentHealth;
    public GameObject keyPrefab;
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;
    public GameObject projectilePrefab; // Projectile prefab
    public float throwCooldown = 2f; // Cooldown for throwing projectiles
    public Transform spawnPoint; // Transform for the projectile spawn point
    [HideInInspector] public bool canThrow = true; // Make canThrow accessible to MiniBossAI
    private bool playerInRange = false;
    private bool dialogueActive = false;
    private bool isEnemy = false;
    private MiniBossAI miniBossAI;
    private Transform player; // Define the player variable

    void Start()
    {
        currentHealth = maxHealth;
        dialoguePanel.SetActive(false); // Ensure the panel is inactive at the start
        miniBossAI = GetComponent<MiniBossAI>();
        if (miniBossAI != null)
        {
            miniBossAI.enabled = false; // Disable the AI script initially
        }
        player = GameObject.FindGameObjectWithTag("Player").transform; // Assign the player variable
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.T))
        {
            Interact();
        }

        if (dialogueActive)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                StartFight();
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                EndDialogue();
            }
        }
    }

    public void Interact()
    {
        if (!dialogueActive && !isEnemy)
        {
            StartCoroutine(ShowDialogue("Do you want to fight me? Press Y for Yes, N for No"));
        }
    }

    private IEnumerator ShowDialogue(string message)
    {
        dialogueActive = true;
        dialogueText.text = message;
        dialoguePanel.SetActive(true); // Show the panel
        yield return null; // Wait for player input
    }

    private void EndDialogue()
    {
        dialogueActive = false;
        dialoguePanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void StartFight()
    {
        isEnemy = true;
        gameObject.tag = "Enemy";
        if (miniBossAI != null)
        {
            miniBossAI.enabled = true; // Enable the AI script to start the fight
        }
        EndDialogue();
    }

    public void TakeDamage(int damage)
    {
        if (isEnemy)
        {
            currentHealth -= damage;
            Debug.Log($"MiniBoss health: {currentHealth}/{maxHealth}");
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        Vector3 spawnPosition = transform.position + Vector3.up * 1.5f; // Adjust the spawn position to be above the ground
        Instantiate(keyPrefab, spawnPosition, Quaternion.identity);
        Destroy(gameObject);
    }

    public IEnumerator ThrowProjectile()
    {
        canThrow = false;
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion projectileRotation = Quaternion.LookRotation(direction);
        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position + direction * 1.5f; // Use spawnPoint if assigned
        Instantiate(projectilePrefab, spawnPosition, projectileRotation);
        yield return new WaitForSeconds(throwCooldown);
        canThrow = true;
    }
}
