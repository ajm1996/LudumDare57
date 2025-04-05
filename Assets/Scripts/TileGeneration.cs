using UnityEngine;
using System.Collections.Generic;

public class TileGeneration : MonoBehaviour
{
    public GameObject rockPrefab;
    public Camera mainCamera; // Assign the main camera in the Inspector
    public float spacing = 1.0f;

    void Start()
    {
        GenerateGridBasedOnCamera();
    }

    void GenerateGridBasedOnCamera()
    {
        if (rockPrefab == null)
        {
            Debug.LogError("Rock prefab is not assigned!");
            return;
        }

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is not assigned!");
            return;
        }

        // Calculate grid dimensions based on camera size
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        int gridWidth = Mathf.CeilToInt(cameraWidth / spacing);
        int gridHeight = Mathf.CeilToInt(cameraHeight / spacing);

        // Calculate the starting position of the grid (center it)
        Vector3 startPosition = mainCamera.transform.position - new Vector3(cameraWidth / 2f, cameraHeight / 2f, 0);

        // Adjust the startPosition to account for the spacing of the rocks.
        startPosition += new Vector3(spacing / 2f, spacing / 2f, 0);

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = startPosition + new Vector3(x * spacing, y * spacing, 0);
                GameObject rock = Instantiate(rockPrefab, position, Quaternion.identity);
                rock.transform.parent = transform;
            }
        }
    }

    public void ClearGrid()
    {
        foreach (Transform child in transform)
        {
            if (child != transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}