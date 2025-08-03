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

    [Header("Lighting Intensities")]
    public float morningIntensity = 1f;
    public float eveningIntensity = 0.5f;

    [Header("Day Counter")]
    [Tooltip("Number of in-game days that have passed")]
    public int daysElapsed = 1;
    
    [Header("Testing")]
    [Tooltip("Manually advance to the next day for testing")]
    public bool advanceToNextDay = false;

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
                break;
            case DayPart.Evening:
                directionalLight.intensity = eveningIntensity;
                break;
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
    
    void Update()
    {
        // Check for manual day advancement in inspector
        if (advanceToNextDay)
        {
            advanceToNextDay = false; // Reset the flag
            AdvanceToNextDay();
        }
    }
}
