using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class Light2DIntensityAndRadiusScroll : MonoBehaviour
{
    public float minIntensity = 0f;
    public float maxIntensity = 5f;
    public float minInnerRadius = 0f;
    public float maxInnerRadius = 10f;
    public float scrollSensitivity = 1f;
    public float radiusSensitivity = 1f;

    private Light2D light2D;

    void Start()
    {
        light2D = GetComponent<Light2D>();
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f)
        {
            // Adjust intensity
            float newIntensity = light2D.intensity + scroll * scrollSensitivity;
            light2D.intensity = Mathf.Clamp(newIntensity, minIntensity, maxIntensity);

            // Adjust inner radius
            float newInnerRadius = light2D.pointLightInnerRadius + scroll;// * radiusSensitivity;
            light2D.pointLightInnerRadius = Mathf.Clamp(newInnerRadius, minInnerRadius, maxInnerRadius);
        }
    }
}