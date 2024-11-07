using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyDamagePlayer : MonoBehaviour
{
    public int attackDamage = 20;
    private Player playerInRange;

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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Clear player reference when they exit the trigger
        if (other.CompareTag("Player"))
        {
            playerInRange = null; // Clear the reference

        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Axe"))
        {
            if (playerInRange != null)
            {

                playerInRange.TakeDamage(attackDamage);
            }
        }
    }

    public void TriggerDamage()
    {
        StartCoroutine(DamageCoroutine());
    }

    // Coroutine to handle the damage
    private IEnumerator DamageCoroutine()
    {
        // Wait for the end of the frame
        yield return new WaitForEndOfFrame();


        DamagePlayer();
    }

    private void DamagePlayer()
    {
        if (playerInRange != null)
        {
            playerInRange.TakeDamage(attackDamage);
        }
    }
}
