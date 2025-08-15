using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

public class DayPartManager : MonoBehaviour
{
    public static DayPartManager Instance { get; private set; }

    public Volume morningVolume;
    public Volume eveningVolume;
    public Light directionalLight;

    [Header("Skybox Materials")]
    [Tooltip("Skybox material to use during morning hours")]
    public Material morningSkybox;
    [Tooltip("Skybox material to use during evening hours")]
    public Material eveningSkybox;

    [Header("Lighting Intensities")]
    public float morningIntensity = 1f;
    public float eveningIntensity = 0.5f;

    [Header("Day Counter")]
    [Tooltip("Number of in-game days that have passed")]
    public int daysElapsed = 1;
    
    [Header("Video Recording Controls")]
    [Tooltip("Manually advance to the next day for testing")]
    public bool advanceToNextDay = false;
    
    [Tooltip("Manually switch day part for video recording")]
    public bool switchToMorning = false;
    public bool switchToEvening = false;
    
    [Tooltip("Current day part for video recording (can be manually set)")]
    public DayPart manualDayPart = DayPart.Morning;
    
    [Header("Lighting Control")]
    [Tooltip("Whether to control TimeManager lighting when switching day parts")]
    public bool controlTimeManagerLighting = true;
    
    [Tooltip("Manually toggle lights on/off for video recording")]
    public bool toggleLightsOn = false;
    public bool toggleLightsOff = false;

    public enum DayPart { None, Morning, Evening }
    public DayPart currentPart = DayPart.None;
    
