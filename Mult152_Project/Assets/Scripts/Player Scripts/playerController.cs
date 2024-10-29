using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public float jumpHeight = 10.0f;
    public float gravityModifier = 1.0f;
    public float speed = 2.35f;
    public float rollDistance = 2f;
    public float rollCooldown = 3f; // Cooldown duration in seconds
    public float rollSpeedMultiplier = 10.0f; // Speed multiplier for rolling
    private float lrInput;
    private float udInput;
    private bool jumpInput;
    private bool isInvulnerable = false;
    private bool canRoll = true; // Flag to check if roll is available
    public GameObject projectilePrefab; // The kunai prefab
    public Transform knifeSpawnPoint; // Transform for the knife spawn point
    private bool isGrounded = true;
    private bool isBlocking = false;
    private Rigidbody rb;
    public float fallMultiplier = 2.5f; // Multiplier to increase fall speed
    private PlayerInventory inventory;
    private Animator animator; // Reference to the Animator component
    private AudioSource audiosource;
    public AudioClip KnifeThrow;
    public ParticleSystem lightning;
    public ParticleSystem clouds;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Physics.gravity *= gravityModifier;
        inventory = GetComponent<PlayerInventory>();
        animator = GetComponent<Animator>(); // Initialize the Animator
        animator.SetBool("canRoll", true);
        lightning = GameObject.Find("Lightning").GetComponent<ParticleSystem>();
        clouds = GameObject.Find("Clouds").GetComponent<ParticleSystem>();
        lightning.Play();
        clouds.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isBlocking) // Check if not blocking
        {
            lrInput = Input.GetAxis("Horizontal");
            udInput = Input.GetAxis("Vertical");
            jumpInput = Input.GetKeyDown(KeyCode.Space);

            // Update Animator parameters
            animator.SetFloat("Speed", Mathf.Abs(lrInput) + Mathf.Abs(udInput));

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
                animator.SetTrigger("JumpTrigger"); // Trigger the jump animation
                animator.SetBool("IsJumping", true); // Set IsJumping to true
                isGrounded = false;
            }

            // Apply fall multiplier for quicker fall
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }

            // Trigger the throw animation
            if (Input.GetKeyDown(KeyCode.G))
            {
                animator.SetTrigger("KunaiThrow"); // Trigger the throw animation
                audiosource.PlayOneShot(KnifeThrow, 4f);
            }

            // Trigger the roll animation
            if (Input.GetKeyDown(KeyCode.C) && canRoll)
            {
                Debug.Log("Roll key pressed and canRoll is true"); // Debug log
                // Using coroutine to handle the timing of the invulnerability and cooldown
                StartCoroutine(Dodge());
                animator.SetTrigger("Roll");
                animator.SetBool("canRoll", false);
                animator.SetBool("IsDodging", true);
            }
        }

        // Check for interaction input
        if (Input.GetKeyDown(KeyCode.T))
        {
            Interact();
        }

        // Reset IsJumping when grounded
        if (isGrounded && !jumpInput)
        {
            animator.SetBool("IsJumping", false);
        }
    }

    private IEnumerator Dodge()
    {
        Vector3 rollDirection = transform.forward;
        float rollSpeed = rollSpeedMultiplier;
        float rollDuration = rollDistance / rollSpeed;
        float elapsedTime = 0f;

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

    public void ThrowKunai()
    {
        Debug.Log("Kunai instantiated!");
        Vector3 spawnPosition = knifeSpawnPoint.position;
        Quaternion spawnRotation = Quaternion.LookRotation(transform.forward); // Use player's forward direction
        Instantiate(projectilePrefab, spawnPosition, spawnRotation);
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
