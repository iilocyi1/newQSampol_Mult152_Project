using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingKnives : MonoBehaviour
{
    public float speed = 15.0f; // Speed at which the knife moves forward
    public int damage = 20; // Damage dealt by the knife
    private AudioSource audioSource;
    public AudioClip KnifeHit;


    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        // Move the knife forward based on its current rotation and speed
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Apply damage to the enemy using the IDamageable interface
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                audioSource.PlayOneShot(KnifeHit, 4f);
           
                Debug.Log("Knife hit an enemy!");
            }

            // Destroy the knife after hitting an enemy
            Destroy(gameObject);
        }
        else if (other.CompareTag("Obstacle"))
        {
            // Destroy the knife if it hits an obstacle
            Destroy(gameObject);
        }
    }
}
