using System;
using UnityEngine;
using System.Collections;

public class PlayerMining : MonoBehaviour
{
    [SerializeField] private float miningDistance = 5f;
    [SerializeField] private float miningWidth = 1f;  // Width of the mining area
    [SerializeField] private float timeToBreak = 0.5f;  // Time in seconds to break an object
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
                Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = transform.position.z;
                miningDirection = (mouseWorldPos - transform.position).normalized;
            }
        }

        if (isMining)
        {
            RaycastHit2D hit = Physics2D.BoxCast(
                transform.position,
                new Vector2(miningWidth, miningWidth),
                0f,
                miningDirection,
                miningDistance
            );

            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Breakable"))
                {
                    StartCoroutine(DestroyAfterDelay(hit.collider.gameObject));
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
    public void GatherFossilFuel(float fossilFuelAmount)
    {
        fossilFuelLevel = Math.Min(fossilFuelLevel + fossilFuelAmount, 100);
    }
}
