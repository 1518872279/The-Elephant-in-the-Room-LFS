using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DayBasedObject
{
    [Tooltip("Day number when this object should be activated/deactivated")]
    public int day;
    [Tooltip("Game object to activate/deactivate")]
    public GameObject gameObject;
    [Tooltip("Whether the object should be active (true) or inactive (false) on this day")]
    public bool shouldBeActive;
    [Tooltip("Description of what this object represents (for organization)")]
    public string description;
}

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Day Schedule (minutes since midnight)")]
    public int morningStart = 8 * 60;
    public int morningEnd   = 9 * 60;
    public int eveningStart = 18 * 60;
    public int eveningEnd   = 23 * 60;

    [Header("Day Management")]
    [Tooltip("Current day number (starts at 1)")]
    public int currentDay = 1;

    [Header("Day-Based Object Management")]
    [Tooltip("Objects that should be activated/deactivated on specific days")]
    public List<DayBasedObject> dayBasedObjects = new List<DayBasedObject>();

    [Header("Fixed Event Durations (in minutes)")]
    public List<string> eventNames;
    public List<int> eventDurations; // in minutes

    [Header("Lighting Control")]
    [Tooltip("Interior lights to control")]
    public GameObject[] interiorLights;
    [Tooltip("Exterior lights to control")]
    public GameObject[] exteriorLights;
    [Tooltip("Whether lights should be on during evening")]
    public bool lightsOnInEvening = true;

    private Dictionary<string, int> durations = new Dictionary<string,int>();
    public int currentTime;
    private bool lightsCurrentlyOn = false;

    public event Action<int> OnTimeChanged;
    public event Action<int> OnDayChanged;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        for (int i = 0; i < Math.Min(eventNames.Count, eventDurations.Count); i++)
            durations[eventNames[i]] = eventDurations[i];

        currentTime = morningStart;
        OnTimeChanged?.Invoke(currentTime);
        OnDayChanged?.Invoke(currentDay);
    }

    void Start()
    {
        // Initialize lighting based on current time
        UpdateLighting();
        
        // Initialize day-based objects
        UpdateDayBasedObjects();
    }

    /// <summary>Tries to start an event by name. Advances time if within window.</summary>
    public bool TryStartEvent(string eventName)
    {
        if (!durations.TryGetValue(eventName, out int duration))
        {
            Debug.LogWarning($"Event '{eventName}' not defined.");
            return false;
        }
        int windowEnd = GetWindowEnd();
        if (currentTime + duration > windowEnd)
        {
            Debug.LogWarning($"Cannot start '{eventName}': exceeds time window.");
            return false;
        }
        currentTime += duration;
        OnTimeChanged?.Invoke(currentTime);
        UpdateLighting();
        return true;
    }

    private int GetWindowEnd()
    {
        if (currentTime >= morningStart && currentTime < morningEnd)
            return morningEnd;
        if (currentTime >= eveningStart && currentTime < eveningEnd)
            return eveningEnd;
        return currentTime;
    }

    public int GetCurrentTime() => currentTime;
    public int GetCurrentDay() => currentDay;

    /// <summary>Manually set the in-game clock time</summary>
    public void SetTime(int minutes)
    {
        currentTime = minutes;
        OnTimeChanged?.Invoke(currentTime);
        UpdateLighting();
    }

    /// <summary>Advance to the next day</summary>
    public void AdvanceDay()
    {
        currentDay++;
        OnDayChanged?.Invoke(currentDay);
        UpdateDayBasedObjects();
        Debug.Log($"TimeManager: Advanced to Day {currentDay}");
    }

    /// <summary>Set the current day</summary>
    public void SetDay(int day)
    {
        if (day >= 1)
        {
            currentDay = day;
            OnDayChanged?.Invoke(currentDay);
            UpdateDayBasedObjects();
            Debug.Log($"TimeManager: Set to Day {currentDay}");
        }
        else
        {
            Debug.LogWarning($"TimeManager: Invalid day number {day}. Must be 1 or greater");
        }
    }

    /// <summary>Update day-based objects based on current day</summary>
    private void UpdateDayBasedObjects()
    {
        foreach (DayBasedObject dayObject in dayBasedObjects)
        {
            if (dayObject.gameObject != null && dayObject.day == currentDay)
            {
                dayObject.gameObject.SetActive(dayObject.shouldBeActive);
                string action = dayObject.shouldBeActive ? "activated" : "deactivated";
                string description = string.IsNullOrEmpty(dayObject.description) ? dayObject.gameObject.name : dayObject.description;
                Debug.Log($"TimeManager: {description} {action} on Day {currentDay}");
            }
        }
    }

    /// <summary>Add a new day-based object programmatically</summary>
    public void AddDayBasedObject(int day, GameObject gameObject, bool shouldBeActive, string description = "")
    {
        DayBasedObject newObject = new DayBasedObject
        {
            day = day,
            gameObject = gameObject,
            shouldBeActive = shouldBeActive,
            description = description
        };
        
        dayBasedObjects.Add(newObject);
        
        // If this is for the current day, apply it immediately
        if (day == currentDay)
        {
            gameObject.SetActive(shouldBeActive);
            string action = shouldBeActive ? "activated" : "deactivated";
            Debug.Log($"TimeManager: {description} {action} on Day {currentDay}");
        }
    }

    /// <summary>Remove a day-based object by game object reference</summary>
    public void RemoveDayBasedObject(GameObject gameObject)
    {
        dayBasedObjects.RemoveAll(obj => obj.gameObject == gameObject);
    }

    /// <summary>Get all day-based objects for a specific day</summary>
    public List<DayBasedObject> GetDayBasedObjects(int day)
    {
        return dayBasedObjects.FindAll(obj => obj.day == day);
    }

    /// <summary>Update lighting based on current time</summary>
    private void UpdateLighting()
    {
        bool shouldLightsBeOn = IsEveningTime() && lightsOnInEvening;
        
        if (shouldLightsBeOn != lightsCurrentlyOn)
        {
            lightsCurrentlyOn = shouldLightsBeOn;
            
            // Update interior lights
            if (interiorLights != null)
            {
                foreach (GameObject lightObject in interiorLights)
                {
                    if (lightObject != null)
                    {
                        lightObject.SetActive(lightsCurrentlyOn);
                    }
                }
            }
            
            // Update exterior lights
            if (exteriorLights != null)
            {
                foreach (GameObject lightObject in exteriorLights)
                {
                    if (lightObject != null)
                    {
                        lightObject.SetActive(lightsCurrentlyOn);
                    }
                }
            }
            
            string timeOfDay = IsEveningTime() ? "evening" : "morning";
            string lightStatus = lightsCurrentlyOn ? "ON" : "OFF";
            Debug.Log($"TimeManager: Lights turned {lightStatus} ({timeOfDay} time)");
        }
    }

    /// <summary>Check if current time is in evening period</summary>
    private bool IsEveningTime()
    {
        return currentTime >= eveningStart && currentTime < eveningEnd;
    }

    /// <summary>Check if current time is in morning period</summary>
    private bool IsMorningTime()
    {
        return currentTime >= morningStart && currentTime < morningEnd;
    }

    /// <summary>Manually toggle lights on/off</summary>
    public void ToggleLights(bool turnOn)
    {
        lightsCurrentlyOn = turnOn;
        
        // Update interior lights
        if (interiorLights != null)
        {
            foreach (GameObject lightObject in interiorLights)
            {
                if (lightObject != null)
                {
                    lightObject.SetActive(turnOn);
                }
            }
        }
        
        // Update exterior lights
        if (exteriorLights != null)
        {
            foreach (GameObject lightObject in exteriorLights)
            {
                if (lightObject != null)
                {
                    lightObject.SetActive(turnOn);
                }
            }
        }
        
        string lightStatus = turnOn ? "ON" : "OFF";
        Debug.Log($"TimeManager: Lights manually turned {lightStatus}");
        
        // Notify DayPartManager if it exists
        if (DayPartManager.Instance != null)
        {
            // This allows DayPartManager to know about lighting changes
            // and potentially update its state accordingly
        }
    }
    
    /// <summary>Get current lighting state</summary>
    public bool AreLightsOn()
    {
        return lightsCurrentlyOn;
    }
    
    /// <summary>Check if current time should have lights on (based on evening time)</summary>
    public bool ShouldLightsBeOn()
    {
        return IsEveningTime() && lightsOnInEvening;
    }
} 