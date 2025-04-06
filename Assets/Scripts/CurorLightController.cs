using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CursorLightController : MonoBehaviour
{
    public Camera mainCamera;
    public Light2D cursorLight;
    public float maxBrightness = 2.0f; // Adjust this value to control the maximum brightness

    void Update()
    {
        if (mainCamera != null && cursorLight != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
            worldPosition.z = 0;
            cursorLight.transform.position = worldPosition;

            // Check for overlapping lights
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(worldPosition, cursorLight.pointLightOuterRadius);

            float highestIntensity = cursorLight.intensity; // Start with the cursor light's intensity

            foreach (Collider2D hitCollider in hitColliders)
            {
                Light2D otherLight = hitCollider.GetComponent<Light2D>();
                if (otherLight != null && otherLight != cursorLight)
                {
                    // Find the highest intensity
                    highestIntensity = Mathf.Max(highestIntensity, otherLight.intensity);
                }
            }

            // Cap the combined intensity
            cursorLight.intensity = Mathf.Min(highestIntensity, maxBrightness);
        }
    }
}