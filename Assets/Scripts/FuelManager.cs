using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FuelManager : MonoBehaviour
{
    public static FuelManager Instance;  // Singleton instance

    [SerializeField] public float fossilFuelLevel = 100f;
    [SerializeField] public float passiveFuelRate = 0.3f;
    [SerializeField] public float maxFuelRate = 1f;
    [SerializeField] public float minFuelRate = 0.1f;

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
        if (fossilFuelLevel <= 0)
        {
            // Game over, restart the game
            SceneManager.LoadScene(0);
        }
    }

    public void AddFuel(float fuelAmount)
    {
        fossilFuelLevel = Math.Min(fossilFuelLevel + fuelAmount, 100);
    }

    public void RemoveFuel(float fuelAmount)
    {
        fossilFuelLevel = Math.Max(fossilFuelLevel - fuelAmount, 0);
    }
}