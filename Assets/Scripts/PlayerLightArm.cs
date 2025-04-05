using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class Light2DIntensityAndRadiusScroll : MonoBehaviour
{
    public float minIntensity = 0f;
    public float maxIntensity = 5f;
    public float minInnerRadius = 0f;
    public float maxInnerRadius = 10f;
    public float minOuterRadius = 25f;
    public float maxOuterRadius = 50f;
    public float scrollSensitivity = 1f;

    public float scrollPercentage = 25f;

    private Light2D light2D;

    public FuelManager fuelManager;

    void Start()
    {
        light2D = GetComponent<Light2D>();
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f)
        {
            scrollPercentage += scroll * scrollSensitivity;
            scrollPercentage = Mathf.Clamp(scrollPercentage, 0f, 100f);

            // Adjust intensity
            light2D.intensity = minIntensity + (maxIntensity - minIntensity) * scrollPercentage / 100f;

            // Adjust inner radius
            light2D.pointLightInnerRadius = minInnerRadius + (maxInnerRadius - minInnerRadius) * scrollPercentage / 100f;

            // Adjust outer radius
            light2D.pointLightOuterRadius = minOuterRadius + (maxOuterRadius - minOuterRadius) * scrollPercentage / 100f;

            // Get fuel manager from the player and set the fuel consumption rate based on the scroll percentage
            if (fuelManager != null)
            {
                float fuelRate = Mathf.Lerp(fuelManager.minFuelRate, fuelManager.maxFuelRate, scrollPercentage / 100f);
                fuelManager.passiveFuelRate = fuelRate;
            }
        }
    }
}