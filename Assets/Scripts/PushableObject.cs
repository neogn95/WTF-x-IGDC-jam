using UnityEngine;
using System.Collections;

public class PushableObject : MonoBehaviour
{
    public float moveTime = 0.2f;
    public LayerMask obstacleLayer;
    public GameObject stumpPrefab;
    public GameObject logPrefab;

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
            StartCoroutine(MoveToPosition(newPosition, direction));
            return true;
        }

        return false;
    }

    IEnumerator MoveToPosition(Vector3 newPosition, Vector3 direction)
    {
        isMoving = true;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        // If this is a tree, spawn a stump at the original position
        if (gameObject.name.Contains("Tree_Brown") || gameObject.name.Contains("Tree_Green"))
        {
            Instantiate(stumpPrefab, startPosition, Quaternion.identity);
        }

        while (elapsedTime < moveTime)
        {
            transform.position = Vector3.Lerp(startPosition, newPosition, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = newPosition;
        isMoving = false;

        // If this is a tree, replace it with a log
        if (gameObject.name.Contains("Tree_Brown") || gameObject.name.Contains("Tree_Green"))
        {
            Instantiate(logPrefab, newPosition, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}