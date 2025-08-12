using UnityEngine;

/// <summary>
/// Example script demonstrating how to use the CameraOrbitRecorder
/// This script can be attached to any GameObject to set up automatic camera orbiting
/// </summary>
public class CameraOrbitExample : MonoBehaviour
{
    [Header("Example Configuration")]
    [Tooltip("The target object to orbit around (if not set, will use this GameObject)")]
    public Transform targetObject;
    
    [Tooltip("Automatically start orbiting when the scene starts")]
    public bool autoStartOrbit = false;
    
    [Header("Preset Configurations")]
    [Tooltip("Quick preset configurations for different recording styles")]
    public OrbitPreset currentPreset = OrbitPreset.Standard;
    
    private CameraOrbitRecorder orbitRecorder;
    
    public enum OrbitPreset
    {
        Standard,
        CloseUp,
        WideShot,
        SlowMotion,
        FastMotion,
        TopDown,
        LowAngle,
        Cinematic,
        Gameplay,
        Showcase
    }
    
    void Start()
    {
        // Get or add the CameraOrbitRecorder component
        orbitRecorder = GetComponent<CameraOrbitRecorder>();
        if (orbitRecorder == null)
        {
            orbitRecorder = gameObject.AddComponent<CameraOrbitRecorder>();
        }
        
        // Set the target object
        if (targetObject != null)
        {
            orbitRecorder.SetTarget(targetObject);
        }
        else
        {
            // If no target specified, look for common target objects
            FindAndSetTarget();
        }
        
        // Apply preset configuration
        ApplyPreset(currentPreset);
        
        // Auto-start if enabled
        if (autoStartOrbit)
        {
            Invoke(nameof(StartOrbit), 1f); // Delay by 1 second to ensure everything is set up
        }
    }
    
    void FindAndSetTarget()
    {
        // Try to find common target objects
        string[] commonTargetNames = {
            "Player", "PlayerController", "Character", "MainCharacter",
            "Elephant", "Target", "Focus", "Center", "Main"
        };
        
        string[] commonTargetTags = {
            "Player", "MainCamera", "Target", "Focus"
        };
        
        // Try to find by name first
        foreach (string name in commonTargetNames)
        {
            GameObject target = GameObject.Find(name);
            if (target != null)
            {
                orbitRecorder.SetTarget(target.transform);
                Debug.Log($"Found target by name: {name}");
                return;
            }
        }
        
        // Try to find by tag
        foreach (string tag in commonTargetTags)
        {
            GameObject target = GameObject.FindWithTag(tag);
            if (target != null)
            {
                orbitRecorder.SetTarget(target.transform);
                Debug.Log($"Found target by tag: {tag}");
                return;
            }
        }
        
        // If no target found, use this GameObject
        orbitRecorder.SetTarget(transform);
        Debug.Log("No specific target found, using this GameObject as target");
    }
    
    void ApplyPreset(OrbitPreset preset)
    {
        switch (preset)
        {
            case OrbitPreset.Standard:
                orbitRecorder.ConfigureOrbit(5f, 2f, 30f, 0f, 360f);
                orbitRecorder.orbitSettings.fieldOfView = 60f;
                break;
                
            case OrbitPreset.CloseUp:
                orbitRecorder.ConfigureOrbit(2f, 1f, 20f, 0f, 360f);
                orbitRecorder.orbitSettings.fieldOfView = 45f;
                break;
                
            case OrbitPreset.WideShot:
                orbitRecorder.ConfigureOrbit(10f, 3f, 15f, 0f, 360f);
                orbitRecorder.orbitSettings.fieldOfView = 90f;
                break;
                
            case OrbitPreset.SlowMotion:
                orbitRecorder.ConfigureOrbit(6f, 2.5f, 10f, 0f, 360f);
                orbitRecorder.orbitSettings.fieldOfView = 70f;
                break;
                
            case OrbitPreset.FastMotion:
                orbitRecorder.ConfigureOrbit(4f, 1.5f, 60f, 0f, 360f);
                orbitRecorder.orbitSettings.fieldOfView = 50f;
                break;
                
            case OrbitPreset.TopDown:
                orbitRecorder.ConfigureOrbit(8f, 8f, 25f, 0f, 360f);
                orbitRecorder.orbitSettings.fieldOfView = 45f;
                break;
                
            case OrbitPreset.LowAngle:
                orbitRecorder.ConfigureOrbit(3f, -1f, 35f, 0f, 360f);
                orbitRecorder.orbitSettings.fieldOfView = 55f;
                break;
                
            case OrbitPreset.Cinematic:
                orbitRecorder.ConfigureOrbit(7f, 3f, 15f, 0f, 360f);
                orbitRecorder.orbitSettings.fieldOfView = 50f;
                orbitRecorder.orbitSettings.smoothness = 1f;
                break;
                
            case OrbitPreset.Gameplay:
                orbitRecorder.ConfigureOrbit(4f, 1.5f, 45f, 0f, 360f);
                orbitRecorder.orbitSettings.fieldOfView = 65f;
                break;
                
            case OrbitPreset.Showcase:
                orbitRecorder.ConfigureOrbit(6f, 2f, 20f, 0f, 360f);
                orbitRecorder.orbitSettings.fieldOfView = 55f;
                orbitRecorder.orbitSettings.smoothness = 0.8f;
                break;
        }
    }
    
