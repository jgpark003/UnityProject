using UnityEngine;

public class CameraFollowGrid : MonoBehaviour
{
    public Transform player;
    public Vector2 gridSize = new Vector2(16f, 9f); // 화면 사이즈 (기본 1920x1080 단위)
    public Vector2 gridOrigin = Vector2.zero;       // 시작 기준점 (예: 플레이어 시작 위치)

    private Vector2Int currentCell;

    void Start()
    {
        SnapToPlayerCell(force: true);
    }

    void LateUpdate()
    {
        Vector2 relativePos = (Vector2)player.position - gridOrigin;

        Vector2Int newCell = new Vector2Int(
            Mathf.FloorToInt(relativePos.x / gridSize.x),
            Mathf.FloorToInt(relativePos.y / gridSize.y)
        );

        if (newCell != currentCell)
        {
            currentCell = newCell;
            SnapToPlayerCell();
        }
    }

    void SnapToPlayerCell(bool force = false)
    {
        Vector2 bottomLeft = gridOrigin + new Vector2(
            currentCell.x * gridSize.x,
            currentCell.y * gridSize.y
        );

        Vector2 cameraCenter = bottomLeft + gridSize / 2f;

        Vector3 newPos = new Vector3(cameraCenter.x, cameraCenter.y, transform.position.z);

        if (force)
            transform.position = newPos;
        else
            StartCoroutine(SnapNextFrame(newPos));
    }

    System.Collections.IEnumerator SnapNextFrame(Vector2 target)
    {
        yield return null;
        transform.position = new Vector3(target.x, target.y, transform.position.z);
    }
}
