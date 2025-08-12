using UnityEngine;
using System.Collections;

[System.Serializable]
public class OrbitSettings
{
    [Header("Target Settings")]
    [Tooltip("The target object to orbit around")]
    public Transform targetObject;
    
    [Header("Orbit Settings")]
    [Tooltip("Distance from the target object")]
    public float orbitDistance = 5f;
    
    [Tooltip("Height offset from the target object")]
    public float heightOffset = 2f;
    
    [Tooltip("Rotation speed in degrees per second")]
    public float rotationSpeed = 30f;
    
    [Tooltip("Starting angle in degrees (0 = right, 90 = forward, 180 = left, 270 = back)")]
    public float startAngle = 0f;
    
    [Tooltip("Maximum angle to rotate to (set to 0 for continuous rotation)")]
    public float maxAngle = 360f;
    
    [Header("Camera Settings")]
    [Tooltip("Field of view for the camera")]
    public float fieldOfView = 60f;
    
    [Tooltip("Smoothness of camera movement (lower = smoother)")]
    [Range(0.1f, 10f)]
    public float smoothness = 2f;
}

public class CameraOrbitRecorder : MonoBehaviour
{
    [Header("Orbit Configuration")]
    public OrbitSettings orbitSettings = new OrbitSettings();
    
    [Header("Controls")]
    [Tooltip("Key to start/stop orbiting")]
    public KeyCode toggleOrbitKey = KeyCode.R;
    
    [Header("UI Display")]
    [Tooltip("Show orbit information on screen")]
    public bool showDebugInfo = true;
    
    // Private variables
    private Camera orbitCamera;
    private bool isOrbiting = false;
    private float currentAngle;
    private Vector3 targetPosition;
    private Vector3 currentVelocity;
    
    void Start()
    {
        // Get or create camera component
        orbitCamera = GetComponent<Camera>();
        if (orbitCamera == null)
        {
            orbitCamera = gameObject.AddComponent<Camera>();
        }
        
        // Set initial camera settings
        orbitCamera.fieldOfView = orbitSettings.fieldOfView;
        
        // Initialize angle
        currentAngle = orbitSettings.startAngle;
        
        // Set initial position
        UpdateCameraPosition();
    }
    
    void Update()
    {
        HandleInput();
        
        if (isOrbiting)
        {
            UpdateOrbit();
        }
    }
    
    void HandleInput()
    {
        // Toggle orbiting
        if (Input.GetKeyDown(toggleOrbitKey))
        {
            ToggleOrbit();
        }
    }
    
    void UpdateOrbit()
    {
        // Update angle
        currentAngle += orbitSettings.rotationSpeed * Time.deltaTime;
        
        // Check if we've reached the maximum angle
        if (orbitSettings.maxAngle > 0 && currentAngle >= orbitSettings.startAngle + orbitSettings.maxAngle)
        {
            StopOrbit();
            return;
        }
        
        UpdateCameraPosition();
    }
    
    void UpdateCameraPosition()
    {
        if (orbitSettings.targetObject == null)
        {
            Debug.LogWarning("No target object assigned for camera orbit!");
            return;
        }
        
        // Calculate target position
        float radians = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(
            Mathf.Cos(radians) * orbitSettings.orbitDistance,
            orbitSettings.heightOffset,
            Mathf.Sin(radians) * orbitSettings.orbitDistance
        );
        
        targetPosition = orbitSettings.targetObject.position + offset;
        
        // Smoothly move camera to target position
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            targetPosition, 
            ref currentVelocity, 
            1f / orbitSettings.smoothness
        );
        
        // Look at target
        transform.LookAt(orbitSettings.targetObject.position);
    }
    
    public void StartOrbit()
    {
        if (orbitSettings.targetObject == null)
        {
            Debug.LogError("Cannot start orbit: No target object assigned!");
            return;
        }
        
        isOrbiting = true;
        currentAngle = orbitSettings.startAngle;
        Debug.Log("Started orbiting around " + orbitSettings.targetObject.name);
    }
    
    public void StopOrbit()
    {
        isOrbiting = false;
        Debug.Log("Stopped orbiting");
    }
    
    public void ToggleOrbit()
    {
        if (isOrbiting)
        {
            StopOrbit();
        }
        else
        {
            StartOrbit();
        }
    }
    
    void OnGUI()
    {
        if (!showDebugInfo) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 150));
        GUILayout.Label("Camera Orbit Recorder", GUI.skin.box);
        
        GUILayout.Label($"Target: {(orbitSettings.targetObject ? orbitSettings.targetObject.name : "None")}");
        GUILayout.Label($"Orbiting: {isOrbiting}");
        GUILayout.Label($"Current Angle: {currentAngle:F1}°");
        GUILayout.Label($"Speed: {orbitSettings.rotationSpeed}°/s");
        
        GUILayout.Space(10);
        GUILayout.Label("Controls:");
        GUILayout.Label($"Toggle Orbit: {toggleOrbitKey}");
        
        GUILayout.EndArea();
    }
    
    // Public methods for external control
    public void SetTarget(Transform newTarget)
    {
        orbitSettings.targetObject = newTarget;
        if (isOrbiting)
        {
            UpdateCameraPosition();
        }
    }
    
    public void SetOrbitSpeed(float speed)
    {
        orbitSettings.rotationSpeed = speed;
    }
    
    public void SetOrbitDistance(float distance)
    {
        orbitSettings.orbitDistance = distance;
        if (isOrbiting)
        {
            UpdateCameraPosition();
        }
    }
    
    public void SetHeightOffset(float height)
    {
        orbitSettings.heightOffset = height;
        if (isOrbiting)
        {
            UpdateCameraPosition();
        }
    }
    
    // Runtime configuration methods
    public void ConfigureOrbit(float distance, float height, float speed, float startAngle = 0f, float maxAngle = 360f)
    {
        orbitSettings.orbitDistance = distance;
        orbitSettings.heightOffset = height;
        orbitSettings.rotationSpeed = speed;
        orbitSettings.startAngle = startAngle;
        orbitSettings.maxAngle = maxAngle;
        
        if (isOrbiting)
        {
            currentAngle = startAngle;
            UpdateCameraPosition();
        }
    }
    
    // Utility methods for finding targets
    public void FindAndSetTargetByName(string targetName)
    {
        GameObject target = GameObject.Find(targetName);
        if (target != null)
        {
            SetTarget(target.transform);
        }
        else
        {
            Debug.LogWarning($"Could not find target object with name: {targetName}");
        }
    }
    
    public void FindAndSetTargetByTag(string targetTag)
    {
        GameObject target = GameObject.FindWithTag(targetTag);
        if (target != null)
        {
            SetTarget(target.transform);
        }
        else
        {
            Debug.LogWarning($"Could not find target object with tag: {targetTag}");
        }
    }
    
    // Getters for external access
    public bool IsOrbiting => isOrbiting;
    public float CurrentAngle => currentAngle;
    public Transform Target => orbitSettings.targetObject;
    public float OrbitDistance => orbitSettings.orbitDistance;
    public float HeightOffset => orbitSettings.heightOffset;
    public float RotationSpeed => orbitSettings.rotationSpeed;
} 