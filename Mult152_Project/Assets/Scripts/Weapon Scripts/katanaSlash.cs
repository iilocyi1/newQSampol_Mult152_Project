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
    private HashSet<Collider> hitEnemies = new HashSet<Collider>(); // Track hit enemies

    public void Start()
    {
        blockMechanic = GetComponent<blockMechanic>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && canAttack && !blockMechanic.IsBlocking())
        {
            Slash();
            animator.SetTrigger("Attack");
        }
    }

    void Slash()
    {
        StartCoroutine(FlashLight());
        StartCoroutine(AttackCooldown());
        hitEnemies.Clear(); // Clear the set at the start of each attack
        isAttacking = true; // Set the flag to true when the attack starts
    }

    void OnTriggerEnter(Collider other)
    {
        if (isAttacking && other.CompareTag("Enemy") && !hitEnemies.Contains(other))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
                hitEnemies.Add(other); // Add the enemy to the set
                Debug.Log("Slash attack hit!");
            }
        }
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
        Gizmos.DrawWireMesh(GetComponent<MeshCollider>().sharedMesh, transform.position, transform.rotation);
    }
}
