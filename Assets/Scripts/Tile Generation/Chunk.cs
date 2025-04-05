using UnityEngine;

public class Chunk : MonoBehaviour
{
    public GameObject smallTilePrefab;
    private bool hasSubdivided = false;
    public int gridSize = 10; // Or your desired subdivision count
    public Vector2Int gridPos; // Assigned by TileGeneration

    public void Subdivide()
    {
        if (hasSubdivided) return;
        hasSubdivided = true;

        float currentChunkSize = GetPrefabSize(gameObject);
        float smallTileSize = currentChunkSize / gridSize;
        Vector3 origin = transform.position - new Vector3(currentChunkSize / 2, currentChunkSize / 2, 0);

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector3 offset = new Vector3(x * smallTileSize + smallTileSize / 2, y * smallTileSize + smallTileSize / 2, 0);
                Vector3 spawnPos = origin + offset;
                GameObject tile = Instantiate(smallTilePrefab, spawnPos, Quaternion.identity);
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

    // Notify TileGeneration on destruction
    void OnDestroy()
    {
        if (TileGeneration.Instance != null)
        {
            TileGeneration.Instance.RemoveChunkAt(gridPos);
        }
    }
}
