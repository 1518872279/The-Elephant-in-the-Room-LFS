using UnityEngine;

/// <summary>
/// Simple example script for controlling TV flicker effects during video recording.
/// This script provides basic controls for switching between flicker patterns.
/// </summary>
public class TVFlickerSetupExample : MonoBehaviour
{
    [Header("TV Setup")]
    [Tooltip("Reference to the TV GameObject with TVFlickerLight component")]
    public TVFlickerLight tvFlickerLight;
    
    [Header("Pattern Control")]
    [Tooltip("Switch between different flicker patterns")]
    [Range(0, 2)]
    public int currentPattern = 0;
    
    [Header("Simple Controls")]
    [Tooltip("Manually toggle flicker on/off")]
    public bool toggleFlicker = false;
    
    [Header("Auto Pattern Change")]
    [Tooltip("Automatically change patterns")]
    public bool autoPatternChange = false;
    
    [Tooltip("Change pattern every X seconds")]
    [Range(5f, 60f)]
    public float patternChangeInterval = 30f;
    
    private float lastPatternChangeTime;
    
    void Start()
    {
        // If no TV flicker light assigned, try to find one
        if (tvFlickerLight == null)
        {
            tvFlickerLight = FindObjectOfType<TVFlickerLight>();
        }
        
        if (tvFlickerLight == null)
        {
            Debug.LogWarning("TVFlickerSetupExample: No TVFlickerLight found! Please assign one or ensure it exists in the scene.");
            return;
        }
        
        // Set initial pattern
        tvFlickerLight.SetFlickerPattern(currentPattern);
        lastPatternChangeTime = Time.time;
        
        Debug.Log("TVFlickerSetupExample: Setup complete. TV flicker system is ready for video recording.");
    }
    
    void Update()
    {
        if (tvFlickerLight == null) return;
        
        // Handle manual pattern changes
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetPattern(0); // Modern TV
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetPattern(1); // Old CRT TV
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetPattern(2); // Faulty TV
        }
        
        // Handle manual toggle
        if (Input.GetKeyDown(KeyCode.T))
        {
            tvFlickerLight.ToggleFlicker();
            Debug.Log($"TV Flicker toggled. Enabled: {tvFlickerLight.flickerEnabled}");
        }
        
        // Auto pattern change
        if (autoPatternChange && Time.time - lastPatternChangeTime > patternChangeInterval)
        {
            CyclePattern();
            lastPatternChangeTime = Time.time;
        }
    }
    
    void SetPattern(int patternIndex)
    {
        if (tvFlickerLight != null && patternIndex != currentPattern)
        {
            currentPattern = patternIndex;
            tvFlickerLight.SetFlickerPattern(patternIndex);
            
            string patternName = "Unknown";
            if (tvFlickerLight.flickerPatterns != null && patternIndex < tvFlickerLight.flickerPatterns.Length)
            {
                patternName = tvFlickerLight.flickerPatterns[patternIndex].patternName;
            }
            
            Debug.Log($"TV Pattern changed to: {patternName}");
        }
    }
    
    void CyclePattern()
    {
        if (tvFlickerLight.flickerPatterns == null || tvFlickerLight.flickerPatterns.Length == 0) return;
        
        int nextPattern = (currentPattern + 1) % tvFlickerLight.flickerPatterns.Length;
        SetPattern(nextPattern);
    }
    
    // Public methods for external control
    public void SetModernTVPattern()
    {
        SetPattern(0);
    }
    
    public void SetOldCRTPattern()
    {
        SetPattern(1);
    }
    
    public void SetFaultyTVPattern()
    {
        SetPattern(2);
    }
    
    public void EnableFlicker()
    {
        if (tvFlickerLight != null)
        {
            tvFlickerLight.flickerEnabled = true;
            tvFlickerLight.StartFlickering();
        }
    }
    
    public void DisableFlicker()
    {
        if (tvFlickerLight != null)
        {
            tvFlickerLight.flickerEnabled = false;
            tvFlickerLight.StopFlickering();
        }
    }
    
    void OnValidate()
    {
        // Update pattern when changed in inspector
        if (Application.isPlaying && tvFlickerLight != null)
        {
            SetPattern(currentPattern);
        }
    }
} 