using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class FuelManager : MonoBehaviour
{
    public static FuelManager Instance;  // Singleton instance

    [Header("Fuel Settings")]
    [SerializeField] public float fossilFuelLevel = 100f;
    [SerializeField] public float fossilFuelMax = 100f;
    [SerializeField] public float passiveFuelRate = 0.3f;
    [SerializeField] public float maxFuelRate = 1f;
    [SerializeField] public float minFuelRate = 0.1f;

    [Header("Battery Sprites")]
    [SerializeField] private Sprite[] batterySprites; // Array of 6 battery sprites
    [SerializeField] private SpriteRenderer batterySpriteRenderer; // SpriteRenderer for the battery

    [Header("Battery Lights")]
    [SerializeField] private Light2D[] batteryLights; // Array of 5 lights corresponding to battery levels

    private bool isBlinking = false;

    void Awake()
    {
        // Set up the singleton instance.
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Decrease fuel over time
        fossilFuelLevel -= passiveFuelRate * Time.deltaTime;

        // Update the battery sprite and lights based on the fuel level
        UpdateBatterySpriteAndLights();

        // Restart the game if fuel is depleted
        if (fossilFuelLevel <= 0)
        {
            SceneManager.LoadScene(0);
        }
    }

    public void AddFuel(float fuelAmount)
    {
        fossilFuelLevel = Mathf.Min(fossilFuelLevel + fuelAmount, fossilFuelMax);
    }

    public void RemoveFuel(float fuelAmount)
    {
        fossilFuelLevel = Mathf.Max(fossilFuelLevel - fuelAmount, 0);
    }

    private void UpdateBatterySpriteAndLights()
    {
        if (batterySprites == null || batterySprites.Length < 6 || batterySpriteRenderer == null || batteryLights == null || batteryLights.Length < 5)
            return;

        float fuelPercentage = fossilFuelLevel / fossilFuelMax;

        if (fuelPercentage > 0.8f)
        {
            batterySpriteRenderer.sprite = batterySprites[0]; // 100% - 81%
            UpdateBatteryLights(5); // All lights on
            isBlinking = false;
        }
        else if (fuelPercentage > 0.6f)
        {
            batterySpriteRenderer.sprite = batterySprites[1]; // 80% - 61%
            UpdateBatteryLights(4); // 4 lights on
            isBlinking = false;
        }
        else if (fuelPercentage > 0.4f)
        {
            batterySpriteRenderer.sprite = batterySprites[2]; // 60% - 41%
            UpdateBatteryLights(3); // 3 lights on
            isBlinking = false;
        }
        else if (fuelPercentage > 0.2f)
        {
            batterySpriteRenderer.sprite = batterySprites[3]; // 40% - 21%
            UpdateBatteryLights(2); // 2 lights on
            isBlinking = false;
        }
        else if (fuelPercentage > 0.05f)
        {
            batterySpriteRenderer.sprite = batterySprites[4]; // 20% - 6%
            UpdateBatteryLights(1); // 1 light on
            isBlinking = false;
        }
        else
        {
            // 5% or less: Blink between the last two sprites
            if (!isBlinking)
            {
                StartCoroutine(BlinkBattery());
            }
        }
    }

    private void UpdateBatteryLights(int activeLights)
    {
        for (int i = 0; i < batteryLights.Length; i++)
        {
            if (i < activeLights)
            {
                batteryLights[i].enabled = true; // Turn on the light
            }
            else
            {
                batteryLights[i].enabled = false; // Turn off the light
            }
        }
    }

    private System.Collections.IEnumerator BlinkBattery()
    {
        isBlinking = true;
        while (fossilFuelLevel / fossilFuelMax <= 0.05f)
        {
            // Blink the battery sprite
            batterySpriteRenderer.sprite = batterySprites[4]; // Second to last sprite
            if (batteryLights.Length > 0)
            {
                batteryLights[0].enabled = true; // Turn on the last light
            }
            yield return new WaitForSeconds(0.5f);

            batterySpriteRenderer.sprite = batterySprites[5]; // Last sprite
            if (batteryLights.Length > 0)
            {
                batteryLights[0].enabled = false; // Turn off the last light
            }
            yield return new WaitForSeconds(0.5f);
        }
        isBlinking = false;
    }

    public void IncreaseBatteryCapacity(float increasePercent)
    {
        fossilFuelMax += fossilFuelMax * (increasePercent / 100f);
        fossilFuelLevel += fossilFuelMax * (increasePercent / 100f);
    }
}