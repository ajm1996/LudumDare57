using System;
using UnityEngine;
using System.Collections;

public class PlayerMining : MonoBehaviour
{
    [SerializeField] private float miningDistance = 5f;
    [SerializeField] private float miningWidth = 1f;  // Width of the mining area
    public float health = 10f; //Just basic health amount
    public float fossilFuelLevel = 100f;
    [SerializeField] private float timeToBreak = 0.5f;  // Time in seconds to break an object
    private bool isMining = false;
    private Vector3 miningDirection;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        // TODO: move this to player controls later
        Physics2D.gravity = new Vector2(0, -25f); // higher grav accel to prevent "bouncy" look
    }

    void Update()
    {
        fossilFuelLevel -= 2 * Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            isMining = !isMining;
            if (isMining)
            {
                Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = transform.position.z;
                miningDirection = (mouseWorldPos - transform.position).normalized;
            }
        }

        if (isMining)
        {
            RaycastHit2D[] hits = Physics2D.BoxCastAll(
                transform.position,
                new Vector2(miningWidth, miningWidth),
                0f,
                miningDirection,
                miningDistance
            );

            int processedTargets = 0;
            foreach (RaycastHit2D hit in hits)
            {
                if (processedTargets >= 10) break;
                
                if (hit.collider != null && hit.collider.CompareTag("Breakable"))
                {
                    StartCoroutine(DestroyAfterDelay(hit.collider.gameObject));
                    processedTargets++;
                }
            }
        }
    }

    private IEnumerator DestroyAfterDelay(GameObject obj)
    {
        yield return new WaitForSeconds(timeToBreak);
        if (obj != null)  // Check if object still exists
        {
            Destroy(obj);
        }
    }

    public void GatherFossilFuel(float fossilFuelAmount)
    {
        fossilFuelLevel = Math.Min(fossilFuelLevel + fossilFuelAmount, 100);
    }
}
