# Elephant Emotional State & Behavior

This document defines a simple emotional state system for the elephant, tracking two core values—**Happiness** and **Stability**—and a basic behavior tree that triggers animations based on event outcomes.

---

## 1. Core Values & Inspector Settings

* **Happiness** (`0–100`)
* **Stability** (no upper bound; reaches `0` → elephant leaves)

Editable in Inspector:

```csharp
[Header("Core Values")]
[Tooltip("0–100 range")] public float happiness = 50f;
[Tooltip("0 = Elephant leaves")] public float stability = 100f;

[Header("Event Modifiers")]
[Tooltip("Delta applied to happiness on positive outcome")] public float happinessIncrease = 10f;
[Tooltip("Delta applied to happiness on negative outcome")] public float happinessDecrease = 5f;
[Tooltip("Delta applied to stability whenever an event completes")] public float stabilityChangeOnEvent = 2f;

[Header("Daily Decay")]
[Tooltip("Stability lost each in-game day")] public float stabilityDecayPerDay = 5f;
```

---

## 2. ElephantStateController

Handles value updates, daily decay, and emits reaction events.

```csharp
// ElephantStateController.cs
using UnityEngine;
using System;

public class ElephantStateController : MonoBehaviour
{
    public static ElephantStateController Instance { get; private set; }

    [Header("Core Values")]
    public float happiness = 50f;
    public float stability = 100f;

    [Header("Event Modifiers")]
    public float happinessIncrease = 10f;
    public float happinessDecrease = 5f;
    public float stabilityChangeOnEvent = 2f;

    [Header("Daily Decay")]
    public float stabilityDecayPerDay = 5f;

    // Fired whenever an event completes: true = positive, false = negative
    public event Action<bool> OnReactionTriggered;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Call when an in-game event finishes.
    /// </summary>
    public void OnEventCompleted(bool positive)
    {
        if (positive)
            happiness += happinessIncrease;
        else
            happiness -= happinessDecrease;

        stability += stabilityChangeOnEvent;
        ClampValues();
        OnReactionTriggered?.Invoke(positive);
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
```

---

## 3. ElephantBehaviorController

Listens for reaction events and triggers Animator parameters.

```csharp
// ElephantBehaviorController.cs
using UnityEngine;

public class ElephantBehaviorController : MonoBehaviour
{
    [Tooltip("Animator with PositiveReaction and NegativeReaction triggers")]  
    public Animator animator;

    void OnEnable()
    {
        ElephantStateController.Instance.OnReactionTriggered += HandleReaction;
    }

    void OnDisable()
    {
        ElephantStateController.Instance.OnReactionTriggered -= HandleReaction;
    }

    void HandleReaction(bool positive)
    {
        animator.SetTrigger(positive ? "PositiveReaction" : "NegativeReaction");
    }
}
```

---

## 4. Simple Behavior Tree Outline

1. **Event Completed** → `OnEventCompleted(positive)`

   * Update **Happiness** and **Stability**.
   * Invoke `OnReactionTriggered(positive)` → animation trigger.
2. **New In-Game Day** → `DecreaseDailyStability()`

   * If `stability <= 0` → elephant leaves (implement departure logic).

---

## 5. Integration Tips

* After any mini-game or event, call:

  ```csharp
  // True for positive outcome, false for negative
  ElephantStateController.Instance.OnEventCompleted(positiveOutcome);
  ```

* Schedule daily decay at midnight or via your time system:

  ```csharp
  ElephantStateController.Instance.DecreaseDailyStability();
  ```

---

## 6. UI Emotional Bars

Displays current **Happiness** and **Stability** on screen via two UI bars.

### 6.1. Canvas Setup

1. In your **Canvas** (Screen Space – Overlay), create two UI **Slider** objects (or **Image**s with **Image Type : Filled**):

   * **HappinessBar**

     * Min = 0, Max = 100, Value = initial happiness
     * Optionally add a **Text** label “Happiness”.
   * **StabilityBar**

     * Min = 0, Max = 100 (or dynamic), Value = initial stability
     * Label “Stability”.
2. Position them in a HUD-friendly area (e.g. top-left).

### 6.2. ElephantUIController

Attach this to a UI manager GameObject and wire up the bars:

```csharp
// ElephantUIController.cs
using UnityEngine;
using UnityEngine.UI;

public class ElephantUIController : MonoBehaviour
{
    [Tooltip("Slider or Filled Image for Happiness")]  
    public Slider happinessBar;
    [Tooltip("Slider or Filled Image for Stability")]  
    public Slider stabilityBar;

    private ElephantStateController state;

    void Start()
    {
        state = ElephantStateController.Instance;
        // Initialize sliders
        happinessBar.minValue = 0;
        happinessBar.maxValue = 100;
        stabilityBar.minValue = 0;
        stabilityBar.maxValue = 100;

        UpdateBars();
        // Subscribe to changes
        state.OnReactionTriggered += _ => UpdateBars();
    }

    void UpdateBars()
    {
        happinessBar.value = state.happiness;
        stabilityBar.value = state.stability;
    }

    void OnDestroy()
    {
        if (state != null)
            state.OnReactionTriggered -= _ => UpdateBars();
    }
}
```

*End of Elephant Behavior Setup.*
