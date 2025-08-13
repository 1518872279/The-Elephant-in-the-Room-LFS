using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DollyTrackSettings
{
    [Header("Track Settings")]
    [Tooltip("List of waypoints that define the dolly track path")]
    public List<Transform> waypoints = new List<Transform>();
    
    [Tooltip("Distance from the track (for offset)")]
    public float trackOffset = 0f;
    
    [Tooltip("Height offset from the track")]
    public float heightOffset = 0f;
    
    [Header("Auto-Discovery Settings")]
    [Tooltip("Automatically find waypoints from dolly track objects in the scene")]
    public bool autoDiscoverWaypoints = true;
    
    [Tooltip("Search radius for finding dolly track objects")]
    public float searchRadius = 100f;
    
    [Tooltip("Tags to search for dolly track objects")]
    public string[] dollyTrackTags = { "DollyTrack", "Waypoint", "CameraTrack" };
    
    [Tooltip("Names to search for dolly track objects")]
    public string[] dollyTrackNames = { "Dolly Track", "Camera Track", "Waypoints", "Track" };
    
    [Header("Movement Settings")]
    [Tooltip("Movement speed along the track")]
    public float movementSpeed = 5f;
    
    [Tooltip("Starting position along the track (0-1)")]
    [Range(0f, 1f)]
    public float startPosition = 0f;
    
    [Tooltip("Whether to loop the track")]
    public bool loopTrack = true;
    
    [Tooltip("Whether to reverse direction when reaching the end")]
    public bool reverseAtEnd = false;
    
    [Header("Camera Settings")]
    [Tooltip("Field of view for the camera")]
    public float fieldOfView = 60f;
    
    [Tooltip("Smoothness of camera movement (lower = smoother)")]
    [Range(0.1f, 10f)]
    public float smoothness = 2f;
    
    [Tooltip("Look ahead distance for camera orientation")]
    public float lookAheadDistance = 2f;
    
    [Header("Easing Settings")]
    [Tooltip("Easing type for movement")]
    public EasingType easingType = EasingType.Linear;
    
    [Tooltip("Easing strength (for non-linear easing)")]
    [Range(0.1f, 5f)]
    public float easingStrength = 1f;
}

public enum EasingType
{
    Linear,
    EaseIn,
    EaseOut,
    EaseInOut,
    SmoothStep
}

public class CameraDollyTrack : MonoBehaviour
{
    [Header("Dolly Track Configuration")]
    public DollyTrackSettings dollySettings = new DollyTrackSettings();
    
    [Header("Controls")]
    [Tooltip("Key to start/stop movement")]
    public KeyCode toggleMovementKey = KeyCode.R;
    
    [Tooltip("Key to reset to start position")]
    public KeyCode resetKey = KeyCode.T;
    
    [Tooltip("Key to refresh waypoints from scene")]
    public KeyCode refreshWaypointsKey = KeyCode.Y;
    
    [Header("UI Display")]
    [Tooltip("Show dolly information on screen")]
    public bool showDebugInfo = true;
    
    // Private variables
    private Camera dollyCamera;
    private bool isMoving = false;
    private float currentPosition;
    private Vector3 targetPosition;
    private Vector3 currentVelocity;
    private int currentDirection = 1; // 1 for forward, -1 for backward
    private int currentSegment = 0;
    private float segmentProgress = 0f;
    
    void Start()
    {
        // Get or create camera component
        dollyCamera = GetComponent<Camera>();
        if (dollyCamera == null)
        {
            dollyCamera = gameObject.AddComponent<Camera>();
        }
        
        // Set initial camera settings
        dollyCamera.fieldOfView = dollySettings.fieldOfView;
        
        // Auto-discover waypoints if enabled
        if (dollySettings.autoDiscoverWaypoints)
        {
            DiscoverWaypointsFromScene();
        }
        
        // Initialize position
        currentPosition = dollySettings.startPosition;
        
        // Set initial position
        UpdateCameraPosition();
    }
    
    void Update()
    {
        HandleInput();
        
        if (isMoving)
        {
            UpdateMovement();
        }
    }
    
