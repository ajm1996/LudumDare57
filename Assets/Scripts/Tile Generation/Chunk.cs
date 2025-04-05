using UnityEngine;

public class Chunk : MonoBehaviour
{
    public GameObject smallTilePrefab;
    private bool hasSubdivided = false;
    public int gridSize = 5; // Number of subdivisions per side

    public void Subdivide()
    {
        if (hasSubdivided) return;
        hasSubdivided = true;

        // Get this chunk's size from its renderer.
        float currentChunkSize = GetPrefabSize(gameObject); // The current chunk's world size.
        float smallTileSize = currentChunkSize / gridSize;
        Vector3 origin = transform.position - new Vector3(currentChunkSize / 2, currentChunkSize / 2, 0);

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                // Position each small tile centered in its grid cell.
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
}