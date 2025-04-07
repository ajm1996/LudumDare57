using UnityEngine;
using System.Collections.Generic;

public class LavaProgressionSystem : MonoBehaviour
{
    [Header("Lava Settings")]
    public GameObject lavaChunkPrefab;  // Prefab for the lava chunks
    public float lavaStartYPosition = -10f;  // Y position that triggers the lava system
    public int initialLavaOffset = 5;  // How many chunks above the trigger position to spawn the lava
    public float lavaSpeed = 1f;  // How fast the lava moves down (in seconds per chunk)
    
    [Header("References")]
    public Transform player;  // Reference to the player
    public TileGeneration tileGenerator;  // Reference to your TileGeneration script
    public string[] breakableTags = { "Breakable" };  // Tags for objects that should be deleted by lava
    
    // Internal tracking
    private float nextLavaMoveTime;
    private float currentLavaY;
    private int minX = 0;
    private int maxX = 0;
    private List<GameObject> currentLavaChunks = new List<GameObject>();
    private bool lavaActivated = false;
    private float chunkSize;
    
    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            if (player == null)
                Debug.LogError("Player not found! Please assign the player reference in the inspector.");
        }
        
        if (tileGenerator == null)
        {
            tileGenerator = FindObjectOfType<TileGeneration>();
            if (tileGenerator == null)
                Debug.LogError("TileGeneration not found! Please assign the TileGeneration reference in the inspector.");
        }
        
        // Get the chunk size from TileGeneration
        chunkSize = tileGenerator.chunkPrefab.GetComponent<Renderer>().bounds.size.x;
        
        // Initialize lava position (will be properly set when activated)
        nextLavaMoveTime = Time.time + lavaSpeed;
    }
    
    private void Update()
    {
        // Check if player has descended below the starting point to activate lava
        if (!lavaActivated && player.position.y < lavaStartYPosition)
        {
            // Set the initial lava position to be initialLavaOffset chunks above the trigger position
            currentLavaY = lavaStartYPosition + (initialLavaOffset * chunkSize);
            lavaActivated = true;
            SpawnLavaLine();
        }
        
        // If lava is active and it's time to move it down
        if (lavaActivated && Time.time > nextLavaMoveTime)
        {
            MoveLavaDown();
            nextLavaMoveTime = Time.time + lavaSpeed;
        }
        
        // Check if we need to expand lava line horizontally
        if (lavaActivated)
        {
            CheckForHorizontalExpansion();
        }
    }
    
    private void SpawnLavaLine()
    {
        // Clear any existing lava chunks
        ClearCurrentLavaChunks();
        
        // Update the minimum and maximum X coordinates
        UpdateHorizontalBounds();
        
        // Spawn a line of lava chunks at the current Y position
        for (int x = minX; x <= maxX; x++)
        {
            SpawnLavaChunkAt(x, Mathf.FloorToInt(currentLavaY / chunkSize));
        }
    }
    
    private void MoveLavaDown()
    {
        // Move the lava line down by one chunk
        currentLavaY -= chunkSize;
        
        // Clear the existing lava chunks
        ClearCurrentLavaChunks();
        
        // Update the horizontal bounds before spawning new chunks
        UpdateHorizontalBounds();
        
        // Spawn new lava chunks
        for (int x = minX; x <= maxX; x++)
        {
            SpawnLavaChunkAt(x, Mathf.FloorToInt(currentLavaY / chunkSize));
        }
        
        // Delete all chunks above the current lava line
        DeleteChunksAboveLava();
    }
    
    private void SpawnLavaChunkAt(int gridX, int gridY)
    {
        Vector3 position = new Vector3(gridX * chunkSize, gridY * chunkSize, 0);
        
        GameObject lavaChunk = Instantiate(lavaChunkPrefab, position, Quaternion.identity);
        lavaChunk.transform.parent = transform;
        
        // Add to our list for tracking
        currentLavaChunks.Add(lavaChunk);
        
        // If the lava chunk has a Chunk component, set its grid position
        Chunk chunkScript = lavaChunk.GetComponent<Chunk>();
        if (chunkScript != null)
        {
            chunkScript.gridPos = new Vector2Int(gridX, gridY);
        }
    }
    
    private void ClearCurrentLavaChunks()
    {
        foreach (GameObject chunk in currentLavaChunks)
        {
            if (chunk != null)
            {
                Destroy(chunk);
            }
        }
        
        currentLavaChunks.Clear();
    }
    
    private void UpdateHorizontalBounds()
    {
        // Find the leftmost and rightmost spawned chunks
        minX = int.MaxValue;
        maxX = int.MinValue;
        
        foreach (Transform child in tileGenerator.transform)
        {
            Chunk chunk = child.GetComponent<Chunk>();
            if (chunk != null)
            {
                minX = Mathf.Min(minX, chunk.gridPos.x);
                maxX = Mathf.Max(maxX, chunk.gridPos.x);
            }
        }
        
        // If no chunks are found, use default values
        if (minX == int.MaxValue || maxX == int.MinValue)
        {
            minX = -10;
            maxX = 10;
        }
        
        // Add some padding to ensure we cover a bit beyond the visible chunks
        minX -= 2;
        maxX += 2;
    }
    
    private void CheckForHorizontalExpansion()
    {
        int oldMinX = minX;
        int oldMaxX = maxX;
        
        UpdateHorizontalBounds();
        
        // If the bounds have expanded, respawn the lava line
        if (minX < oldMinX || maxX > oldMaxX)
        {
            // Only add new chunks at the expanded edges
            if (minX < oldMinX)
            {
                for (int x = minX; x < oldMinX; x++)
                {
                    SpawnLavaChunkAt(x, Mathf.FloorToInt(currentLavaY / chunkSize));
                }
            }
            
            if (maxX > oldMaxX)
            {
                for (int x = oldMaxX + 1; x <= maxX; x++)
                {
                    SpawnLavaChunkAt(x, Mathf.FloorToInt(currentLavaY / chunkSize));
                }
            }
        }
    }
    
    private void DeleteChunksAboveLava()
    {
        int lavaGridY = Mathf.FloorToInt(currentLavaY / chunkSize);
        float lavaWorldY = currentLavaY;
        
        // Create a list to hold objects to be destroyed
        List<GameObject> objectsToDestroy = new List<GameObject>();
        
        // Find all objects with breakable tags above the lava
        foreach (string tag in breakableTags)
        {
            GameObject[] breakableObjects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in breakableObjects)
            {
                // Check if this object is above the lava line
                if (obj.transform.position.y > lavaWorldY && !currentLavaChunks.Contains(obj))
                {
                    // Don't add the same object twice
                    if (!objectsToDestroy.Contains(obj))
                    {
                        objectsToDestroy.Add(obj);
                    }
                }
            }
        }
        
        // Destroy all marked objects
        foreach (GameObject obj in objectsToDestroy)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
    }
}