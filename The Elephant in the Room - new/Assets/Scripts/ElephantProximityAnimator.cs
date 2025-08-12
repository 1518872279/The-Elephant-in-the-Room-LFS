using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class ElephantProximityAnimator : MonoBehaviour
{
    [Header("Proximity Settings")]
    [Tooltip("Distance at which the elephant starts reacting to player")]
    public float proximityDistance = 5f;
    [Tooltip("Fixed time range for animation triggering (in seconds)")]
    public float animationTimeRange = 5f;
    
    [Header("Animation Settings")]
    [Tooltip("List of animation triggers to randomly choose from")]
    public List<string> randomAnimationTriggers = new List<string>();
    [Tooltip("Minimum time between animations (in seconds)")]
    public float minTimeBetweenAnimations = 2f;
    
    [Header("Player Detection")]
    [Tooltip("Tag of the player GameObject")]
    public string playerTag = "Player";
    [Tooltip("Layer mask for player detection")]
    public LayerMask playerLayer = -1;
    
    [Header("Debug")]
    [Tooltip("Show debug information in console")]
    public bool showDebug = false;
    
    private Animator animator;
    private Transform playerTransform;
    private bool playerInRange = false;
    private bool isPlayingAnimation = false;
    private bool hasTriggeredInRange = false;
    private float lastAnimationTime = 0f;
    private float rangeEntryTime = 0f;
    
    void Start()
    {
        // Get required components
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("ElephantProximityAnimator: No Animator component found!");
            enabled = false;
            return;
        }
        
        // Find player
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning($"ElephantProximityAnimator: No GameObject with tag '{playerTag}' found!");
        }
        
        // Validate animation triggers
        if (randomAnimationTriggers.Count == 0)
        {
            Debug.LogWarning("ElephantProximityAnimator: No animation triggers defined! Add some in the inspector.");
        }
    }
    
    void Update()
    {
        CheckPlayerProximity();
    }
    
    void CheckPlayerProximity()
    {
        if (playerTransform == null) return;
        
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool wasInRange = playerInRange;
        playerInRange = distance <= proximityDistance;
        
        // Log when player enters/exits range
        if (playerInRange != wasInRange)
        {
            if (playerInRange)
            {
                if (showDebug) Debug.Log($"Elephant: Player entered proximity range ({distance:F1}m)");
                OnPlayerEnteredRange();
            }
            else
            {
                if (showDebug) Debug.Log($"Elephant: Player left proximity range ({distance:F1}m)");
                OnPlayerLeftRange();
            }
        }
        
        // Check if we should trigger animation within the time range
        if (playerInRange && !hasTriggeredInRange && !isPlayingAnimation)
        {
            float timeInRange = Time.time - rangeEntryTime;
            if (timeInRange >= animationTimeRange)
            {
                PlayRandomAnimation();
                hasTriggeredInRange = true;
            }
        }
    }
    
    void OnPlayerEnteredRange()
    {
        // Reset trigger flag and record entry time
        hasTriggeredInRange = false;
        rangeEntryTime = Time.time;
        
        if (showDebug) Debug.Log($"Elephant: Animation will trigger in {animationTimeRange} seconds if player stays in range");
    }
    
    void OnPlayerLeftRange()
    {
        // Reset trigger flag when player leaves
        hasTriggeredInRange = false;
        
        // Reset all animation triggers to false
        if (animator != null)
        {
            foreach (string trigger in randomAnimationTriggers)
            {
                animator.ResetTrigger(trigger);
            }
        }
        
        // Stop any currently playing animation
        isPlayingAnimation = false;
        
        if (showDebug) Debug.Log("Elephant: Player left range - reset animation trigger and stopped animations");
    }
    
    void PlayRandomAnimation()
    {
        if (randomAnimationTriggers.Count == 0)
        {
            Debug.LogWarning("ElephantProximityAnimator: No animation triggers available!");
            return;
        }
        
        // Check if enough time has passed since last animation
        if (Time.time - lastAnimationTime < minTimeBetweenAnimations)
        {
            if (showDebug) Debug.Log($"Elephant: Skipping animation - too soon since last one");
            return;
        }
        
        // Select random animation trigger
        string randomTrigger = randomAnimationTriggers[Random.Range(0, randomAnimationTriggers.Count)];
        
        // Play the animation
        animator.SetTrigger(randomTrigger);
        isPlayingAnimation = true;
        lastAnimationTime = Time.time;
        
        if (showDebug) Debug.Log($"Elephant: Playing animation '{randomTrigger}' after {animationTimeRange} seconds in range");
        
        // Start coroutine to reset animation state
        StartCoroutine(ResetAnimationState());
    }
    
    IEnumerator ResetAnimationState()
    {
        // Wait for animation to complete (you might need to adjust this based on your animations)
        yield return new WaitForSeconds(minTimeBetweenAnimations);
        
        isPlayingAnimation = false;
        
        if (showDebug) Debug.Log("Elephant: Animation state reset");
    }
    
    // Public method to manually trigger a random animation
    public void TriggerRandomAnimation()
    {
        if (!isPlayingAnimation)
        {
            PlayRandomAnimation();
        }
    }
    
    // Public method to trigger a specific animation
    public void TriggerAnimation(string triggerName)
    {
        if (animator != null && !isPlayingAnimation)
        {
            animator.SetTrigger(triggerName);
            isPlayingAnimation = true;
            lastAnimationTime = Time.time;
            
            if (showDebug) Debug.Log($"Elephant: Playing specific animation '{triggerName}'");
            
            StartCoroutine(ResetAnimationState());
        }
    }
    
    // Public method to reset the trigger state (useful for testing)
    public void ResetTriggerState()
    {
        hasTriggeredInRange = false;
        if (showDebug) Debug.Log("Elephant: Trigger state reset");
    }
    
    // Gizmos for debugging proximity range
    void OnDrawGizmosSelected()
    {
        Gizmos.color = playerInRange ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, proximityDistance);
    }
} 