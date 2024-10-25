using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamagePlayer : MonoBehaviour
{
    public int attackDamage = 5;
    private bool hasDamagedPlayer = false; // Flag to ensure player is damaged only once per collision

    private void OnTriggerEnter(Collider other)
    {
        // Check if the other object has a Player component
        Player player = other.GetComponent<Player>();

        if (player != null && !hasDamagedPlayer)
        {
            Debug.Log("Player hit!");
            // Apply damage to the player
            player.TakeDamage(attackDamage);
            hasDamagedPlayer = true; // Ensure player is damaged only once per collision
        }
        else if (other.CompareTag("Enemy"))
        {
            // Ignore collision with other enemies
            Physics.IgnoreCollision(GetComponent<Collider>(), other);
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
