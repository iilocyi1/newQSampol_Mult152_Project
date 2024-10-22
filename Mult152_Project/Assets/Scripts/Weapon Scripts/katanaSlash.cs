using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class katanaSlash : MonoBehaviour
{
    public float attackRange = 1f;
    public int attackDamage = 50;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public Light attackFlashLight;
    public float flashDuration = 1f;
    private blockMechanic blockMechanic;
    public float attackCooldown = 0.75f; // Cooldown duration in seconds
    private bool canAttack = true; // Flag to check if attack is allowed

    public void Start()
    {
        blockMechanic = GetComponent<blockMechanic>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && canAttack && !blockMechanic.IsBlocking())
        {
            Slash();
        }
    }

    void Slash()
    {
        StartCoroutine(FlashLight());
        StartCoroutine(AttackCooldown());

        // Detect enemies in range of the attack
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        // Damage them
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                IDamageable damageable = enemy.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(attackDamage);
                    Debug.Log("Slash attack hit!");
                }
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
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
