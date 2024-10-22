using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCDialogue : MonoBehaviour
{
    public GameObject miniBossPrefab;
    public Transform spawnPoint;
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel; // Reference to the background panel
    private bool playerInRange = false;
    private bool dialogueActive = false;
    private bool miniBossSpawned = false; // Flag to check if the mini-boss has been spawned

    void Start()
    {
        dialoguePanel.SetActive(false); // Ensure the panel is inactive at the start
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.T))
        {
            Interact();
        }
    }

    public void Interact()
    {
        if (!dialogueActive)
        {
            StartCoroutine(ShowDialogue("To enter, you must defeat the one that holds the key"));
            if (!miniBossSpawned)
            {
                Instantiate(miniBossPrefab, spawnPoint.position, spawnPoint.rotation);
                miniBossSpawned = true; // Set the flag to true after spawning the mini-boss
            }
        }
    }

    private IEnumerator ShowDialogue(string message)
    {
        dialogueActive = true;
        dialogueText.text = message;
        dialoguePanel.SetActive(true); // Show the panel
        yield return new WaitForSeconds(3f); // Display the message for 3 seconds
        dialoguePanel.SetActive(false); // Hide the panel
        dialogueActive = false;
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
}
