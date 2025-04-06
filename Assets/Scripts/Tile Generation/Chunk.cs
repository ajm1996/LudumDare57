using UnityEngine;

public class Chunk : MonoBehaviour
{
    public GameObject smallTilePrefab;  // For rock chunks
    public GameObject smallFuelPrefab;  // For fuel chunks
    public bool isFuel = false;         // Set this in TileGeneration.
    public float fuelAmount = 0.5f;       // (Optional: amount of fuel provided per small fuel piece, or use later on collection)

    public bool isSmallFuel = false;
    private bool hasSubdivided = false;
    public int gridSize = 10;           // Number of subdivisions per side
    public Vector2Int gridPos;          // Assigned by TileGeneration.

    public void Subdivide()
    {
        if (hasSubdivided) return;
        hasSubdivided = true;

        if (isSmallFuel) {
            FuelManager.Instance.AddFuel(fuelAmount);
            Destroy(gameObject);
            return;
        }
        // Remove the branch that instantly destroys fuel chunks.
        // Instead, we subdivide using the correct prefab.
        GameObject subdivisionPrefab = isFuel ? smallFuelPrefab : smallTilePrefab;

        float currentChunkSize = GetPrefabSize(gameObject);
        float smallTileSize = currentChunkSize / gridSize;
        Vector3 origin = transform.position - new Vector3(currentChunkSize / 2, currentChunkSize / 2, 0);

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector3 offset = new Vector3(x * smallTileSize + smallTileSize / 2, y * smallTileSize + smallTileSize / 2, 0);
                Vector3 spawnPos = origin + offset;
                GameObject tile = Instantiate(subdivisionPrefab, spawnPos, Quaternion.identity);
                tile.transform.localScale = Vector3.one * smallTileSize;
            }
        }
        Destroy(gameObject);
    }

    float GetPrefabSize(GameObject obj)
    {
        Renderer renderer = obj.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds.size.x;
        }
        Debug.LogWarning("Object has no renderer to determine size.");
        return 1f;
    }

    // Notify TileGeneration when this chunk is destroyed.
    void OnDestroy()
    {
        if (TileGeneration.Instance != null)
        {
            TileGeneration.Instance.RemoveChunkAt(gridPos);
        }
    }
}