    public void StartOrbit()
    {
        if (orbitRecorder != null)
        {
            orbitRecorder.StartOrbit();
        }
    }
    
    public void StopOrbit()
    {
        if (orbitRecorder != null)
        {
            orbitRecorder.StopOrbit();
        }
    }
    
    public void SetPreset(OrbitPreset preset)
    {
        currentPreset = preset;
        ApplyPreset(preset);
    }
    
    // Public methods for runtime configuration
    public void SetOrbitSpeed(float speed)
    {
        if (orbitRecorder != null)
        {
            orbitRecorder.SetOrbitSpeed(speed);
        }
    }
    
    public void SetOrbitDistance(float distance)
    {
        if (orbitRecorder != null)
        {
            orbitRecorder.SetOrbitDistance(distance);
        }
    }
    
    public void SetHeightOffset(float height)
    {
        if (orbitRecorder != null)
        {
            orbitRecorder.SetHeightOffset(height);
        }
    }
    
    // Example methods for different orbit scenarios
    public void OrbitStandard()
    {
        if (orbitRecorder != null)
        {
            orbitRecorder.ConfigureOrbit(5f, 2f, 30f, 0f, 360f);
            orbitRecorder.StartOrbit();
        }
    }
    
    public void OrbitCloseUp()
    {
        if (orbitRecorder != null)
        {
            orbitRecorder.ConfigureOrbit(2f, 1f, 20f, 0f, 360f);
            orbitRecorder.orbitSettings.fieldOfView = 45f;
            orbitRecorder.StartOrbit();
        }
    }
    
    public void OrbitCinematic()
    {
        if (orbitRecorder != null)
        {
            orbitRecorder.ConfigureOrbit(7f, 3f, 15f, 0f, 360f);
            orbitRecorder.orbitSettings.fieldOfView = 50f;
            orbitRecorder.orbitSettings.smoothness = 1f;
            orbitRecorder.StartOrbit();
        }
    }
    
    public void OrbitGameplay()
    {
        if (orbitRecorder != null)
        {
            orbitRecorder.ConfigureOrbit(4f, 1.5f, 45f, 0f, 360f);
            orbitRecorder.orbitSettings.fieldOfView = 65f;
            orbitRecorder.StartOrbit();
        }
    }
    
    public void OrbitShowcase()
    {
        if (orbitRecorder != null)
        {
            orbitRecorder.ConfigureOrbit(6f, 2f, 20f, 0f, 360f);
            orbitRecorder.orbitSettings.fieldOfView = 55f;
            orbitRecorder.orbitSettings.smoothness = 0.8f;
            orbitRecorder.StartOrbit();
        }
    }
    
    void OnGUI()
    {
        if (orbitRecorder == null) return;
        
        // Create a simple UI for testing
        GUILayout.BeginArea(new Rect(Screen.width - 250, 10, 240, 350));
        GUILayout.Label("Camera Orbit Controls", GUI.skin.box);
        
        if (GUILayout.Button("Start Orbit"))
        {
            StartOrbit();
        }
        
        if (GUILayout.Button("Stop Orbit"))
        {
            StopOrbit();
        }
        
        GUILayout.Space(10);
        GUILayout.Label("Orbit Presets:");
        
        if (GUILayout.Button("Standard"))
        {
            SetPreset(OrbitPreset.Standard);
        }
        
        if (GUILayout.Button("Close Up"))
        {
            SetPreset(OrbitPreset.CloseUp);
        }
        
        if (GUILayout.Button("Wide Shot"))
        {
            SetPreset(OrbitPreset.WideShot);
        }
        
        if (GUILayout.Button("Cinematic"))
        {
            SetPreset(OrbitPreset.Cinematic);
        }
        
        if (GUILayout.Button("Gameplay"))
        {
            SetPreset(OrbitPreset.Gameplay);
        }
        
        if (GUILayout.Button("Showcase"))
        {
            SetPreset(OrbitPreset.Showcase);
        }
        
        GUILayout.Space(10);
        GUILayout.Label("Quick Orbit:");
        
        if (GUILayout.Button("Orbit Standard"))
        {
            OrbitStandard();
        }
        
        if (GUILayout.Button("Orbit Close Up"))
        {
            OrbitCloseUp();
        }
        
        if (GUILayout.Button("Orbit Cinematic"))
        {
            OrbitCinematic();
        }
        
        if (GUILayout.Button("Orbit Gameplay"))
        {
            OrbitGameplay();
        }
        
        if (GUILayout.Button("Orbit Showcase"))
        {
            OrbitShowcase();
        }
        
        GUILayout.EndArea();
    }
} 