    // Event for day part changes
    public event Action<DayPart> OnDayPartChanged;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        TimeManager.Instance.OnTimeChanged += OnTimeChanged;
        // Initialize volumes & light based on current time
        OnTimeChanged(TimeManager.Instance.GetCurrentTime());
    }

    void OnDestroy()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnTimeChanged -= OnTimeChanged;
    }

    private void OnTimeChanged(int minutes)
    {
        var newPart = DeterminePart(minutes);
        if (newPart != currentPart)
        {
            // If we're moving from Evening to Morning, that's a new day
            if (currentPart == DayPart.Evening && newPart == DayPart.Morning)
                daysElapsed++;

            OnDayPartChanged?.Invoke(newPart);    // fire event
            ApplyPart(newPart);
            currentPart = newPart;
        }
    }

    private DayPart DeterminePart(int minutes)
    {
        if (minutes >= TimeManager.Instance.morningStart && minutes < TimeManager.Instance.morningEnd)
            return DayPart.Morning;
        if (minutes >= TimeManager.Instance.eveningStart && minutes < TimeManager.Instance.eveningEnd)
            return DayPart.Evening;
        return currentPart; // remain in current part if outside defined windows
    }

    private void ApplyPart(DayPart part)
    {
        morningVolume.weight = (part == DayPart.Morning) ? 1f : 0f;
        eveningVolume.weight = (part == DayPart.Evening) ? 1f : 0f;

        switch (part)
        {
            case DayPart.Morning:
                directionalLight.intensity = morningIntensity;
                if (morningSkybox != null)
                {
                    RenderSettings.skybox = morningSkybox;
                    Debug.Log($"DayPartManager: Changed to morning skybox: {morningSkybox.name}");
                }
                // Control TimeManager lighting
                if (controlTimeManagerLighting && TimeManager.Instance != null)
                {
                    TimeManager.Instance.ToggleLights(false); // Turn off lights in morning
                }
                break;
            case DayPart.Evening:
                directionalLight.intensity = eveningIntensity;
                if (eveningSkybox != null)
                {
                    RenderSettings.skybox = eveningSkybox;
                    Debug.Log($"DayPartManager: Changed to evening skybox: {eveningSkybox.name}");
                }
                // Control TimeManager lighting
                if (controlTimeManagerLighting && TimeManager.Instance != null)
                {
                    TimeManager.Instance.ToggleLights(true); // Turn on lights in evening
                }
                break;
        }
    }
    
    /// <summary>
    /// Manually switch to morning for video recording
    /// </summary>
    public void SwitchToMorning()
    {
        OnDayPartChanged?.Invoke(DayPart.Morning);
        ApplyPart(DayPart.Morning);
        currentPart = DayPart.Morning;
        manualDayPart = DayPart.Morning;
        Debug.Log("DayPartManager: Manually switched to Morning for video recording");
    }
    
    /// <summary>
    /// Manually switch to evening for video recording
    /// </summary>
    public void SwitchToEvening()
    {
        OnDayPartChanged?.Invoke(DayPart.Evening);
        ApplyPart(DayPart.Evening);
        currentPart = DayPart.Evening;
        manualDayPart = DayPart.Evening;
        Debug.Log("DayPartManager: Manually switched to Evening for video recording");
    }
    
    /// <summary>
    /// Manually set day part for video recording
    /// </summary>
    public void SetDayPart(DayPart dayPart)
    {
        OnDayPartChanged?.Invoke(dayPart);
        ApplyPart(dayPart);
        currentPart = dayPart;
        manualDayPart = dayPart;
        Debug.Log($"DayPartManager: Manually set day part to {dayPart} for video recording");
    }
    
    /// <summary>
    /// Manually toggle lights on/off for video recording
    /// </summary>
    public void ToggleLights(bool turnOn)
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.ToggleLights(turnOn);
            Debug.Log($"DayPartManager: Manually toggled lights {(turnOn ? "ON" : "OFF")} for video recording");
        }
        else
        {
            Debug.LogWarning("DayPartManager: TimeManager not found, cannot toggle lights");
        }
    }
    
    /// <summary>
    /// Manually advance to the next day for testing purposes
    /// </summary>
    public void AdvanceToNextDay()
    {
        daysElapsed++;
        Debug.Log($"DayPartManager: Manually advanced to day {daysElapsed}");
        
        // Trigger day part change to Morning to simulate new day
        OnDayPartChanged?.Invoke(DayPart.Morning);
        ApplyPart(DayPart.Morning);
        currentPart = DayPart.Morning;
    }
    
    /// <summary>
    /// Set to a specific day for testing
    /// </summary>
    public void SetDay(int day)
    {
        if (day < 1) day = 1;
        daysElapsed = day;
        Debug.Log($"DayPartManager: Set to day {daysElapsed}");
        
        // Trigger day part change to Morning to simulate new day
        OnDayPartChanged?.Invoke(DayPart.Morning);
        ApplyPart(DayPart.Morning);
        currentPart = DayPart.Morning;
    }
    
    /// <summary>
    /// Test method to advance to next day (for debugging)
    /// </summary>
    [ContextMenu("Advance to Next Day")]
    public void TestAdvanceToNextDay()
    {
        AdvanceToNextDay();
    }
    
    /// <summary>
    /// Test method to set to day 2 (for debugging)
    /// </summary>
    [ContextMenu("Set to Day 2")]
    public void TestSetToDay2()
    {
        SetDay(2);
    }
    
    /// <summary>
    /// Test method to set to day 3 (for debugging)
    /// </summary>
    [ContextMenu("Set to Day 3")]
    public void TestSetToDay3()
    {
        SetDay(3);
    }
    
    /// <summary>
    /// Test method to switch to morning (for debugging)
    /// </summary>
    [ContextMenu("Switch to Morning")]
    public void TestSwitchToMorning()
    {
        SwitchToMorning();
    }
    
    /// <summary>
    /// Test method to switch to evening (for debugging)
    /// </summary>
    [ContextMenu("Switch to Evening")]
    public void TestSwitchToEvening()
    {
        SwitchToEvening();
    }
    
    /// <summary>
    /// Test method to turn lights on (for debugging)
    /// </summary>
    [ContextMenu("Turn Lights On")]
    public void TestTurnLightsOn()
    {
        ToggleLights(true);
    }
    
    /// <summary>
    /// Test method to turn lights off (for debugging)
    /// </summary>
    [ContextMenu("Turn Lights Off")]
    public void TestTurnLightsOff()
    {
        ToggleLights(false);
    }
    
    /// <summary>
    /// Manually set the skybox material
    /// </summary>
    public void SetSkybox(Material skyboxMaterial)
    {
        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
            Debug.Log($"DayPartManager: Manually set skybox to: {skyboxMaterial.name}");
        }
        else
        {
            Debug.LogWarning("DayPartManager: Attempted to set skybox to null material!");
        }
    }
    
    /// <summary>
    /// Test method to set morning skybox (for debugging)
    /// </summary>
    [ContextMenu("Set Morning Skybox")]
    public void TestSetMorningSkybox()
    {
        SetSkybox(morningSkybox);
    }
    
    /// <summary>
    /// Test method to set evening skybox (for debugging)
    /// </summary>
    [ContextMenu("Set Evening Skybox")]
    public void TestSetEveningSkybox()
    {
        SetSkybox(eveningSkybox);
    }
    
    void Update()
    {
        // Check for manual day advancement in inspector
        if (advanceToNextDay)
        {
            advanceToNextDay = false; // Reset the flag
            AdvanceToNextDay();
        }
        
        // Check for manual day part switching in inspector
        if (switchToMorning)
        {
            switchToMorning = false; // Reset the flag
            SwitchToMorning();
        }
        
        if (switchToEvening)
        {
            switchToEvening = false; // Reset the flag
            SwitchToEvening();
        }
        
        // Check for manual lighting control in inspector
        if (toggleLightsOn)
        {
            toggleLightsOn = false; // Reset the flag
            ToggleLights(true);
        }
        
        if (toggleLightsOff)
        {
            toggleLightsOff = false; // Reset the flag
            ToggleLights(false);
        }
    }
}
