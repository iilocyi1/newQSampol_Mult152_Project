using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public float damage = 30;
    private bool hasDamagedPlayer = false; // Flag to ensure player is damaged only once per collision

    private void OnTriggerEnter(Collider other)
    {
        // Check if the other object has a Player component
        Player player = other.GetComponent<Player>();

        if (player != null && !hasDamagedPlayer)
        {
            Debug.Log("Player hit!");
            // Apply damage to the player
            player.TakeDamage(damage);
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
