using UnityEngine;

public class LogBehavior : MonoBehaviour
{
    public void Roll(Vector2Int direction)
    {
        Vector2Int position = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        Vector2Int nextPosition = position + direction;

        while (GameplayManager.Instance.GetGridObject(nextPosition.x, nextPosition.y) == null)
        {
            nextPosition += direction;
        }
        nextPosition -= direction;  

        if (nextPosition != position)
        {
            GameplayManager.Instance.SetGridObject(position.x, position.y, null);
            GameplayManager.Instance.SetGridObject(nextPosition.x, nextPosition.y, gameObject);
            transform.position = new Vector3(nextPosition.x, 0, nextPosition.y);
        }
    }

    public void Rotate(Vector2Int direction)
    {
        Vector2Int position = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        Vector2Int nextPosition = position + direction;

        if (GameplayManager.Instance.GetGridObject(nextPosition.x, nextPosition.y) == null)
        {
            GameplayManager.Instance.SetGridObject(position.x, position.y, null);
            GameplayManager.Instance.SetGridObject(nextPosition.x, nextPosition.y, gameObject);
            transform.position = new Vector3(nextPosition.x, 0, nextPosition.y);
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 90);
        }
    }
}