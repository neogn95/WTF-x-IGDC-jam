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

    // New variables for grid-based gameplay
    private bool isMovingOnGrid = false;
    private Vector3 gridMoveTarget;

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
            if (isMovingOnGrid)
            {
                MoveTowardsGridTarget();
            }
            else
            {
                MoveGridAligned();
            }
        }
        else
        {
            MoveSmoothly();
        }

        ApplyGravity();
    }

    void HandleSmoothInput()
    {
        // Existing smooth input handling code
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = cameraTransform.right * moveHorizontal + cameraTransform.forward * moveVertical;
        movement.y = 0f;
        moveDirection = movement.normalized;
    }

    void MoveSmoothly()
    {
        // Existing smooth movement code
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
        if (isMovingOnGrid) return;

        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        if (moveHorizontal != 0 || moveVertical != 0)
        {
            Vector3 cameraForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

            Vector3 movement = cameraRight * moveHorizontal + cameraForward * moveVertical;
            moveDirection = SnapToNearestAxis(movement);

            Vector2Int gridDirection = new Vector2Int(Mathf.RoundToInt(moveDirection.x), Mathf.RoundToInt(moveDirection.z));
            Vector2Int currentPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));

            if (GameplayManager.Instance.TryMovePlayer(currentPosition, gridDirection))
            {
                isMovingOnGrid = true;
                gridMoveTarget = transform.position + moveDirection;
            }
        }
    }

    void MoveGridAligned()
    {
        // Existing grid-aligned movement code
        Vector3 targetVelocity = moveDirection * gridAlignedSpeed;
        Vector3 velocityChange = targetVelocity - rb.velocity;
        velocityChange.y = 0f;
        rb.AddForce(velocityChange * acceleration, ForceMode.Acceleration);

        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }

    void MoveTowardsGridTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, gridMoveTarget, gridAlignedSpeed * Time.fixedDeltaTime);

        if (Vector3.Distance(transform.position, gridMoveTarget) < 0.01f)
        {
            transform.position = gridMoveTarget;
            isMovingOnGrid = false;
        }
    }

    Vector3 SnapToNearestAxis(Vector3 direction)
    {
        // Existing SnapToNearestAxis code
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
        // Existing jump handling code
        if (Input.GetButtonDown("Jump") && isGrounded && Time.time > lastJumpTime + jumpCooldown)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            lastJumpTime = Time.time;
        }
    }

    void ApplyGravity()
    {
        // Existing gravity application code
        if (!isGrounded)
        {
            rb.AddForce(Physics.gravity * (gravityMultiplier - 1), ForceMode.Acceleration);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // Existing ground check code
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
        // Existing ground check code
        isGrounded = false;
    }
}