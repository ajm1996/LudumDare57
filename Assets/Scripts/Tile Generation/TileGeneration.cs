using UnityEngine;
using System.Collections.Generic;

public class TileGeneration : MonoBehaviour
{
    public static TileGeneration Instance;

    public GameObject chunkPrefab;
    public GameObject fuelPrefab;
    public GameObject dungeonPrefab;
    public Camera mainCamera;
    public float fuelChance = 0.001f;
    public float dungeonChance = 0.0001f;
    public float minDungeonDistance = 200f;

    // These values are determined dynamically:
    private float chunkSize;
    private float chunkWorldSize;

    private HashSet<Vector2Int> spawnedChunkPositions = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> minedChunkPositions = new HashSet<Vector2Int>(); // For mined chunks
    private HashSet<Vector2> dungeonPositions = new HashSet<Vector2>();
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

        // Add spawn point to dungeon positions to prevent dungeons from generating too close
        dungeonPositions.Add(Vector2.zero);

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

    Vector2 GetPrefabSize2D(GameObject prefab)
    {
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            Bounds combinedBounds = renderers[0].bounds;
            foreach (Renderer renderer in renderers)
            {
                combinedBounds.Encapsulate(renderer.bounds);
            }
            return new Vector2(combinedBounds.size.x, combinedBounds.size.y);
        }
        Debug.LogWarning("Prefab has no renderers to determine size.");
        return Vector2.one;
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

                    // Check for dungeon generation
                    if (Random.value < dungeonChance && !IsTooCloseToDungeon(spawnPos))
                    {
                        GameObject dungeon = Instantiate(dungeonPrefab, Vector2.zero, Quaternion.identity);
                        dungeon.transform.parent = transform;

                        Vector2 dungeonSize = GetPrefabSize2D(dungeon);

                        // 50% chance to flip it so that the dungeon isn't always spawning to the right
                        bool addChunks = Random.value < 0.5f;
                        float offsetX = chunkWorldSize * 12;  // Add a gap so the dungeon doesn't spawn right under the player
                        bool flipped = false;

                        if (addChunks)
                        {
                            spawnPos.x += offsetX;
                        }
                        else
                        {
                            spawnPos.x -= offsetX;
                            flipped = true;

                            // Flip the prefab horizontally
                            dungeon.transform.localScale = new Vector3(
                                dungeon.transform.localScale.x * -1,
                                dungeon.transform.localScale.y,
                                dungeon.transform.localScale.z
                            );

                            // Adjust position to account for the pivot being on the top-left
                            spawnPos.x -= dungeonSize.x;
                        }

                        // Adjust dungeon position so top-left corner aligns with spawnPos
                        dungeon.transform.position = new Vector3(
                            spawnPos.x,
                            spawnPos.y + (chunkWorldSize / 2),
                            spawnPos.z
                        );

                        ClearChunksForDungeon(dungeon.transform.position, dungeonSize, flipped);
                        dungeonPositions.Add(spawnPos);
                        continue;
                    }

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

    bool IsTooCloseToDungeon(Vector2 position)
    {
        foreach (Vector2 dungeonPos in dungeonPositions)
        {
            if (Vector2.Distance(position, dungeonPos) < minDungeonDistance)
                return true;
        }
        return false;
    }

    void ClearChunksForDungeon(Vector2 position, Vector2 dungeonSize, bool flipped)
    {
        // Adjust bounds based on the top-left alignment
        float maxXWorld;
        float minXWorld;
        if (flipped) {
            minXWorld = position.x - dungeonSize.x;
            maxXWorld = position.x;
        } else {
            minXWorld = position.x;
            maxXWorld = position.x + dungeonSize.x;
        }
        float minYWorld = position.y - dungeonSize.y;
        float maxYWorld = position.y;

        // If flipped, subtract the dungeon width from maxXWorld
        if (flipped)
        {
            maxXWorld = position.x;
            minXWorld = position.x - dungeonSize.x;
        }

        // All are off by 1
        int minX = Mathf.FloorToInt(minXWorld / chunkWorldSize) + 1;
        int maxX = Mathf.CeilToInt(maxXWorld / chunkWorldSize) - 1;
        int minY = Mathf.FloorToInt(minYWorld / chunkWorldSize) + 1;
        int maxY = Mathf.CeilToInt(maxYWorld / chunkWorldSize) - 1;

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                minedChunkPositions.Add(gridPos);
                spawnedChunkPositions.Remove(gridPos);

                // Destroy any existing chunks in this area
                foreach (Transform child in transform)
                {
                    Chunk chunk = child.GetComponent<Chunk>();
                    if (chunk != null && chunk.gridPos == gridPos)
                    {
                        Destroy(child.gameObject);
                    }
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