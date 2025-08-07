using UnityEngine;

public class ElephantWashTrigger : MonoBehaviour, IInteractable
{
    [Header("Wash System")]
    [Tooltip("Reference to the ElephantWashManager")]
    public ElephantWashManager washManager;
    
    [Header("Trigger Settings")]
    [Tooltip("Event name for TimeManager integration")]
    public string eventName = "ElephantWash";
    [Tooltip("Minimum time required to start wash (in minutes)")]
    public int minTimeRequired = 30;
    
    [Header("UI")]
    [Tooltip("Interaction hint text")]
    public string interactionText = "Press E to wash elephant";
    
    [Header("Audio")]
    [Tooltip("Sound to play when interaction is available")]
    public AudioClip interactionSound;
    private AudioSource audioSource;

    void Start()
    {
        // Set up audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Validate references
        if (washManager == null)
        {
            Debug.LogError("ElephantWashTrigger: No ElephantWashManager assigned!");
        }
    }

    /// <summary>
    /// Called when player interacts with the trigger
    /// </summary>
    public void Interact()
    {
        if (washManager == null)
        {
            Debug.LogError("ElephantWashTrigger: Cannot start wash - no manager assigned!");
            return;
        }
        
        // Check if we have enough time
        if (TimeManager.Instance != null)
        {
            if (!TimeManager.Instance.TryStartEvent(eventName))
            {
                Debug.LogWarning("ElephantWashTrigger: Not enough time to start wash!");
                // You could show a UI message here
                return;
            }
        }
        
        // Play interaction sound
        if (audioSource != null && interactionSound != null)
        {
            audioSource.PlayOneShot(interactionSound);
        }
        
        // Start the wash mini-game
        washManager.StartWash();
        
        Debug.Log("ElephantWashTrigger: Started elephant wash mini-game");
    }

    /// <summary>
    /// Get the interaction text for UI display
    /// </summary>
    public string GetInteractionText()
    {
        return interactionText;
    }

    /// <summary>
    /// Check if the wash can be started (has enough time)
    /// </summary>
    public bool CanStartWash()
    {
        if (TimeManager.Instance == null) return true;
        
        // Check if there's enough time in the current window
        int currentTime = TimeManager.Instance.GetCurrentTime();
        int windowEnd = GetCurrentWindowEnd();
        
        return (windowEnd - currentTime) >= minTimeRequired;
    }

    /// <summary>
    /// Get the end time of the current time window
    /// </summary>
    private int GetCurrentWindowEnd()
    {
        if (TimeManager.Instance == null) return 0;
        
        int currentTime = TimeManager.Instance.GetCurrentTime();
        
        // Check morning window (8:00 - 9:00)
        if (currentTime >= 8 * 60 && currentTime < 9 * 60)
        {
            return 9 * 60;
        }
        
        // Check evening window (18:00 - 23:00)
        if (currentTime >= 18 * 60 && currentTime < 23 * 60)
        {
            return 23 * 60;
        }
        
        return currentTime;
    }

    /// <summary>
    /// Test method to start wash (for debugging)
    /// </summary>
    [ContextMenu("Test Start Wash")]
    public void TestStartWash()
    {
        Interact();
    }

    /// <summary>
    /// Test method to check if wash can be started (for debugging)
    /// </summary>
    [ContextMenu("Test Can Start Wash")]
    public void TestCanStartWash()
    {
        bool canStart = CanStartWash();
        Debug.Log($"ElephantWashTrigger: Can start wash: {canStart}");
    }
} 