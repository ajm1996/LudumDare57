using System;
using UnityEngine;

public class PlayerMining : MonoBehaviour
{
    [SerializeField] private float miningDistance = 5f;
    [SerializeField] private float miningWidth = 1f;  // Width of the mining area
    public float health = 10f; //Just basic health amount
    public float fossilFuelLevel = 100f;
    private bool isMining = false;
    private Vector3 miningDirection;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        fossilFuelLevel -= 2 * Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            isMining = !isMining;
            if (isMining)
            {
                print("Mining started");
                // Convert mouse position to world point in 2D
                Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = transform.position.z; // Keep same Z as player
                miningDirection = (mouseWorldPos - transform.position).normalized;
                print(miningDirection);
            }
        }

        if (isMining)
        {
            RaycastHit2D hit = Physics2D.BoxCast(
                transform.position,           // origin
                new Vector2(miningWidth, miningWidth), // size of the box
                0f,                          // angle of the box (0 = no rotation)
                miningDirection,             // direction of the cast
                miningDistance              // distance to check
            );

            if (hit.collider != null)
            {
                print("Mining in direction: " + miningDirection);
                if (hit.collider.CompareTag("Breakable"))
                {
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }

    public void GatherFossilFuel(float fossilFuelAmount)
    {
        fossilFuelLevel = Math.Min(fossilFuelLevel + fossilFuelAmount, 100);
    }
}
