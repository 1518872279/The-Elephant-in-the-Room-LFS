using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Day Schedule (minutes since midnight)")]
    public int morningStart = 8 * 60;
    public int morningEnd   = 9 * 60;
    public int eveningStart = 18 * 60;
    public int eveningEnd   = 23 * 60;

    [Header("Fixed Event Durations (in minutes)")]
    public List<string> eventNames;
    public List<int> eventDurations; // in minutes

    private Dictionary<string, int> durations = new Dictionary<string,int>();
    public int currentTime;

    public event Action<int> OnTimeChanged;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        for (int i = 0; i < Math.Min(eventNames.Count, eventDurations.Count); i++)
            durations[eventNames[i]] = eventDurations[i];

        currentTime = morningStart;
        OnTimeChanged?.Invoke(currentTime);
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
} 