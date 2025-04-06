using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlickeringLight : MonoBehaviour
{
    public float baseIntensity = 1.0f;
    public float flickerChance = 0.05f;
    public float flickerDuration = 0.1f;
    public AudioSource flickerSound; // Add this line

    private Light2D light2D;
    private float flickerTimer = 0f;
    private float originalIntensity;

    void Start()
    {
        light2D = GetComponent<Light2D>();
        originalIntensity = baseIntensity;
    }

    void Update()
    {
        if (light2D != null)
        {
            if (flickerTimer > 0)
            {
                flickerTimer -= Time.deltaTime;
                if (flickerTimer <= 0)
                {
                    light2D.intensity = originalIntensity;
                    if (flickerSound != null)
                    {
                        flickerSound.mute = false; // Unmute sound
                    }
                }
            }
            else
            {
                if (Random.value < flickerChance)
                {
                    light2D.intensity = 0f;
                    flickerTimer = flickerDuration;
                    if (flickerSound != null)
                    {
                        flickerSound.mute = true; // Mute sound
                    }
                }
                else
                {
                    light2D.intensity = originalIntensity;
                }
            }
        }
    }
}