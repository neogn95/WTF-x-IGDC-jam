using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{
    [Header("Movement Type")]
    public bool useGridMovement = false;
    public float gridSize = 1f;
    public float gridMoveSpeed = 5f;

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
    private bool isMoving = false;

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
            HandleGridInput();
        }
        else
        {
            HandleSmoothInput();
            if (enableJump)
            {
                HandleJump();
            }
        }
    }

    void FixedUpdate()
    {
        if (!useGridMovement)
        {
            MoveSmoothly();
            ApplyGravity();
        }
    }

    void HandleSmoothInput()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = cameraTransform.right * moveHorizontal + cameraTransform.forward * moveVertical;
        movement.y = 0f;
        Vector3 targetDirection = movement.normalized;

        moveDirection = Vector3.Lerp(moveDirection, targetDirection, Time.deltaTime * (isGrounded ? acceleration : acceleration * airControl));
    }

    void MoveSmoothly()
    {
        Vector3 movement = moveDirection * moveSpeed;
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(rb.velocity, Vector3.up);
        rb.velocity = Vector3.Lerp(horizontalVelocity, movement, Time.fixedDeltaTime * (isGrounded ? acceleration : acceleration * airControl)) + Vector3.up * rb.velocity.y;

        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }

    void HandleGridInput()
    {
        if (isMoving) return;

        Vector3 movement = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            movement = cameraTransform.forward;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            movement = -cameraTransform.forward;
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            movement = -cameraTransform.right;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            movement = cameraTransform.right;
        }

        if (movement != Vector3.zero)
        {
            movement.y = 0f; // Ensure movement is on the horizontal plane
            movement = movement.normalized;

            // Snap the movement to the nearest 90-degree angle
            Vector3 snappedMovement = SnapToNearestAxis(movement);

            Vector3 targetPosition = transform.position + snappedMovement * gridSize;
            StartCoroutine(MoveToGridPosition(targetPosition));
        }
    }

    Vector3 SnapToNearestAxis(Vector3 direction)
    {
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

    IEnumerator MoveToGridPosition(Vector3 targetPosition)
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        float journeyLength = Vector3.Distance(startPosition, targetPosition);
        float startTime = Time.time;

        while (transform.position != targetPosition)
        {
            float distanceCovered = (Time.time - startTime) * gridMoveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;
            transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
            yield return null;
        }

        isMoving = false;
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