    void HandleInput()
    {
        // Toggle movement
        if (Input.GetKeyDown(toggleMovementKey))
        {
            ToggleMovement();
        }
        
        // Reset position
        if (Input.GetKeyDown(resetKey))
        {
            ResetToStart();
        }
        
        // Refresh waypoints
        if (Input.GetKeyDown(refreshWaypointsKey))
        {
            RefreshWaypoints();
        }
    }
    
    void UpdateMovement()
    {
        if (dollySettings.waypoints.Count < 2)
        {
            Debug.LogWarning("Need at least 2 waypoints for dolly movement!");
            return;
        }
        
        // Calculate movement based on speed and time
        float movement = dollySettings.movementSpeed * Time.deltaTime;
        
        // Update position
        if (currentDirection > 0)
        {
            currentPosition += movement;
        }
        else
        {
            currentPosition -= movement;
        }
        
        // Handle track boundaries
        if (currentPosition >= 1f)
        {
            if (dollySettings.loopTrack)
            {
                currentPosition = 0f;
            }
            else if (dollySettings.reverseAtEnd)
            {
                currentPosition = 1f;
                currentDirection = -1;
            }
            else
            {
                currentPosition = 1f;
                StopMovement();
                return;
            }
        }
        else if (currentPosition <= 0f)
        {
            if (dollySettings.loopTrack)
            {
                currentPosition = 1f;
            }
            else if (dollySettings.reverseAtEnd)
            {
                currentPosition = 0f;
                currentDirection = 1;
            }
            else
            {
                currentPosition = 0f;
                StopMovement();
                return;
            }
        }
        
        UpdateCameraPosition();
    }
    
    void UpdateCameraPosition()
    {
        if (dollySettings.waypoints.Count < 2)
        {
            Debug.LogWarning("Need at least 2 waypoints for dolly movement!");
            return;
        }
        
        // Calculate position along the track
        Vector3 trackPosition = CalculatePositionOnTrack(currentPosition);
        
        // Apply offsets
        targetPosition = trackPosition + Vector3.up * dollySettings.heightOffset;
        
        // Smoothly move camera to target position
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            targetPosition, 
            ref currentVelocity, 
            1f / dollySettings.smoothness
        );
        
