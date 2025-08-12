using UnityEngine;
using System;

public class ElephantStateController : MonoBehaviour
{
    public static ElephantStateController Instance { get; private set; }

    [Header("Core Values")]
    [Tooltip("0–100 range")]
    public float happiness = 50f;
    [Tooltip("0 = Elephant leaves")]
    public float stability = 100f;

    [Header("Event-Specific Modifiers")]
    [Tooltip("List of events and their happiness/stability deltas")]
    public ElephantEventModifier[] eventModifiers;

    [Header("Daily Decay")]
    [Tooltip("Stability lost each in-game day")]
    public float stabilityDecayPerDay = 5f;

    /// <summary>Fired when an event completes; passes event name.</summary>
    public event Action<string> OnReactionTriggered;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Call when an in-game event finishes, specifying its name.
    /// </summary>
    public void OnEventCompleted(string eventName)
    {
        // Find modifier for this event
        var modifier = Array.Find(eventModifiers, em => em.eventName == eventName);
        
        if (modifier.eventName != null)
        {
            happiness += modifier.happinessDelta;
            stability += modifier.stabilityDelta;
            ClampValues();
            OnReactionTriggered?.Invoke(eventName);
        }
        else
        {
            Debug.LogWarning($"No event modifier found for event: {eventName}");
        }
    }

    /// <summary>
    /// Applies daily stability decay; call at start of each in-game day.
    /// </summary>
    [ContextMenu("Apply Daily Stability Decay")]
    public void DecreaseDailyStability()
    {
        stability -= stabilityDecayPerDay;
        ClampValues();
        if (stability <= 0f)
            LeavePlayer();
    }
    
    /// <summary>
    /// Directly modify happiness and trigger the reaction event
    /// </summary>
    /// <param name="happinessDelta">Amount to change happiness by</param>
    /// <param name="eventName">Name of the event that caused this change</param>
    public void ModifyHappiness(float happinessDelta, string eventName = "")
    {
        happiness += happinessDelta;
        ClampValues();
        
        if (!string.IsNullOrEmpty(eventName))
        {
            OnReactionTriggered?.Invoke(eventName);
        }
        
        Debug.Log($"ElephantStateController: Happiness changed by {happinessDelta}. New happiness: {happiness}");
    }

    void ClampValues()
    {
        happiness = Mathf.Clamp(happiness, 0f, 100f);
        stability = Mathf.Max(0f, stability);
    }

    void LeavePlayer()
    {
        // TODO: implement departure behavior
        Debug.Log("Elephant has left due to low stability.");
    }
}

[System.Serializable]
public struct ElephantEventModifier
{
    public string eventName;
    [Tooltip("Delta to apply to happiness when this event occurs")]
    public float happinessDelta;
    [Tooltip("Delta to apply to stability when this event occurs")]
    public float stabilityDelta;
} 