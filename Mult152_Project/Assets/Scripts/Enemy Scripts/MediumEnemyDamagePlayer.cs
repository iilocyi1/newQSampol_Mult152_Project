using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumEnemyDamagePlayer : MonoBehaviour
{
    public int attackDamage = 10; // Damage to deal to the player
    private Player playerInRange; // Store player reference when inside trigger

    void Start()
    {
        // Initialization if needed
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object is the player and store reference
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            playerInRange = player; // Store player reference
            Debug.Log("Player entered the trigger.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Clear player reference when they exit the trigger
        if (other.CompareTag("Player"))
        {
            playerInRange = null; // Clear the reference
            Debug.Log("Player exited the trigger.");
        }
    }

    // Method to damage the player, called from the animation event
    public void DamagePlayer()
    {
        if (playerInRange != null)
        {
            Debug.Log("Player hit! Applying damage.");
            playerInRange.TakeDamage(attackDamage); // Damage the player
        }
    }

    // Method to start the coroutine
    public void TriggerDamage()
    {
        StartCoroutine(DamageCoroutine());
    }

    // Coroutine to handle the damage
    private IEnumerator DamageCoroutine()
    {
        // Wait for the specific frame or time in the animation
        yield return new WaitForSeconds(0.5f); // Adjust the time as needed

        // Call the DamagePlayer method
        DamagePlayer();
    }
}
