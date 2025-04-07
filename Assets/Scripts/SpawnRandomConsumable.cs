using UnityEngine;

public class SpawnRandomConsumable : MonoBehaviour
{
    public enum ConsumableType
    {
        Speed,
        JumpDistance,
        BatteryCapacity,
        MiningSpeed
    }

    [Header("Consumable Prefabs")]
    [SerializeField] private GameObject speedPrefab;
    [SerializeField] private GameObject jumpDistancePrefab;
    [SerializeField] private GameObject batteryCapacityPrefab;
    [SerializeField] private GameObject miningSpeedPrefab;

    void Start()
    {
        // Get a random consumable type
        ConsumableType randomType = (ConsumableType)Random.Range(0, System.Enum.GetValues(typeof(ConsumableType)).Length);

        // Spawn the corresponding prefab
        GameObject prefabToSpawn = null;

        switch (randomType)
        {
            case ConsumableType.Speed:
                prefabToSpawn = speedPrefab;
                break;
            case ConsumableType.JumpDistance:
                prefabToSpawn = jumpDistancePrefab;
                break;
            case ConsumableType.BatteryCapacity:
                prefabToSpawn = batteryCapacityPrefab;
                break;
            case ConsumableType.MiningSpeed:
                prefabToSpawn = miningSpeedPrefab;
                break;
        }

        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
        }
    }

}