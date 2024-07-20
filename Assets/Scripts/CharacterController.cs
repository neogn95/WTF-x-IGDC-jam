using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{
    [Header("Movement Type")]
    public bool useGridMovement = false;
    public float gridAlignedSpeed = 5f;

    [Header("Smooth Movement")]
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
        if (useGridMovement)
        {
            HandleGridAlignedInput();
        }
        else
        {
            HandleSmoothInput();
        }

        if (enableJump)
        {
            HandleJump();
        }
    }

    void FixedUpdate()
    {
        if (useGridMovement)
        {
            MoveGridAligned();
        }
        else
        {
            MoveSmoothly();
        }

        ApplyGravity();
    }

    void HandleSmoothInput()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = cameraTransform.right * moveHorizontal + cameraTransform.forward * moveVertical;
        movement.y = 0f;
        moveDirection = movement.normalized;
    }

    void MoveSmoothly()
    {
        Vector3 targetVelocity = moveDirection * moveSpeed;
        Vector3 velocityChange = targetVelocity - rb.velocity;
        velocityChange.y = 0f;
        rb.AddForce(velocityChange * acceleration, ForceMode.Acceleration);

        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }

    void HandleGridAlignedInput()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector3 cameraForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 cameraRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

        Vector3 movement = cameraRight * moveHorizontal + cameraForward * moveVertical;
        moveDirection = SnapToNearestAxis(movement);
    }

    void MoveGridAligned()
    {
        Vector3 targetVelocity = moveDirection * gridAlignedSpeed;
        Vector3 velocityChange = targetVelocity - rb.velocity;
        velocityChange.y = 0f;
        rb.AddForce(velocityChange * acceleration, ForceMode.Acceleration);

        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }

    Vector3 SnapToNearestAxis(Vector3 direction)
    {
        if (direction == Vector3.zero) return Vector3.zero;

        float x = Mathf.Abs(direction.x);
        float z = Mathf.Abs(direction.z);

        if (x > z)
        {
            return new Vector3(Mathf.Sign(direction.x), 0, 0);
        }
        else
        {
            return new Vector3(0, 0, Mathf.Sign(direction.z));
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