using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyDamagePlayer : MonoBehaviour
{
    public int attackDamage = 20;

    private void OnTriggerEnter(Collider other)
    {
        if (enabled && other.CompareTag("Player"))
        {
            // Check if the other object has a Player component
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                Debug.Log("Applying damage.");
                // Apply damage to the player
                player.TakeDamage(attackDamage);
                // Call OnTriggerExit logic
                OnTriggerExit(other);
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (enabled && other.CompareTag("Player"))
        {
            // Add any logic needed when the player exits the damage zone
        }
    }
}
