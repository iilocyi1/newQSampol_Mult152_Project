using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detectCollisions : MonoBehaviour
{
    public int attackDamage = 20;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy hit!");
            // Apply damage to the enemy using the IDamageable interface
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
            }
        }

    }
}
