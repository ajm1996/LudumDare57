using System;
using UnityEngine;

public class FuelManager : MonoBehaviour
{
    public static FuelManager Instance;  // Singleton instance

    [SerializeField] public float fossilFuelLevel = 100f;
    [SerializeField] public float passiveFuelRate = 0.3f;

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
        fossilFuelLevel -= passiveFuelRate * Time.deltaTime;
    }

    public void AddFuel(float fuelAmount)
    {
        // Update fossilFuelLevel correctly
        fossilFuelLevel = Math.Min(fossilFuelLevel + fuelAmount, 100);
    }

    public void RemoveFuel(float fuelAmount)
    {
        fossilFuelLevel = Math.Max(fossilFuelLevel - fuelAmount, 0);
    }
}