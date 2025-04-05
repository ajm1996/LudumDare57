using UnityEngine;
using System.Collections.Generic;

public class TileGeneration : MonoBehaviour
{
    public GameObject chunkPrefab;
    public Camera mainCamera;
    
    // These values are now determined dynamically:
    private float chunkSize;    // World size of the chunk prefab.
    private float chunkWorldSize; // We'll set this equal to chunkSize.
    
    private HashSet<Vector2Int> spawnedChunkPositions = new HashSet<Vector2Int>();
    private Vector2Int lastCameraGridPosition;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        // Determine the chunk's world size from its prefab renderer.
        chunkSize = GetPrefabSize(chunkPrefab);
        chunkWorldSize = chunkSize; // Use the actual chunk size.

        lastCameraGridPosition = GetCameraChunkGridPosition();
        GenerateChunksAroundCamera();
    }

    float GetPrefabSize(GameObject prefab)
    {
        Renderer renderer = prefab.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds.size.x; // Assumes a square chunk.
        }
        Debug.LogWarning("Prefab has no renderer to determine size.");
        return 1f;
    }

    Vector2Int GetCameraChunkGridPosition()
    {
        Vector3 pos = mainCamera.transform.position;
        return new Vector2Int(
            Mathf.FloorToInt(pos.x / chunkWorldSize),
            Mathf.FloorToInt(pos.y / chunkWorldSize)
        );
    }

    void GenerateChunksAroundCamera()
    {
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        Vector2 camPos = mainCamera.transform.position;
        int padding = 2;

        int minChunkX = Mathf.FloorToInt((camPos.x - cameraWidth / 2) / chunkWorldSize) - padding;
        int maxChunkX = Mathf.CeilToInt((camPos.x + cameraWidth / 2) / chunkWorldSize) + padding;
        int minChunkY = Mathf.FloorToInt((camPos.y - cameraHeight / 2) / chunkWorldSize) - padding;
        int maxChunkY = Mathf.CeilToInt((camPos.y + cameraHeight / 2) / chunkWorldSize) + padding;

        for (int chunkX = minChunkX; chunkX <= maxChunkX; chunkX++)
        {
            for (int chunkY = minChunkY; chunkY <= maxChunkY; chunkY++)
            {
                Vector2Int chunkGridPos = new Vector2Int(chunkX, chunkY);
                if (chunkY <= 0 && !spawnedChunkPositions.Contains(chunkGridPos))
                {
                    Vector3 spawnPos = new Vector3(
                        chunkX * chunkWorldSize,
                        chunkY * chunkWorldSize,
                        0f
                    );
                    GameObject chunk = Instantiate(chunkPrefab, spawnPos, Quaternion.identity);
                    chunk.transform.parent = transform;
                    spawnedChunkPositions.Add(chunkGridPos);
                }
            }
        }
    }

    public void ClearChunks()
    {
        spawnedChunkPositions.Clear();
        foreach (Transform child in transform)
        {
            if (child != transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}