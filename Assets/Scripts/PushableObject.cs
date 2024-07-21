using UnityEngine;

public class PushableObject : MonoBehaviour
{
    public float moveTime = 0.2f;
    public LayerMask obstacleLayer;

    private bool isMoving = false;

    public bool TryPush(Vector3 direction)
    {
        if (isMoving)
            return false;

        Vector3 newPosition = transform.position + direction;

        // Check for obstacles in the push direction
        RaycastHit hit;
        if (!Physics.Raycast(transform.position, direction, out hit, 1f, obstacleLayer))
        {
            // No obstacle, start moving the object
            StartCoroutine(MoveToPosition(newPosition));
            return true;
        }

        return false;
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