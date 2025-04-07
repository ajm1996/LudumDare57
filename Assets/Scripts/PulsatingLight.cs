using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PulsatingLight : MonoBehaviour
{
    public float minIntensity = 1.0f;
    public float maxIntensity = 2.0f;
    public float pulsationSpeed = 1.0f;

    private Light2D light2D;

    void Start()
    {
        light2D = GetComponent<Light2D>();
    }

    void Update()
    {
        if (light2D != null)
        {
            float pulsation = Mathf.Sin(Time.time * pulsationSpeed) * 0.5f + 0.5f; // Removed timeOffset
            light2D.intensity = Mathf.Lerp(minIntensity, maxIntensity, pulsation);
        }
    }
}