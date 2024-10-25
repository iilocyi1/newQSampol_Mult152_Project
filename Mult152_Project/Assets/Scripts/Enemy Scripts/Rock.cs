using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public int damage = 10;
    public float speed = 6f; // Speed at which the rock moves forward

    private void Start()
    {
        // Apply initial force to the rock to move it forward
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(transform.forward * speed, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has a Player component
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            // Apply damage to the player
            player.TakeDamage(damage);
        }

        // Ensure the rock stays on the ground after hitting something
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }
}
