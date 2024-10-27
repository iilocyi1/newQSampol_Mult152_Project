using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class katanaSlash : MonoBehaviour
{
    public int attackDamage = 50;
    public Light attackFlashLight;
    public float flashDuration = 1f;
    private blockMechanic blockMechanic;
    public float attackCooldown = 0.75f; // Cooldown duration in seconds
    private bool canAttack = true; // Flag to check if attack is allowed
    private Animator animator;
    public static bool isAttacking = false; // Flag to indicate if an attack is in progress
    public AudioClip hitSound;  // Audio clip to play when hitting an enemy
    public AudioClip attackSound;
    private AudioSource audioSource;

    void Start()
    {
        blockMechanic = GetComponent<blockMechanic>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();  // Get the AudioSource component
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && canAttack && !blockMechanic.IsBlocking())
        {
            Slash();
            animator.SetTrigger("Attack");
            audioSource.PlayOneShot(attackSound);
        }
    }

    void Slash()
    {
        StartCoroutine(FlashLight());
        StartCoroutine(AttackCooldown());
        isAttacking = true; // Set the flag to true when the attack starts

        // Find the closest enemy
        Collider closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            IDamageable damageable = closestEnemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
                Debug.Log("Slash attack hit the closest enemy!");

                // Play the hit sound
                if (audioSource != null && hitSound != null)
                {
                    audioSource.PlayOneShot(hitSound, 4f);
                }
            }
        }
    }

    Collider FindClosestEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1f); // Adjust the radius as needed
        Collider closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = collider;
                }
            }
        }

        return closestEnemy;
    }

    IEnumerator FlashLight()
    {
        attackFlashLight.enabled = true;
        yield return new WaitForSeconds(flashDuration);
        attackFlashLight.enabled = false;
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        isAttacking = false; // Reset the flag after the cooldown
    }

    void OnDrawGizmosSelected()
    {
        // Optional: Visualize the collider in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f); // Adjust the radius as needed
    }
}
