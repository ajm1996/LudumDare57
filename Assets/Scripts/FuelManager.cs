using System;
using UnityEngine;

public class FuelManager : MonoBehaviour
{

    [SerializeField] public float fossilFuelLevel = 100f;
    [SerializeField] public float passiveFuelRate = 0.3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fossilFuelLevel -= passiveFuelRate * Time.deltaTime;
    }

    public void AddFuel(float fuelAmount)
    {
        fuelAmount = Math.Min(fossilFuelLevel + fuelAmount, 100);
    }

    public void RemoveFuel(float fuelAmount)
    {
        fossilFuelLevel = Math.Max(fossilFuelLevel - fuelAmount, 0);
    }
}
