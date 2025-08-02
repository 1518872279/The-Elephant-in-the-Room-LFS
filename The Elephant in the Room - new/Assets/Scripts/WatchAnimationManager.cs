using UnityEngine;

[RequireComponent(typeof(Animator))]
public class WatchAnimationManager : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("Animator component for the watch")]
    public Animator watchAnimator;
    
    [Header("Animation States")]
    [Tooltip("Name of the idle animation state")]
    public string idleStateName = "Idle";
    
    [Tooltip("Name of the check time animation state")]
    public string checkTimeStateName = "CheckTime";
    
    [Tooltip("Name of the put down animation state")]
    public string putDownStateName = "PutDown";
    
    [Header("Input Settings")]
    [Tooltip("Key to trigger watch animations")]
    public KeyCode triggerKey = KeyCode.Alpha3;
    
    [Header("Animation Parameters")]
    [Tooltip("Bool parameter to trigger check time animation")]
    public string checkTimeTrigger = "CheckTime";
    
    [Tooltip("Bool parameter to trigger put down animation")]
    public string putDownTrigger = "PutDown";
    
    [Tooltip("Bool parameter to return to idle")]
    public string idleTrigger = "Idle";
    
    [Header("Activation Settings")]
    [Tooltip("Whether the watch should be deactivated by default")]
    public bool startDeactivated = true;
    
    [Tooltip("Whether to deactivate the watch after put down animation")]
    public bool deactivateAfterPutDown = true;
    
    [Header("Time Synchronization")]
    [Tooltip("Whether to automatically sync watch hands with game time")]
    public bool autoSyncTime = true;
    
    [Tooltip("Transform for the hour hand")]
    public Transform hourHand;
    
    [Tooltip("Transform for the minute hand")]
    public Transform minuteHand;
    
    [Tooltip("Optional: Transform for the second hand")]
    public Transform secondHand;
    
    [Tooltip("Clockwise rotation direction (true = clockwise, false = counter-clockwise)")]
    public bool clockwiseRotation = true;

    private bool isCheckingTime = false;
    private bool isPuttingDown = false;
    private bool isInIdle = true;
    private bool isActivated = false;

    void Start()
    {
        // Get the animator component if not assigned
        if (watchAnimator == null)
        {
            watchAnimator = GetComponent<Animator>();
        }
        
        // Deactivate the watch by default if configured
        if (startDeactivated)
        {
            DeactivateWatch();
        }
        else
        {
            ActivateWatch();
        }
        
        // Ensure we start in idle state
        if (watchAnimator != null)
        {
            watchAnimator.SetBool(idleTrigger, true);
            watchAnimator.SetBool(checkTimeTrigger, false);
            watchAnimator.SetBool(putDownTrigger, false);
        }
        else
        {
            Debug.LogError("WatchAnimationManager: No Animator component found!");
        }
        
        // Initial time synchronization
        if (autoSyncTime)
        {
            SyncWatchTime();
            Debug.Log("current Time is " + TimeManager.Instance.GetCurrentTime());
        }
    }

    void Update()
    {
        // Update watch hands if auto sync is enabled and watch is activated
        if (autoSyncTime && isActivated)
        {
            UpdateWatchTime();
        }
    }

    void ToggleWatchAnimation()
    {
        if (watchAnimator == null) return;

        if (isInIdle)
        {
            // Start checking time
            StartCheckTimeAnimation();
        }
        else if (isCheckingTime)
        {
            // Put down the watch
            StartPutDownAnimation();
        }
        else if (isPuttingDown)
        {
            // Return to idle
            ReturnToIdle();
        }
    }

    void StartCheckTimeAnimation()
    {
        if (watchAnimator == null) return;

        // Reset all states
        watchAnimator.SetBool(idleTrigger, false);
        watchAnimator.SetBool(putDownTrigger, false);
        
        // Start check time animation
        watchAnimator.SetBool(checkTimeTrigger, true);
        
        isInIdle = false;
        isCheckingTime = true;
        isPuttingDown = false;
        
        Debug.Log("Watch: Starting check time animation");
    }

    void StartPutDownAnimation()
    {
        if (watchAnimator == null) return;

        // Reset all states
        watchAnimator.SetBool(idleTrigger, false);
        watchAnimator.SetBool(checkTimeTrigger, false);
        
        // Start put down animation
        watchAnimator.SetBool(putDownTrigger, true);
        
        isInIdle = false;
        isCheckingTime = false;
        isPuttingDown = true;
        
        Debug.Log("Watch: Starting put down animation");
    }

    void ReturnToIdle()
    {
        if (watchAnimator == null) return;

        // Reset all states
        watchAnimator.SetBool(checkTimeTrigger, false);
        watchAnimator.SetBool(putDownTrigger, false);
        
        // Return to idle
        watchAnimator.SetBool(idleTrigger, true);
        
        isInIdle = true;
        isCheckingTime = false;
        isPuttingDown = false;
        
        Debug.Log("Watch: Returning to idle state");
        // Deactivate the watch after put down if configured
        if (deactivateAfterPutDown)
        {
            DeactivateWatch();
        }
    }

    // Public methods for external control
    public void ForceCheckTime()
    {
        if (!isActivated)
        {
            ActivateWatch();
        }
        StartCheckTimeAnimation();
    }

    public void ForcePutDown()
    {
        if (!isActivated)
        {
            ActivateWatch();
        }
        StartPutDownAnimation();
    }

    public void ForceIdle()
    {
        if (!isActivated)
        {
            ActivateWatch();
        }
        ReturnToIdle();
    }
    
    // Main method to handle watch input from FirstPersonController
    public void HandleWatchInput()
    {
        if (!isActivated)
        {
            // Activate the watch and start check time animation
            ActivateWatch();
            StartCheckTimeAnimation();
        }
        else
        {
            // Handle normal animation cycling
            ToggleWatchAnimation();
        }
    }
    
    // Activation control methods
    public void ActivateWatch()
    {
        gameObject.SetActive(true);
        isActivated = true;
        Debug.Log("Watch: Activated");
    }
    
    public void DeactivateWatch()
    {
        gameObject.SetActive(false);
        isActivated = false;
        Debug.Log("Watch: Deactivated");
    }

    // Animation event callbacks (can be called from animation events)
    public void OnCheckTimeAnimationComplete()
    {
        // This can be called from an animation event when check time animation finishes
        Debug.Log("Watch: Check time animation completed");
    }

    public void OnPutDownAnimationComplete()
    {
        // This can be called from an animation event when put down animation finishes
        Debug.Log("Watch: Put down animation completed");
    }

    // Time synchronization methods
    public void SyncWatchTime()
    {
        if (!isActivated) return;
        
        if (TimeManager.Instance != null)
        {
            int gameMinutes = TimeManager.Instance.GetCurrentTime();
            int hours = gameMinutes / 60;
            int minutes = gameMinutes % 60;
            UpdateWatchHands(hours, minutes, 0);
            
            Debug.Log($"Watch: Synchronized to game time {hours:D2}:{minutes:D2}:00");

        }
        else
        {
            Debug.LogWarning("Watch: TimeManager not found, using real time instead");
            System.DateTime currentTime = System.DateTime.Now;
            UpdateWatchHands(currentTime.Hour, currentTime.Minute, currentTime.Second);
        }
    }
    
    public void SyncWatchTime(int hour, int minute, int second = 0)
    {
        if (!isActivated) return;
        
        UpdateWatchHands(hour, minute, second);
        
        Debug.Log($"Watch: Synchronized to {hour:D2}:{minute:D2}:{second:D2}");
    }
    
    public void SyncWatchTime(int gameMinutes)
    {
        if (!isActivated) return;
        
        int hours = gameMinutes / 60;
        int minutes = gameMinutes % 60;
        UpdateWatchHands(hours, minutes, 0);
        
        Debug.Log($"Watch: Synchronized to game time {hours:D2}:{minutes:D2}:00 ({gameMinutes} minutes)");
    }
    
    public void UpdateWatchTime()
    {
        if (!isActivated) return;
        
        if (TimeManager.Instance != null)
        {
            int gameMinutes = TimeManager.Instance.GetCurrentTime();
            int hours = gameMinutes / 60;
            int minutes = gameMinutes % 60;
            UpdateWatchHands(hours, minutes, 0);
        }
        else
        {
            // Fallback to real time if TimeManager not available
            System.DateTime currentTime = System.DateTime.Now;
            UpdateWatchHands(currentTime.Hour, currentTime.Minute, currentTime.Second);
        }
    }
    
    private void UpdateWatchHands(int hour, int minute, int second)
    {
        // Convert to 12-hour format for hour hand
        int hour12 = hour % 12;
        if (hour12 == 0) hour12 = 12;
        
        // Calculate rotation angles
        // Hour hand: 30 degrees per hour (360/12) + minute influence
        float hourAngle = (hour12 * 30f) + (minute * 0.5f); // 0.5 degrees per minute
        
        // Minute hand: 6 degrees per minute (360/60)
        float minuteAngle = minute * 6f;
        
        // Second hand: 6 degrees per second (360/60)
        float secondAngle = second * 6f;
        
        // Apply rotation direction
        if (!clockwiseRotation)
        {
            hourAngle = -hourAngle;
            minuteAngle = -minuteAngle;
            secondAngle = -secondAngle;
        }
        
        // Update hour hand
        if (hourHand != null)
        {
            hourHand.localRotation = Quaternion.Euler(0, hourAngle, 0);
        }
        
        // Update minute hand
        if (minuteHand != null)
        {
            minuteHand.localRotation = Quaternion.Euler(0, minuteAngle, 0);
        }
        
        // Update second hand (optional)
        if (secondHand != null)
        {
            secondHand.localRotation = Quaternion.Euler(0, secondAngle, 0);
        }
    }
    
    // Getter methods for external scripts to check current state
    public bool IsCheckingTime => isCheckingTime;
    public bool IsPuttingDown => isPuttingDown;
    public bool IsInIdle => isInIdle;
    public bool IsActivated => isActivated;
} 