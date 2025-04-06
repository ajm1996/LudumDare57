using UnityEngine;
using UnityEngine.Audio;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Sprites")]
    [SerializeField] private Sprite FacingCamera;
    [SerializeField] private Sprite FacingLeft;
    [SerializeField] private Sprite FacingRight;
    
    // Private variables
    private Rigidbody2D rb;
    private AudioSource[] audiosource;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform MiningArmTransform;
    [SerializeField] private SpriteRenderer MiningArmSprite;
    [SerializeField] private Transform FlashlightArmTransform;
    [SerializeField] private SpriteRenderer FlashlightArmSprite;
    private bool isGrounded;
    private float horizontalInput;
    private bool jumpPressed;
    private bool isFacingRight = true;
    private float footstepTime = 0.8f;

    // Components caching
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Audiosource array positions
        // 0 = Footstep 1 = DrillWindup
        audiosource = GetComponents<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Input handling
    private void Update()
    {
        // Get horizontal movement input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        
        // Check for jump input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpPressed = true;
        }
    }

    // Physics handling
    private void FixedUpdate()
    {
        // Check if player is grounded
        CheckGrounded();
        
        // Move the player
        Move();
        
        // Handle jumping
        Jump();
        
        // Flip the player sprite based on movement direction
        //Flip();

        UpdateSprite();
    }

    private void CheckGrounded()
    {
        // Create a circle at groundCheck position to detect ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Move()
    {
        // Calculate target speed
        float targetSpeed = horizontalInput * moveSpeed;
        
        // Calculate the difference between current and desired velocity
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        
        // Apply acceleration in the direction we want to move
        float movement = speedDiff * 5f; // 5f is an acceleration factor
        
        // Apply the force
        rb.AddForce(movement * Vector2.right);

        // Track when to play footstep sounds when moving
        if(rb.linearVelocityX != 0 && isGrounded)
        {
            footstepTime -= Time.deltaTime;

            if(footstepTime < 0)
            {
                audiosource[0].Play();
                footstepTime = 0.8f;
            }
        }
        
        // Clamp the velocity to prevent excessive speed
        float maxSpeed = moveSpeed;
        rb.linearVelocity = new Vector2(
            Mathf.Clamp(rb.linearVelocity.x, -maxSpeed, maxSpeed),
            rb.linearVelocity.y
        );
    }

    private void Jump()
    {
        if (jumpPressed)
        {
            // Apply jump force
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpPressed = false;
        }
    }

    private void Flip()
    {
        // If moving right but facing left, or moving left but facing right, flip the sprite
        if ((horizontalInput > 0 && !isFacingRight) || (horizontalInput < 0 && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void UpdateSprite()
    {
        if (rb.linearVelocityX == 0 && horizontalInput == 0)
        {
            spriteRenderer.sprite = FacingCamera;
            MiningArmSprite.sortingOrder = 2;
            FlashlightArmSprite.sortingOrder = 2;
            MiningArmTransform.transform.localPosition = new Vector3(0.25f, 0.7f);            
            FlashlightArmTransform.transform.localPosition = new Vector3(-0.25f, 0.7f);
        }
        else if(rb.linearVelocityX < 0 && horizontalInput < 0)
        {
            spriteRenderer.sprite = FacingLeft;
            MiningArmSprite.sortingOrder = 2;
            FlashlightArmSprite.sortingOrder = 0;
            MiningArmTransform.transform.localPosition = new Vector3(0, 0.7f);
            FlashlightArmTransform.transform.localPosition = new Vector3(0, 0.7f);
        }
        else if(horizontalInput > 0)
        {
            spriteRenderer.sprite = FacingRight;
            MiningArmSprite.sortingOrder = 0;
            FlashlightArmSprite.sortingOrder = 2;
            MiningArmTransform.transform.localPosition = new Vector3(0, 0.7f);
            FlashlightArmTransform.transform.localPosition = new Vector3(0, 0.7f);
        }
    }

    // Visualize the ground check radius in the editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
