using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float gravity = 20f;
    [SerializeField] private float slopeForce = 5f;
    [SerializeField] private float airControl = 0.5f;
    
    [Header("Ground Check")]
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    
    private Rigidbody2D rb;
    private bool isGrounded;
    private float currentSlope;
    private Vector2 velocity;
    
    private void Awake()
    {
        // Get or add Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        
        // Configure Rigidbody2D
        rb.gravityScale = 0f; // We'll handle gravity manually
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Setup ground check if not assigned
        if (groundCheck == null)
        {
            GameObject check = new GameObject("GroundCheck");
            check.transform.SetParent(transform);
            check.transform.localPosition = new Vector3(0, -0.5f, 0); // Position below the player
            groundCheck = check.transform;
            Debug.Log("Created GroundCheck object. Make sure to adjust its position in the inspector.");
        }
    }
    
    private void Update()
    {
        // Check if we're grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        // Handle input
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                // Apply upward force when on ground
                velocity.y = maxSpeed;
            }
        }
    }
    
    private void FixedUpdate()
    {
        // Apply gravity
        if (!isGrounded)
        {
            velocity.y -= gravity * Time.fixedDeltaTime;
        }
        
        // Apply slope force when grounded
        if (isGrounded)
        {
            // Calculate slope force based on the angle of the ground
            float slopeAngle = Mathf.Atan2(currentSlope, 1f) * Mathf.Rad2Deg;
            float slopeForceMultiplier = Mathf.Sin(slopeAngle * Mathf.Deg2Rad);
            velocity.x += slopeForce * slopeForceMultiplier * Time.fixedDeltaTime;
        }
        
        // Clamp velocity
        velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        velocity.y = Mathf.Clamp(velocity.y, -maxSpeed, maxSpeed);
        
        // Apply velocity
        rb.velocity = velocity;
    }
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Calculate the slope of the ground we're on
        if (collision.contacts.Length > 0)
        {
            ContactPoint2D contact = collision.contacts[0];
            currentSlope = contact.normal.x / contact.normal.y;
        }
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Reset slope when leaving ground
        currentSlope = 0f;
    }

    private void OnDrawGizmos()
    {
        // Draw ground check radius in editor
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
} 