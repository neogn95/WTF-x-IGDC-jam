using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{
    [Header("Movement")]
    public bool enableMovement = true;
    public float moveSpeed = 5f;
    public float acceleration = 50f;
    public float deceleration = 50f;
    public float airControl = 0.3f;

    [Header("Jump")]
    public bool enableJump = true;
    public float jumpForce = 7f;
    public float jumpCooldown = 0.1f;
    public float gravityMultiplier = 2.5f;

    [Header("Camera")]
    public Transform cameraTransform;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool isGrounded;
    private float lastJumpTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        if (enableMovement)
        {
            HandleInput();
        }

        if (enableJump)
        {
            HandleJump();
        }
    }

    void FixedUpdate()
    {
        if (enableMovement)
        {
            Move();
        }

        ApplyGravity();
    }

    void HandleInput()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = cameraTransform.right * moveHorizontal + cameraTransform.forward * moveVertical;
        movement.y = 0f;
        Vector3 targetDirection = movement.normalized;

        moveDirection = Vector3.Lerp(moveDirection, targetDirection, Time.deltaTime * (isGrounded ? acceleration : acceleration * airControl));
    }

    void Move()
    {
        Vector3 movement = moveDirection * moveSpeed;
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(rb.velocity, Vector3.up);
        rb.velocity = Vector3.Lerp(horizontalVelocity, movement, Time.fixedDeltaTime * (isGrounded ? acceleration : acceleration * airControl)) + Vector3.up * rb.velocity.y;

        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && Time.time > lastJumpTime + jumpCooldown)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            lastJumpTime = Time.time;
        }
    }

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            rb.AddForce(Physics.gravity * (gravityMultiplier - 1), ForceMode.Acceleration);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y > 0.7f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}