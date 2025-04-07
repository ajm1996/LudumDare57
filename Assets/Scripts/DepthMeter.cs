using UnityEngine;
using TMPro;

public class DepthMeter : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float startingYPosition = 0f;
    [SerializeField] private bool invertDepth = true; // Set to true if going down increases depth
    [SerializeField] private string depthUnit = "m";
    [SerializeField] private int decimalPlaces = 0;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Vector2 backgroundSize = new Vector2(1f, 0.4f);
    [SerializeField] private string sortingLayerName = "UI"; // Sorting layer for visibility
    [SerializeField] private int orderInLayer = 10; // Order within the layer
    [SerializeField] private float textAreaWidth = 1f;
    [SerializeField] private float textAreaHeight = 0.35f;
    [SerializeField] private float fontSize = 5f;
    [SerializeField] private float textPadding = 0.05f; // Padding around text

    
    public float depth; // Padding around text
    
    private TextMeshPro depthText;
    private GameObject meterBackground;
    private GameObject textObj;
    private float initialY;
    private Camera mainCamera;

    private void Awake()
    {
        if (playerTransform == null)
        {
            playerTransform = transform.parent;
        }
        
        initialY = startingYPosition;
        mainCamera = Camera.main;
        
        // Create the meter display
        CreateDepthMeter();
    }

    private void CreateDepthMeter()
    {
        // Create the background quad
        meterBackground = GameObject.CreatePrimitive(PrimitiveType.Quad);
        meterBackground.name = "DepthMeterBackground";
        meterBackground.transform.SetParent(transform, false);
        
        // Preserve the parent's Z position
        Vector3 localPos = Vector3.zero;
        meterBackground.transform.localPosition = localPos;
        meterBackground.transform.localScale = new Vector3(backgroundSize.x, backgroundSize.y, 1f);
        
        // Set the background material to black
        Renderer bgRenderer = meterBackground.GetComponent<Renderer>();
        bgRenderer.material = new Material(Shader.Find("Standard"));
        bgRenderer.material.color = Color.black;
        
        // Set sorting layer and order for the background
        bgRenderer.sortingLayerName = sortingLayerName;
        bgRenderer.sortingOrder = orderInLayer;
        
        // Create the text
        textObj = new GameObject("DepthText");
        textObj.transform.SetParent(meterBackground.transform, false);
        
        // Place text slightly in front of background but maintain Z hierarchy
        textObj.transform.localPosition = new Vector3(0f, 0f, -0.01f);
        textObj.transform.localRotation = Quaternion.identity;
        
        // Scale the text object based on the text area dimensions
        // This scales relative to the background's local space
        textObj.transform.localScale = new Vector3(
            textAreaWidth / backgroundSize.x, 
            textAreaHeight / backgroundSize.y, 
            1f
        );
        
        // Add TextMeshPro component
        depthText = textObj.AddComponent<TextMeshPro>();
        depthText.alignment = TextAlignmentOptions.Center;
        depthText.fontSize = fontSize;
        depthText.color = textColor;
        depthText.text = "0" + depthUnit;
        
        // Set text wrapping and dimensions
        depthText.enableWordWrapping = false;
        depthText.overflowMode = TextOverflowModes.Overflow;
        
        // Set the rect transform to fill its parent's space
        depthText.rectTransform.sizeDelta = new Vector2(1, 1);
        
        // Adjust vertical alignment
        depthText.verticalAlignment = VerticalAlignmentOptions.Middle;
        
        // Set sorting order for TextMeshPro
        depthText.renderer.sortingLayerName = sortingLayerName;
        depthText.renderer.sortingOrder = orderInLayer + 1; // One higher than background
        
        // Make text face camera
        textObj.AddComponent<FaceCamera>();
        
        // Remove collider from background
        Collider bgCollider = meterBackground.GetComponent<Collider>();
        if (bgCollider != null)
        {
            Destroy(bgCollider);
        }
    }

    private void Update()
    {
        UpdateDepthMeter();
        
        // Dynamically update the text scale if parameters change at runtime
        UpdateTextScale();
    }
    
    private void UpdateTextScale()
    {
        if (textObj != null && meterBackground != null)
        {
            textObj.transform.localScale = new Vector3(
                textAreaWidth / backgroundSize.x, 
                textAreaHeight / backgroundSize.y, 
                1f
            );
        }
    }

    private void UpdateDepthMeter()
    {
        if (playerTransform == null || depthText == null)
            return;

        // Calculate depth based on initial Y position
        float currentY = playerTransform.position.y;
        depth = initialY - currentY + 17f; //Hardcoded +17 because that's how the player starts relative to meter (makes meter read 0m at start)
        
        // Invert if needed (when going down increases depth)
        if (invertDepth)
        {
            depth = Mathf.Abs(depth);
        }

        // Format the depth text with the appropriate number of decimal places
        string formattedDepth = depth.ToString($"F{decimalPlaces}");
        depthText.text = $"{formattedDepth}{depthUnit}";
    }
}

// Helper script to make text face camera
public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
    }
    
    void Update()
    {
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                            mainCamera.transform.rotation * Vector3.up);
        }
    }
}