using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamagePlayer : MonoBehaviour
{
    public int attackDamage = 5;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the other object has a Player component
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            Debug.Log("Player hit!");
            // Apply damage to the player
            player.TakeDamage(attackDamage);
        }
        Destroy(gameObject);
    }
}
