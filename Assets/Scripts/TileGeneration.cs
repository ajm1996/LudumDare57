using UnityEngine;
using System.Collections.Generic;

public class TileGeneration : MonoBehaviour
{
    public GameObject rockPrefab;
    public Camera mainCamera;
    private HashSet<Vector2Int> spawnedPositions = new HashSet<Vector2Int>();
    private Vector2Int lastCameraGridPosition;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        lastCameraGridPosition = Vector2Int.FloorToInt(mainCamera.transform.position);
        GenerateGridAroundCamera();
    }

    void Update()
    {
        Vector2Int currentCameraGridPos = Vector2Int.FloorToInt(mainCamera.transform.position);
        if (currentCameraGridPos != lastCameraGridPosition)
        {
            GenerateGridAroundCamera();
            lastCameraGridPosition = currentCameraGridPos;
        }
    }

    void GenerateGridAroundCamera()
    {
        if (rockPrefab == null || mainCamera == null) return;

        // Calculate the visible area in grid coordinates
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        Vector2 camPos = mainCamera.transform.position;
        
        // Add some padding to prevent visible edges
        int padding = 2;
        int minX = Mathf.FloorToInt(camPos.x - (cameraWidth/2)) - padding;
        int maxX = Mathf.CeilToInt(camPos.x + (cameraWidth/2)) + padding;
        int minY = Mathf.FloorToInt(camPos.y - (cameraHeight/2)) - padding;
        int maxY = Mathf.CeilToInt(camPos.y + (cameraHeight/2)) + padding;

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                if (y <= 0 && !spawnedPositions.Contains(gridPos))
                {
                    GameObject rock = Instantiate(rockPrefab, new Vector3(x, y, 0), Quaternion.identity);
                    rock.transform.parent = transform;
                    spawnedPositions.Add(gridPos);
                }
            }
        }
    }

    public void ClearGrid()
    {
        spawnedPositions.Clear();
        foreach (Transform child in transform)
        {
            if (child != transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}