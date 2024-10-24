using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roll : MonoBehaviour
{
    public float rollDistance = 2f;
    public float rollCooldown = 3f; // Cooldown duration in seconds
    private bool isInvulnerable = false;
    private bool canRoll = true; // Flag to check if roll is available
    public float rollSpeedMultiplier = 10.0f; // Speed multiplier for rolling
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("canRoll", true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && canRoll)
        {
            // Using coroutine to handle the timing of the invulnerability and cooldown
            StartCoroutine(Dodge());
           
        }
    }

    private IEnumerator Dodge()
    {
        Vector3 rollDirection = transform.forward;
        float rollSpeed = rollSpeedMultiplier;
        float rollDuration = rollDistance / rollSpeed;
        float elapsedTime = 0f;
        animator.SetTrigger("Roll");
        animator.SetBool("canRoll", false);
        animator.SetBool("IsDodging", true);

        // Make the character invulnerable
        isInvulnerable = true;

        while (elapsedTime < rollDuration)
        {
            transform.position += rollDirection * rollSpeed * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Make the character vulnerable again
        isInvulnerable = false;

        animator.SetBool("IsDodging", false);

        // Start the cooldown
        canRoll = false;
        yield return new WaitForSeconds(rollCooldown);
        canRoll = true;
        animator.SetBool("canRoll", true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isInvulnerable)
        {
            // Ignore collision
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }
        else
        {
            // Handle collision normally
        }
    }
}