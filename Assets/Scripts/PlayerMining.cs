using UnityEngine;
using System.Collections;

public class PlayerMining : MonoBehaviour
{
    [SerializeField] private float miningDistance = 5f;
    [SerializeField] private float miningWidth = 1f;  // Width of the mining area
    [SerializeField] private float mineCooldown = 0.25f;
    private float lastMineTime = -999f;
    public float health = 10f; //Just basic health amount
    [SerializeField] private float timeToBreak = 0.1f;  // Time in seconds to break an object
    private bool isMining = false;
    private Vector3 miningDirection;
    private Camera mainCamera;
    private bool isHoldingMouse = false;
    private float mouseDownTime;
    [SerializeField] private float longPressThreshold = 0.3f; // Time in seconds to consider a press as "long"
    private bool wasLongPress = false;
    private AudioSource[] audioSources;

    // Serialized fields for the mining arm sprites
    [SerializeField] private Sprite spinningDrillSprite1;
    [SerializeField] private Sprite spinningDrillSprite2;
    [SerializeField] private Sprite nonSpinningDrillSprite;
    [SerializeField] private SpriteRenderer miningArmSpriteRenderer;

    // Timer for alternating drill sprites
    [SerializeField] private float drillSpriteSwitchInterval = 0.1f; // Time in seconds to switch sprites
    private float lastSpriteSwitchTime = 0f;
    private bool useFirstDrillSprite = true;

    void Start()
    {
        mainCamera = Camera.main;
        // TODO: move this to player controls later
        Physics2D.gravity = new Vector2(0, -25f); // higher grav accel to prevent "bouncy" look
        audioSources = GetComponents<AudioSource>();
    }

    void Update()
    {
        // Check for click vs. hold
        if (Input.GetMouseButtonDown(0))
        {
            isHoldingMouse = true;
            mouseDownTime = Time.time;
            wasLongPress = false;

            // Play drill windup noise
            audioSources[1].Play();
            
            // Don't immediately set isMining = true
            // Instead, capture the current direction if we're going to start mining
            if (!isMining)
            {
                UpdateMiningDirection();
            }
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            // Determine if this was a long press
            wasLongPress = Time.time - mouseDownTime >= longPressThreshold;
            isHoldingMouse = false;

            if (wasLongPress)
            {
                // Stop mining if this was a long press
                isMining = false;
            }
            else
            {
                // It was a quick tap, so toggle mining
                isMining = !isMining;
                
                // If we just started mining, update the direction
                if (isMining)
                {
                    UpdateMiningDirection();
                }
            }
        }
        
        // If holding the mouse button, continuously update the direction and ensure mining is active
        if (isHoldingMouse && Time.time - mouseDownTime >= longPressThreshold)
        {
            wasLongPress = true;
            isMining = true;
            UpdateMiningDirection();
        }

        // Update the mining arm sprite based on the mining state
        UpdateMiningArmSprite();

        if (isMining)
        {
            if(Time.time >= lastMineTime + mineCooldown)
            {
                lastMineTime = Time.time;

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
    }

    private void UpdateMiningDirection()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = transform.position.z;
        miningDirection = (mouseWorldPos - transform.position).normalized;
    }

    private void UpdateMiningArmSprite()
    {
        if (isMining)
        {
            // Alternate between the two spinning drill sprites
            if (Time.time >= lastSpriteSwitchTime + drillSpriteSwitchInterval)
            {
                lastSpriteSwitchTime = Time.time;
                useFirstDrillSprite = !useFirstDrillSprite;
            }

            miningArmSpriteRenderer.sprite = useFirstDrillSprite ? spinningDrillSprite1 : spinningDrillSprite2;
        }
        else
        {
            miningArmSpriteRenderer.sprite = nonSpinningDrillSprite;
        }
    }

    private IEnumerator DestroyAfterDelay(GameObject obj)
    {
        yield return new WaitForSeconds(timeToBreak);
        if (obj != null)
        {
            // Check if the object is a chunk that should subdivide
            Chunk chunk = obj.GetComponent<Chunk>();
            if (chunk != null)
            {
                chunk.Subdivide();
            }
            else
            {
                Destroy(obj);
                if(!audioSources[1].isPlaying)
                {
                    audioSources[1].Play();
                }
            }
        }
    }
    public void IncreaseMiningSpeed(float percent)
    {
        mineCooldown *= 1f / (1f + percent / 100f);
    }
}