using System;
using UnityEngine;
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

        // Update the battery sprite based on the fuel level
        UpdateBatterySprite();

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

    private void UpdateBatterySprite()
    {
        if (batterySprites == null || batterySprites.Length < 6 || batterySpriteRenderer == null)
            return;

        float fuelPercentage = fossilFuelLevel / fossilFuelMax;

        if (fuelPercentage > 0.8f)
        {
            batterySpriteRenderer.sprite = batterySprites[0]; // 100% - 81%
            isBlinking = false;
        }
        else if (fuelPercentage > 0.6f)
        {
            batterySpriteRenderer.sprite = batterySprites[1]; // 80% - 61%
            isBlinking = false;
        }
        else if (fuelPercentage > 0.4f)
        {
            batterySpriteRenderer.sprite = batterySprites[2]; // 60% - 41%
            isBlinking = false;
        }
        else if (fuelPercentage > 0.2f)
        {
            batterySpriteRenderer.sprite = batterySprites[3]; // 40% - 21%
            isBlinking = false;
        }
        else if (fuelPercentage > 0.05f)
        {
            batterySpriteRenderer.sprite = batterySprites[4]; // 20% - 6%
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

    private System.Collections.IEnumerator BlinkBattery()
    {
        isBlinking = true;
        while (fossilFuelLevel / 100f <= 0.05f)
        {
            batterySpriteRenderer.sprite = batterySprites[4]; // Second to last sprite
            yield return new WaitForSeconds(0.5f);
            batterySpriteRenderer.sprite = batterySprites[5]; // Last sprite
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