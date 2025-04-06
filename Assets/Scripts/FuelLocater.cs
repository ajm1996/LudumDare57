using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ArmFuelDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("The component type that identifies fuel objects")]
    public string fuelComponentName = "FuelComponent";
    
    [Tooltip("Which layers to check for fuel objects")]
    public LayerMask fuelLayerMask = -1;
    
    [Tooltip("Maximum distance to detect fuel")]
    public float detectionRange = 5f;
    
    [Tooltip("How quickly the arm rotates to target")]
    public float rotationSpeed = 5f;
    
    [Header("Default Position")]
    [Tooltip("Default rotation when no fuel is detected (pointing down)")]
    public Vector3 defaultRotation = new Vector3(0, 0, -90f);

    [Header("Light Settings")]
    [Tooltip("Reference to the Light2D component")]
    public Light2D armLight;
    
    [Tooltip("Maximum light intensity when closest to fuel")]
    public float maxLightIntensity = 1.0f;
    
    [Tooltip("Maximum inner radius when closest to fuel")]
    public float maxInnerRadius = 0.5f;

    // Reference to transform of the arm's pivot (shoulder)
    [SerializeField] private Transform armPivot; // The pivot point of the arm
    
    // Store nearest detected fuel
    private Transform nearestFuel;

    // Store the distance to nearest fuel
    private float distanceToFuel;
    
    void Start()
    {
        // Get the parent transform (shoulder pivot)
        armPivot = transform.parent;
        
        // Set initial rotation to default (down)
        if (armPivot != null)
        {
            armPivot.localEulerAngles = defaultRotation;
        }
        else
        {
            Debug.LogError("Arm pivot parent not found. Make sure this script is attached to the arm with a parent transform.");
        }

        // If light reference is not set in inspector, try to get it from this gameObject
        if (armLight == null)
        {
            armLight = GetComponent<Light2D>();
            
            if (armLight == null)
            {
                Debug.LogWarning("Light2D component not found. Light effects will be disabled.");
            }
        }
        
        // Initialize light to off state
        UpdateLightProperties(0);
    }
    
    void Update()
    {
        // Find nearest fuel chunk
        FindNearestFuel();
        
        // Rotate arm based on detection
        RotateArm();

        // Update light properties
        UpdateLight();
    }
    
    void FindNearestFuel()
    {
        // Reset nearest fuel reference
        nearestFuel = null;
        
        // Use Physics2D to find all colliders within range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRange, fuelLayerMask);
        
        float closestDistance = detectionRange;
        
        // Find the closest fuel object within range
        foreach (Collider2D collider in colliders)
        {
            // Check if this object has the component we're looking for
            if (HasFuelComponent(collider.gameObject))
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestFuel = collider.transform;
                }
            }
        }
    }
    
    bool HasFuelComponent(GameObject obj)
    {
        // Method 1: Check for a specific component by name
        Component fuelComponent = obj.GetComponent(fuelComponentName);
        if (fuelComponent != null)
            return true;
            
        // Method 2: Check if the object is an instance of your fuel prefab
        // You can also check other properties that identify your fuel objects
        // For example: return obj.name.Contains("FuelChunk");
        
        return false;
    }
    
    void RotateArm()
    {
        if (armPivot == null) return;
        
        if (nearestFuel != null)
        {
            // Calculate direction to fuel
            Vector2 direction = nearestFuel.position - armPivot.position;
            
            // Calculate angle
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            // Create target rotation
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            
            // Smoothly rotate towards target
            armPivot.rotation = Quaternion.Slerp(armPivot.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            // No fuel detected, rotate to default position (down)
            Quaternion defaultRot = Quaternion.Euler(defaultRotation);
            armPivot.localRotation = Quaternion.Slerp(armPivot.localRotation, defaultRot, rotationSpeed * Time.deltaTime);
        }
    }

    void UpdateLight()
    {
        if (armLight == null) return;
        
        float intensityFactor = 0f;
        
        if (nearestFuel != null)
        {
            // Calculate how close we are to fuel as a percentage (1.0 = closest, 0.0 = farthest/none)
            intensityFactor = 1.0f - (distanceToFuel / detectionRange);
            
            // Optional: Square the factor to make it more exponential (light gets brighter faster as you get closer)
            intensityFactor = intensityFactor * intensityFactor;
        }
        
        // Update light properties
        UpdateLightProperties(intensityFactor);
    }
    
    void UpdateLightProperties(float factor)
    {
        if (armLight == null) return;
        
        // Update intensity - scales from 0 to maxLightIntensity
        armLight.intensity = factor * maxLightIntensity;
        
        // Update inner radius - scales from 0 to maxInnerRadius
        armLight.pointLightInnerRadius = factor * maxInnerRadius;
    }
    
    // Optional: Visualize the detection range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}