using UnityEngine;

public class Consumable : MonoBehaviour
{
    public enum ConsumableType
    {
        FossilFuel,
        Speed,
        JumpDistance,
        BatteryCapacity,
        MiningSpeed
    }

    [SerializeField] private ConsumableType consumableType;
    [SerializeField] private float fossilFuelAmount = 75f;
    [SerializeField] private float speedIncrease = 10f;
    [SerializeField] private float jumpDistanceIncrease = 15f;
    [SerializeField] private float batteryCapacityIncreasePercent = 15f;
    [SerializeField] private float miningSpeedIncreasePercent = 15f;

    [SerializeField] private float hoverAmplitude = 1f; // How far the sprite moves up and down
    [SerializeField] private float hoverSpeed = 3f; // Speed of the hover animation

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Floating animation
        float hoverOffset = Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
        transform.position = startPosition + new Vector3(0, hoverOffset, 0);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ApplyEffect(collision.GetComponent<PlayerController>(), collision.GetComponent<FuelManager>(), collision.GetComponent<PlayerMining>());
            Destroy(gameObject);
        }
    }

    private void ApplyEffect(PlayerController playerController, FuelManager fuelManager, PlayerMining playerMining)
    {
        switch (consumableType)
        {
            case ConsumableType.FossilFuel:
                if (fuelManager != null)
                {
                    fuelManager.AddFuel(fossilFuelAmount);
                }
                break;

            case ConsumableType.Speed:
                if (playerController != null)
                {
                    playerController.IncreaseSpeed(speedIncrease);
                }
                break;

            case ConsumableType.JumpDistance:
                if (playerController != null)
                {
                    playerController.IncreaseJumpDistance(jumpDistanceIncrease);
                }
                break;

            case ConsumableType.BatteryCapacity:
                if (fuelManager != null)
                {
                    fuelManager.IncreaseBatteryCapacity(batteryCapacityIncreasePercent);
                }
                break;

            case ConsumableType.MiningSpeed:
                if (playerController != null)
                {
                    playerMining.IncreaseMiningSpeed(miningSpeedIncreasePercent);
                }
                break;
        }
    }
}