        // Calculate look direction
        Vector3 lookDirection = CalculateLookDirection(currentPosition);
        if (lookDirection != Vector3.zero)
        {
            transform.LookAt(transform.position + lookDirection);
        }
    }
    
    Vector3 CalculatePositionOnTrack(float t)
    {
        if (dollySettings.waypoints.Count == 0)
            return Vector3.zero;
        
        if (dollySettings.waypoints.Count == 1)
            return dollySettings.waypoints[0].position;
        
        // Apply easing
        float easedT = ApplyEasing(t);
        
        // Calculate total track length
        float totalLength = CalculateTotalTrackLength();
        float targetDistance = easedT * totalLength;
        
        // Find the segment and position within that segment
        float currentDistance = 0f;
        for (int i = 0; i < dollySettings.waypoints.Count - 1; i++)
        {
            float segmentLength = Vector3.Distance(
                dollySettings.waypoints[i].position, 
                dollySettings.waypoints[i + 1].position
            );
            
            if (currentDistance + segmentLength >= targetDistance)
            {
                // We're in this segment
                float segmentT = (targetDistance - currentDistance) / segmentLength;
                Vector3 startPos = dollySettings.waypoints[i].position;
                Vector3 endPos = dollySettings.waypoints[i + 1].position;
                
                // Apply track offset perpendicular to movement direction
                Vector3 direction = (endPos - startPos).normalized;
                Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;
                
                return Vector3.Lerp(startPos, endPos, segmentT) + perpendicular * dollySettings.trackOffset;
            }
            
            currentDistance += segmentLength;
        }
        
        // If we reach here, we're at the end
        return dollySettings.waypoints[dollySettings.waypoints.Count - 1].position;
    }
    
    Vector3 CalculateLookDirection(float t)
    {
        if (dollySettings.waypoints.Count < 2)
            return Vector3.forward;
        
        // Calculate look ahead position
        float lookAheadT = t + (dollySettings.lookAheadDistance / CalculateTotalTrackLength());
        if (lookAheadT > 1f)
        {
            if (dollySettings.loopTrack)
                lookAheadT -= 1f;
            else
                lookAheadT = 1f;
        }
        
        Vector3 currentPos = CalculatePositionOnTrack(t);
        Vector3 lookAheadPos = CalculatePositionOnTrack(lookAheadT);
        
        return (lookAheadPos - currentPos).normalized;
    }
    
    float CalculateTotalTrackLength()
    {
        float totalLength = 0f;
        for (int i = 0; i < dollySettings.waypoints.Count - 1; i++)
        {
            totalLength += Vector3.Distance(
                dollySettings.waypoints[i].position, 
                dollySettings.waypoints[i + 1].position
            );
        }
        return totalLength;
    }
    
    float ApplyEasing(float t)
    {
        switch (dollySettings.easingType)
        {
            case EasingType.Linear:
                return t;
                
            case EasingType.EaseIn:
                return Mathf.Pow(t, dollySettings.easingStrength);
                
            case EasingType.EaseOut:
                return 1f - Mathf.Pow(1f - t, dollySettings.easingStrength);
                
            case EasingType.EaseInOut:
                if (t < 0.5f)
                    return 0.5f * Mathf.Pow(2f * t, dollySettings.easingStrength);
                else
                    return 1f - 0.5f * Mathf.Pow(2f * (1f - t), dollySettings.easingStrength);
                
            case EasingType.SmoothStep:
                return t * t * (3f - 2f * t);
                
            default:
                return t;
        }
    }
    
    public void StartMovement()
    {
        if (dollySettings.waypoints.Count < 2)
        {
            Debug.LogError("Cannot start movement: Need at least 2 waypoints!");
            return;
        }
        
        isMoving = true;
        Debug.Log("Started dolly movement");
    }
    
    public void StopMovement()
    {
        isMoving = false;
        Debug.Log("Stopped dolly movement");
    }
    
    public void ToggleMovement()
    {
        if (isMoving)
        {
            StopMovement();
        }
        else
        {
            StartMovement();
        }
    }
    
    public void ResetToStart()
    {
        currentPosition = dollySettings.startPosition;
        currentDirection = 1;
        UpdateCameraPosition();
        Debug.Log("Reset to start position");
    }
    
    public void SetPosition(float position)
    {
        currentPosition = Mathf.Clamp01(position);
        UpdateCameraPosition();
    }
    
    public void SetSpeed(float speed)
    {
        dollySettings.movementSpeed = speed;
    }
    
    public void ReverseDirection()
    {
        currentDirection *= -1;
    }
    
    void OnGUI()
    {
        if (!showDebugInfo) return;
        
        try
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 250));
            
            GUILayout.BeginVertical("box");
            GUILayout.Label("Camera Dolly Track", GUI.skin.box);
            
            GUILayout.Label($"Waypoints: {dollySettings.waypoints.Count}");
            GUILayout.Label($"Auto-Discovery: {(dollySettings.autoDiscoverWaypoints ? "Enabled" : "Disabled")}");
            GUILayout.Label($"Moving: {isMoving}");
            GUILayout.Label($"Position: {currentPosition:F3}");
            GUILayout.Label($"Speed: {dollySettings.movementSpeed}");
            GUILayout.Label($"Direction: {(currentDirection > 0 ? "Forward" : "Backward")}");
            
            GUILayout.Space(10);
            GUILayout.Label("Controls:");
            GUILayout.Label($"Toggle Movement: {toggleMovementKey}");
            GUILayout.Label($"Reset Position: {resetKey}");
            GUILayout.Label($"Refresh Waypoints: {refreshWaypointsKey}");
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        catch (System.Exception e)
        {
            // Silently handle GUI errors to prevent console spam
            Debug.LogWarning($"GUI Error in CameraDollyTrack: {e.Message}");
        }
    }
    
    // Getters for external access
    public bool IsMoving => isMoving;
    public float CurrentPosition => currentPosition;
    public float MovementSpeed => dollySettings.movementSpeed;
    public List<Transform> Waypoints => dollySettings.waypoints;
    
    // Auto-discovery methods
    void DiscoverWaypointsFromScene()
    {
        try
        {
            // Clear existing waypoints if auto-discovery is enabled
            if (dollySettings.autoDiscoverWaypoints)
            {
                dollySettings.waypoints.Clear();
            }
            
            // Try to find waypoints from various sources
            DiscoverFromCinemachineDollyTracks();
            DiscoverFromWaypointObjects();
            DiscoverFromTaggedObjects();
            DiscoverFromNamedObjects();
            
            // Sort waypoints by distance from camera if we found any
            if (dollySettings.waypoints.Count > 0)
            {
                SortWaypointsByDistance();
                Debug.Log($"Auto-discovered {dollySettings.waypoints.Count} waypoints from scene objects");
            }
            else
            {
                Debug.LogWarning("No waypoints found in scene. Please add waypoints manually or check auto-discovery settings.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error during waypoint discovery: {e.Message}");
        }
    }
    
    void DiscoverFromCinemachineDollyTracks()
    {
        try
        {
            // Search for Cinemachine dolly track components
            var dollyTracks = FindObjectsOfType<MonoBehaviour>();
            
            foreach (var track in dollyTracks)
            {
                if (track == null) continue;
                
                // Check if it's a Cinemachine dolly track
                if (track.GetType().Name.Contains("CinemachineDollyTrack") || 
                    track.GetType().Name.Contains("CinemachinePath") ||
                    track.GetType().Name.Contains("CinemachineSmoothPath"))
                {
                    // Try to get waypoints from the track using reflection
                    var waypointsProperty = track.GetType().GetProperty("Waypoints");
                    if (waypointsProperty != null)
                    {
                        var waypoints = waypointsProperty.GetValue(track) as Transform[];
                        if (waypoints != null)
                        {
                            foreach (var waypoint in waypoints)
                            {
                                if (waypoint != null && !dollySettings.waypoints.Contains(waypoint))
                                {
                                    dollySettings.waypoints.Add(waypoint);
                                }
                            }
                        }
                    }
                    
                    // Try alternative property names
                    var m_WaypointsProperty = track.GetType().GetField("m_Waypoints", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (m_WaypointsProperty != null)
                    {
                        var waypoints = m_WaypointsProperty.GetValue(track) as Transform[];
                        if (waypoints != null)
                        {
                            foreach (var waypoint in waypoints)
                            {
                                if (waypoint != null && !dollySettings.waypoints.Contains(waypoint))
                                {
                                    dollySettings.waypoints.Add(waypoint);
                                }
                            }
                        }
                    }
                    
                    // Try to get waypoints from children if the track object has them
                    if (track.transform != null && track.transform.childCount > 0)
                    {
                        for (int i = 0; i < track.transform.childCount; i++)
                        {
                            var child = track.transform.GetChild(i);
                            if (child != null && (child.name.ToLower().Contains("waypoint") || 
                                child.name.ToLower().Contains("point") ||
                                child.name.ToLower().Contains("node")))
                            {
                                if (!dollySettings.waypoints.Contains(child))
                                {
                                    dollySettings.waypoints.Add(child);
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Error discovering Cinemachine dolly tracks: {e.Message}");
        }
    }
    
    void DiscoverFromWaypointObjects()
    {
        try
        {
            // Search for objects with "Waypoint" in their name
            var waypointObjects = GameObject.FindObjectsOfType<Transform>();
            
            foreach (var obj in waypointObjects)
            {
                if (obj != null && obj.name.ToLower().Contains("waypoint") && 
                    Vector3.Distance(transform.position, obj.position) <= dollySettings.searchRadius)
                {
                    if (!dollySettings.waypoints.Contains(obj))
                    {
                        dollySettings.waypoints.Add(obj);
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Error discovering waypoint objects: {e.Message}");
        }
    }
    
    void DiscoverFromTaggedObjects()
    {
        try
        {
            // Search for objects with specific tags
            foreach (string tag in dollySettings.dollyTrackTags)
            {
                if (string.IsNullOrEmpty(tag)) continue;
                
                var taggedObjects = GameObject.FindGameObjectsWithTag(tag);
                foreach (var obj in taggedObjects)
                {
                    if (obj != null && obj.transform != null && 
                        Vector3.Distance(transform.position, obj.transform.position) <= dollySettings.searchRadius)
                    {
                        // If the object has children, add them as waypoints
                        if (obj.transform.childCount > 0)
                        {
                            for (int i = 0; i < obj.transform.childCount; i++)
                            {
                                var child = obj.transform.GetChild(i);
                                if (child != null && !dollySettings.waypoints.Contains(child))
                                {
                                    dollySettings.waypoints.Add(child);
                                }
                            }
                        }
                        else
                        {
                            // Add the object itself as a waypoint
                            if (!dollySettings.waypoints.Contains(obj.transform))
                            {
                                dollySettings.waypoints.Add(obj.transform);
                            }
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Error discovering tagged objects: {e.Message}");
        }
    }
    
    void DiscoverFromNamedObjects()
    {
        try
        {
            // Search for objects with specific names
            var allObjects = GameObject.FindObjectsOfType<Transform>();
            
            foreach (var obj in allObjects)
            {
                if (obj == null) continue;
                
                foreach (string name in dollySettings.dollyTrackNames)
                {
                    if (string.IsNullOrEmpty(name)) continue;
                    
                    if (obj.name.Contains(name) && 
                        Vector3.Distance(transform.position, obj.position) <= dollySettings.searchRadius)
                    {
                        // If the object has children, add them as waypoints
                        if (obj.childCount > 0)
                        {
                            for (int i = 0; i < obj.childCount; i++)
                            {
                                var child = obj.GetChild(i);
                                if (child != null && !dollySettings.waypoints.Contains(child))
                                {
                                    dollySettings.waypoints.Add(child);
                                }
                            }
                        }
                        else
                        {
                            // Add the object itself as a waypoint
                            if (!dollySettings.waypoints.Contains(obj))
                            {
                                dollySettings.waypoints.Add(obj);
                            }
                        }
                        break; // Found a match, no need to check other names
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Error discovering named objects: {e.Message}");
        }
    }
    
    // Manual waypoint management methods
    public void AddWaypoint(Transform waypoint)
    {
        if (!dollySettings.waypoints.Contains(waypoint))
        {
            dollySettings.waypoints.Add(waypoint);
        }
    }
    
    public void RemoveWaypoint(int index)
    {
        if (index >= 0 && index < dollySettings.waypoints.Count)
        {
            dollySettings.waypoints.RemoveAt(index);
        }
    }
    
    public void ClearWaypoints()
    {
        dollySettings.waypoints.Clear();
    }
    
    // Public method to refresh waypoints
    public void RefreshWaypoints()
    {
        DiscoverWaypointsFromScene();
    }
    
    // Sort waypoints by distance from camera
    void SortWaypointsByDistance()
    {
        if (dollySettings.waypoints.Count < 2) return;
        
        // Sort waypoints by distance from camera position
        dollySettings.waypoints.Sort((a, b) => 
        {
            if (a == null || b == null) return 0;
            float distA = Vector3.Distance(transform.position, a.position);
            float distB = Vector3.Distance(transform.position, b.position);
            return distA.CompareTo(distB);
        });
        
        Debug.Log("Waypoints sorted by distance from camera");
    }
    
    // Draw gizmos for track visualization
    void OnDrawGizmos()
    {
        if (dollySettings.waypoints.Count < 2) return;
        
        // Draw track lines
        Gizmos.color = Color.yellow;
        
        for (int i = 0; i < dollySettings.waypoints.Count - 1; i++)
        {
            if (dollySettings.waypoints[i] != null && dollySettings.waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(
                    dollySettings.waypoints[i].position, 
                    dollySettings.waypoints[i + 1].position
                );
                
                // Draw waypoint spheres
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(dollySettings.waypoints[i].position, 0.5f);
                Gizmos.color = Color.yellow;
            }
        }
        
        // Draw last waypoint
        if (dollySettings.waypoints.Count > 0 && dollySettings.waypoints[dollySettings.waypoints.Count - 1] != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(dollySettings.waypoints[dollySettings.waypoints.Count - 1].position, 0.5f);
        }
        
        // Draw current position
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }
        
        // Draw search radius if auto-discovery is enabled
        if (dollySettings.autoDiscoverWaypoints)
        {
            Gizmos.color = new Color(0, 1, 0, 0.1f); // Semi-transparent green
            Gizmos.DrawWireSphere(transform.position, dollySettings.searchRadius);
        }
    }
} 