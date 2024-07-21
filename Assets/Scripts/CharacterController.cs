using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float moveTime = 0.2f;
    public LayerMask obstacleLayer;

    private Vector3 targetPosition;
    private bool isMoving = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        targetPosition = transform.position;
    }

    void Update()
    {
        if (!isMoving)
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Vector3 movement = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            movement = Vector3.forward;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            movement = Vector3.back;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            movement = Vector3.left;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            movement = Vector3.right;

        if (movement != Vector3.zero)
        {
            TryMove(movement);
        }
    }

    void TryMove(Vector3 direction)
    {
        Vector3 newPosition = transform.position + direction;

        // Check for obstacles
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 1f, obstacleLayer))
        {
            PushableObject pushable = hit.collider.GetComponent<PushableObject>();
            if (pushable != null && pushable.TryPush(direction))
            {
                // If the object was successfully pushed, move the player
                StartCoroutine(MoveToPosition(newPosition));
            }
        }
        else
        {
            // No obstacle, move the player
            StartCoroutine(MoveToPosition(newPosition));
        }
    }

    System.Collections.IEnumerator MoveToPosition(Vector3 newPosition)
    {
        isMoving = true;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < moveTime)
        {
            transform.position = Vector3.Lerp(startPosition, newPosition, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = newPosition;
        isMoving = false;
    }
}