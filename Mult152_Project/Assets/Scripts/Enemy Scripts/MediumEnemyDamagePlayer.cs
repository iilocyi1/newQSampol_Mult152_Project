using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumEnemyDamagePlayer : MonoBehaviour
{
    public int attackDamage = 10;
    private bool hasDamagedPlayer = false; // Flag to ensure player is damaged only once per attack

    private void OnTriggerEnter(Collider other)
    {
        // Check if the other object has a Player component
        Player player = other.GetComponent<Player>();
        SimpleEnemyAI enemyAI = GetComponentInParent<SimpleEnemyAI>();

        if (player != null && enemyAI != null && enemyAI.IsAttacking() && !hasDamagedPlayer)
        {
            Debug.Log("Player hit!");
            // Apply damage to the player
            player.TakeDamage(attackDamage);
            hasDamagedPlayer = true; // Ensure player is damaged only once per attack
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Reset the damage flag when the player exits the trigger
        if (other.CompareTag("Player"))
        {
            hasDamagedPlayer = false;
        }
    }
}
