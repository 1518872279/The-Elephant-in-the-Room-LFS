using UnityEngine;

public class TornadoTestController : MonoBehaviour
{
    [Header("Test Controls")]
    public KeyCode toggleStormKey = KeyCode.S;
    public KeyCode toggleTornadoKey = KeyCode.T;
    public KeyCode moveTornadoKey = KeyCode.M;
    
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float heightChangeSpeed = 2f;
    
    [Header("Debug Display")]
    public bool showDebugInfo = true;
    public Vector2 debugPosition = new Vector2(10, 10);
    
    private StormController stormController;
    private TornadoController tornadoController;
    private TornadoStormIntegration tornadoIntegration;
    private Camera playerCamera;
    
    void Start()
    {
        // Find components
        stormController = StormController.Instance;
        tornadoController = GetComponent<TornadoController>();
        tornadoIntegration = GetComponent<TornadoStormIntegration>();
        playerCamera = Camera.main;
        
        if (playerCamera == null)
            playerCamera = FindObjectOfType<Camera>();
    }
    
    void Update()
    {
        HandleInput();
        UpdateDebugInfo();
    }
    
    void HandleInput()
    {
        // Toggle storm
        if (Input.GetKeyDown(toggleStormKey))
        {
            if (stormController != null)
            {
                stormController.ToggleStorm();
                Debug.Log("Storm toggled!");
            }
        }
        
        // Toggle tornado
        if (Input.GetKeyDown(toggleTornadoKey))
        {
            if (tornadoIntegration != null)
            {
                tornadoIntegration.ToggleTornado();
            }
            else if (tornadoController != null)
            {
                tornadoController.enabled = !tornadoController.enabled;
                Debug.Log("Tornado toggled!");
            }
        }
        
        // Move tornado with mouse
        if (Input.GetKey(moveTornadoKey))
        {
            MoveTornadoWithMouse();
        }
        
        // Height adjustment
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position += Vector3.up * heightChangeSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.position += Vector3.down * heightChangeSpeed * Time.deltaTime;
        }
    }
    
    void MoveTornadoWithMouse()
    {
        if (playerCamera == null) return;
        
        // Get mouse position in world space
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = playerCamera.transform.position.y - transform.position.y;
        Vector3 worldPos = playerCamera.ScreenToWorldPoint(mousePos);
        
        // Move tornado to mouse position
        Vector3 targetPos = new Vector3(worldPos.x, transform.position.y, worldPos.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }
    
    void UpdateDebugInfo()
    {
        if (!showDebugInfo) return;
        
        string debugText = "=== TORNADO TEST CONTROLS ===\n";
        debugText += $"S - Toggle Storm\n";
        debugText += $"T - Toggle Tornado\n";
        debugText += $"M - Move Tornado (hold + mouse)\n";
        debugText += $"Q/E - Adjust Height\n\n";
        
        debugText += "=== STATUS ===\n";
        if (stormController != null)
            debugText += $"Storm Active: {stormController.IsStormActive}\n";
        if (tornadoIntegration != null)
            debugText += $"Tornado Active: {tornadoIntegration.IsTornadoActive}\n";
        if (tornadoController != null)
        {
            debugText += $"Tornado Enabled: {tornadoController.enabled}\n";
            debugText += $"Affected Objects: {tornadoController.GetAffectedObjectCount()}\n";
            debugText += $"Radius: {tornadoController.radius:F1}\n";
            debugText += $"Force: {tornadoController.maxForce:F1}\n";
        }
        if (tornadoIntegration != null)
            debugText += $"Intensity: {tornadoIntegration.GetTornadoIntensity():F2}\n";
        
        debugText += $"Position: {transform.position}\n";
        
        // Display debug info
        GUI.Label(new Rect(debugPosition.x, debugPosition.y, 300, 400), debugText);
    }
    
    void OnGUI()
    {
        if (showDebugInfo)
        {
            UpdateDebugInfo();
        }
    }
    
    // Public methods for external control
    public void SetTornadoPosition(Vector3 position)
    {
        transform.position = position;
    }
    
    public void SetTornadoRadius(float radius)
    {
        if (tornadoController != null)
            tornadoController.SetRadius(radius);
    }
    
    public void SetTornadoForce(float force)
    {
        if (tornadoController != null)
            tornadoController.SetForce(force);
    }
} 