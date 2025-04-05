using UnityEngine;
using System.Collections.Generic;

public class TileGeneration : MonoBehaviour
{
    public static TileGeneration Instance;

    public GameObject chunkPrefab;
    public GameObject fuelPrefab;
    public Camera mainCamera;
    public float fuelChance = 0.001f;

    // These values are determined dynamically:
    private float chunkSize;
    private float chunkWorldSize;
    
    private HashSet<Vector2Int> spawnedChunkPositions = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> minedChunkPositions = new HashSet<Vector2Int>(); // For mined chunks
    private Vector2Int lastCameraGridPosition;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        // Determine the chunk's world size from its prefab renderer.
        chunkSize = GetPrefabSize(chunkPrefab);
        chunkWorldSize = chunkSize; // Use the actual chunk size.

        lastCameraGridPosition = GetCameraChunkGridPosition();
        GenerateChunksAroundCamera();
    }
    
    void Update()
    {
        Vector2Int currentGridPos = GetCameraChunkGridPosition();
        if (currentGridPos != lastCameraGridPosition)
        {
            GenerateChunksAroundCamera();
            lastCameraGridPosition = currentGridPos;
        }
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
                // Only spawn if not already spawned and not mined.
                if (chunkY <= 0 && !spawnedChunkPositions.Contains(chunkGridPos) && !minedChunkPositions.Contains(chunkGridPos))
                {
                    Vector3 spawnPos = new Vector3(
                        chunkX * chunkWorldSize,
                        chunkY * chunkWorldSize,
                        0f
                    );
                    
                    GameObject chunk;

                    if (Random.value < fuelChance)
                    {
                        chunk = Instantiate(fuelPrefab, spawnPos, Quaternion.identity);
                    }
                    else
                    {
                        chunk = Instantiate(chunkPrefab, spawnPos, Quaternion.identity);
                    }
                    
                    chunk.transform.parent = transform;
    
                    // Set the grid coordinate and fuel flag on the chunk's component.
                    Chunk chunkScript = chunk.GetComponent<Chunk>();
                    if (chunkScript != null)
                    {
                        chunkScript.gridPos = chunkGridPos;
                    }
    
                    spawnedChunkPositions.Add(chunkGridPos);
                }
            }
        }
    }
    
    // Called by a chunk when it is destroyed (mined)
    public void RemoveChunkAt(Vector2Int gridPos)
    {
        spawnedChunkPositions.Remove(gridPos);
        minedChunkPositions.Add(gridPos); // Mark this grid position as mined so it doesn't regenerate
    }
    
    public void ClearChunks()
    {
        spawnedChunkPositions.Clear();
        minedChunkPositions.Clear();
        foreach (Transform child in transform)
        {
            if (child != transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}