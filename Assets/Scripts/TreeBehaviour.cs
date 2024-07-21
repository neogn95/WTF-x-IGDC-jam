using UnityEngine;

public class TreeBehavior : MonoBehaviour
{
    public GameObject stumpPrefab;
    public GameObject logPrefab;

    public void Interact(Vector2Int direction)
    {
        Vector2Int position = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        Vector2Int oppositePosition = position + direction;

        if (GameplayManager.Instance.GetGridObject(oppositePosition.x, oppositePosition.y) == null)
        {
            // Place stump
            GameObject stump = Instantiate(stumpPrefab, transform.position, Quaternion.identity);
            stump.tag = "LevelObject";
            GameplayManager.Instance.SetGridObject(position.x, position.y, stump);

            // Place log
            GameObject log = Instantiate(logPrefab, new Vector3(oppositePosition.x, 0, oppositePosition.y), Quaternion.identity);
            log.tag = "LevelObject";
            GameplayManager.Instance.SetGridObject(oppositePosition.x, oppositePosition.y, log);

            // Destroy tree
            Destroy(gameObject);
        }
    }
}