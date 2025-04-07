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

    public float scrollPercentage = 20f;

    private Light2D light2D;

    public FuelManager fuelManager;

    void Start()
    {
        light2D = GetComponent<Light2D>();
    }

    void Update()
    {
        // Adjust scroll percentage with mouse scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f)
        {
            scrollPercentage += scroll * scrollSensitivity;
        }

        // Adjust scroll percentage with arrow keys
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            scrollPercentage += 10f;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            scrollPercentage -= 10f;
        }

        // Clamp scroll percentage between 0 and 100
        scrollPercentage = Mathf.Clamp(scrollPercentage, 0f, 100f);

        // Adjust intensity
        light2D.intensity = minIntensity + (maxIntensity - minIntensity) * scrollPercentage / 100f;

        // Adjust inner radius
        light2D.pointLightInnerRadius = minInnerRadius + (maxInnerRadius - minInnerRadius) * scrollPercentage / 100f;

        // Adjust outer radius
        light2D.pointLightOuterRadius = minOuterRadius + (maxOuterRadius - minOuterRadius) * scrollPercentage / 100f;

        // Adjust fuel consumption rate
        if (fuelManager != null)
        {
            float fuelRate = Mathf.Lerp(fuelManager.minFuelRate, fuelManager.maxFuelRate, scrollPercentage / 100f);
            fuelManager.passiveFuelRate = fuelRate;
        }
    }
}