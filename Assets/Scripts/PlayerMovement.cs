using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    
    // Private variables
    private Rigidbody2D rb;
    private bool isGrounded;
    private float horizontalInput;
    private bool jumpPressed;
    private bool isFacingRight = true;

    // Components caching
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        Flip();
    }

    private void CheckGrounded()
    {
        // Create a circle at groundCheck position to detect ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Move()
    {
        // Set velocity with current Y velocity preserved
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
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

    // Visualize the ground check radius in the editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
