using UnityEngine;

public class CameraFollowGrid : MonoBehaviour
{
    public Transform player;

    private Vector2 gridSize;
    private Vector2 gridOrigin;
    private Vector2Int currentCell;

    void Start()
    {
        CalculateGridSize();

        // 시작 카메라 위치를 기준 origin으로 설정
        gridOrigin = new Vector2(
            transform.position.x - gridSize.x / 2f,
            transform.position.y - gridSize.y / 2f
        );

        currentCell = Vector2Int.zero;

        SnapToCell(currentCell, force: true);
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
            SnapToCell(currentCell);
        }
    }

    void CalculateGridSize()
    {
        Camera cam = Camera.main;

        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * ((float)Screen.width / Screen.height);

        gridSize = new Vector2(camWidth, camHeight);
    }

    void SnapToCell(Vector2Int cell, bool force = false)
    {
        Vector2 bottomLeft = gridOrigin + new Vector2(
            cell.x * gridSize.x,
            cell.y * gridSize.y
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
