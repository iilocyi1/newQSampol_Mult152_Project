using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public float jumpHeight = 10.0f;
    public float gravityModifier = 1.0f;
    public float speed = 2.0f;
    private float lrInput;
    private float udInput;
    private bool jumpInput;
    public GameObject projectilePrefab;
    public Transform knifeSpawnPoint; // Transform for the knife spawn point
    private bool isGrounded = true;
    private bool isBlocking = false;
    private Rigidbody rb;
    public float fallMultiplier = 2.5f; // Multiplier to increase fall speed
    private int projectileCount = 0; // Counter for projectiles fired
    private bool canFire = true; // Flag to check if firing is allowed
    public float fireCooldown = 2.5f; // Cooldown duration in seconds
    private PlayerInventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Physics.gravity *= gravityModifier;
        inventory = GetComponent<PlayerInventory>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isBlocking)
        {
            lrInput = Input.GetAxis("Horizontal");
            udInput = Input.GetAxis("Vertical");
            jumpInput = Input.GetKeyDown(KeyCode.Space);

            // Calculate movement direction
            Vector3 movementDirection = new Vector3(-udInput, 0, lrInput).normalized;

            // Rotate the character based on movement direction
            if (movementDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(movementDirection);
            }

            // Translate the character in the direction it is facing
            Vector3 movement = movementDirection * Time.fixedDeltaTime * speed;
            transform.Translate(movement, Space.World);

            isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

            // Handle jumping
            if (isGrounded && jumpInput)
            {
                rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                isGrounded = false;
            }

            // Apply fall multiplier for quicker fall
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }

            // Instantiate projectile
            if (Input.GetKeyDown(KeyCode.G) && canFire)
            {
                Debug.Log("Projectile fired!");
                Vector3 spawnPosition = knifeSpawnPoint.position;
                Quaternion spawnRotation = knifeSpawnPoint.rotation;
                Instantiate(projectilePrefab, spawnPosition, spawnRotation);

                projectileCount++;

                if (projectileCount >= 3)
                {
                    StartCoroutine(FireCooldown());
                }
            }
        }

        // Check for interaction input
        if (Input.GetKeyDown(KeyCode.T))
        {
            Interact();
        }
    }

    private IEnumerator FireCooldown()
    {
        canFire = false;
        yield return new WaitForSeconds(fireCooldown);
        canFire = true;
        projectileCount = 0; // Reset the counter after cooldown
    }

    public void SetBlockingState(bool blocking)
    {
        isBlocking = blocking;

        if (blocking)
        {
            speed = 0;
        }
        else
        {
            speed = 2.0f;
        }
    }

    private void Interact()
    {
        // Interact with NPCs, pick up items, and use keys
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))
        {
            if (hit.collider.CompareTag("NPC"))
            {
                Debug.Log("You have interacted with NPC");
                hit.collider.GetComponent<NPCDialogue>().Interact();
            }
            else if (hit.collider.CompareTag("Key"))
            {
                hit.collider.GetComponent<Key>().PickUp();
            }
            else if (hit.collider.CompareTag("Gate"))
            {
                if (inventory.UseKey())
                {
                    hit.collider.GetComponent<Gate>().OpenGate();
                }
            }
        }
    }
}
