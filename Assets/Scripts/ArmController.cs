using UnityEngine;

public class ArmController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform armPivot; // The pivot point of the arm
    [SerializeField] private Transform playerTransform; // Reference to the player's transform
    [SerializeField] private Camera mainCamera; // Reference to the main camera
    [SerializeField] private ParticleSystem drillEffect; // Particle effect for the drill
    
    [Header("Settings")]
    [SerializeField] private MouseButton freezeButton = MouseButton.Left; // Which mouse button freezes this arm
    [SerializeField] private float rotationSpeed = 10f; // How quickly the arm rotates to aim at cursor
    [SerializeField] private float longPressThreshold = 0.3f; // Time in seconds to consider a press as "long"
    
    // Current state
    private bool isFrozen = false;
    private Vector3 frozenDirection;
    private bool isHoldingMouse = false;
    private float mouseDownTime;
    private bool wasLongPress = false;
    
    // Define an enum for mouse buttons (makes it easy to configure in inspector)
    public enum MouseButton
    {
        Left = 0,
        Right = 1,
        Middle = 2
    }
    
    private void Start()
    {
        // If no camera is assigned, use the main camera
        if (mainCamera == null)
            mainCamera = Camera.main;
            
        // If no player reference is assigned, use parent transform
        if (playerTransform == null)
            playerTransform = transform.parent;
    }
    
    private void Update()
    {
        if (freezeButton == MouseButton.Left) {
            // Check for mouse button down
            if (Input.GetMouseButtonDown((int)freezeButton))
            {
                isHoldingMouse = true;
                mouseDownTime = Time.time;
                wasLongPress = false;
                
                // Don't immediately change frozen state
                // If we're going to freeze, capture the current direction
                if (!isFrozen)
                {
                    frozenDirection = GetAimDirection();
                }
            }
            
            // Check for mouse button up
            if (Input.GetMouseButtonUp((int)freezeButton))
            {
                // Determine if this was a long press
                wasLongPress = Time.time - mouseDownTime >= longPressThreshold;
                isHoldingMouse = false;
                
                if (wasLongPress)
                {
                    frozenDirection = GetAimDirection();
                }
                else
                {
                    // It was a quick tap, so toggle frozen state
                    isFrozen = !isFrozen;
                    
                    if (isFrozen)
                    {
                        // Update frozen direction when freezing
                        frozenDirection = GetAimDirection();
                    }
                    UpdateDrillEffect(); // Update particle effect state
                }
            }
            
            // If holding mouse past threshold, temporarily unfreeze to follow cursor
            if (isHoldingMouse && Time.time - mouseDownTime >= longPressThreshold)
            {
                wasLongPress = true;
                // Temporarily follow cursor while holding
                UpdateArmRotation(false);
            }
            else //if (!wasLongPress)
            {
                // Normal update based on frozen state
                UpdateArmRotation(isFrozen);
            }
        } else if (freezeButton == MouseButton.Right) {
            if (Input.GetMouseButtonDown((int)freezeButton))
            {
                isFrozen = !isFrozen; // Toggle frozen state

                if (isFrozen)
                {
                    // Store current direction when freezing
                    frozenDirection = GetAimDirection();
                }
                UpdateDrillEffect(); // Update particle effect state
            }

            // Update arm rotation
            UpdateArmRotation(isFrozen);
        }
    }
    
    private void UpdateArmRotation(bool useFrozenDirection)
    {
        if (useFrozenDirection)
        {
            // When frozen, maintain the saved direction
            float angle = Mathf.Atan2(frozenDirection.y, frozenDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            armPivot.rotation = targetRotation;
        }
        else
        {
            // When not frozen, aim toward cursor
            Vector3 aimDirection = GetAimDirection();
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            
            // Smoothly rotate to target
            armPivot.rotation = Quaternion.Lerp(armPivot.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    private Vector3 GetAimDirection()
    {
        // Get mouse position in world space
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f; // Ensure we're on the same z-plane
        
        // Calculate direction from arm pivot to mouse position
        Vector3 direction = mousePos - armPivot.position;
        return direction.normalized;
    }
    
    private void UpdateDrillEffect()
    {
        print("Updating drill effect: " + isFrozen);
        if (drillEffect != null)
        {
            if (isFrozen)
                drillEffect.Play(); // Enable particle effect
            else
                drillEffect.Stop(); // Disable particle effect
        }
    }

    // Public method to reset the arm to unfrozen state
    public void ResetArm()
    {
        isFrozen = false;
    }
}