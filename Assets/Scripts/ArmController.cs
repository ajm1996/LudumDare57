using UnityEngine;

public class ArmController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform armPivot; // The pivot point of the arm
    [SerializeField] private Transform playerTransform; // Reference to the player's transform
    [SerializeField] private Camera mainCamera; // Reference to the main camera
    
    [Header("Settings")]
    [SerializeField] private MouseButton freezeButton = MouseButton.Left; // Which mouse button freezes this arm
    [SerializeField] private float rotationSpeed = 10f; // How quickly the arm rotates to aim at cursor
    
    // Current state
    private bool isFrozen = false;
    private Vector3 frozenDirection;
    
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
        // Check for freeze/unfreeze input
        if (Input.GetMouseButtonDown((int)freezeButton))
        {
            isFrozen = !isFrozen; // Toggle frozen state
            
            if (isFrozen)
            {
                // Store current direction when freezing
                frozenDirection = GetAimDirection();
            }
        }
        
        // Update arm rotation
        UpdateArmRotation();
    }
    
    private void UpdateArmRotation()
    {
        if (isFrozen)
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
    
    // Public method to reset the arm to unfrozen state
    public void ResetArm()
    {
        isFrozen = false;
    }
}