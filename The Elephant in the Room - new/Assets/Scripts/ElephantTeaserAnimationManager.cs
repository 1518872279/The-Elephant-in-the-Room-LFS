using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ElephantTeaserAnimationManager : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("Animator component for the elephant teaser")]
    public Animator teaserAnimator;
    
    [Header("Animation States")]
    [Tooltip("Name of the take out animation state")]
    public string takeOutStateName = "TakeOut";
    
    [Tooltip("Name of the using animation state")]
    public string usingStateName = "Using";
    
    [Tooltip("Name of the take back animation state")]
    public string takeBackStateName = "TakeBack";
    
    [Header("Animation Parameters")]
    [Tooltip("Bool parameter to trigger take out animation")]
    public string takeOutTrigger = "TakeOut";
    
    [Tooltip("Bool parameter to trigger using animation")]
    public string usingTrigger = "Using";
    
    [Tooltip("Bool parameter to trigger take back animation")]
    public string takeBackTrigger = "TakeBack";
    
    [Header("Activation Settings")]
    [Tooltip("Whether the teaser should be deactivated by default")]
    public bool startDeactivated = true;
    
    [Tooltip("Whether to deactivate the teaser after take back animation")]
    public bool deactivateAfterTakeBack = true;
    
    [Header("Using Animation Settings")]
    [Tooltip("Duration of the using animation in seconds")]
    public float usingAnimationDuration = 2f;
    
    [Header("Elephant State Effects")]
    [Tooltip("Happiness increase when using the elephant teaser")]
    public float happinessIncrease = 15f;
    [Tooltip("Event name for the elephant teaser usage")]
    public string teaserEventName = "ElephantTeaser";
    
    [Header("Proximity Settings")]
    [Tooltip("Reference to the elephant GameObject")]
    public GameObject elephantObject;
    [Tooltip("Maximum distance from elephant for happiness increase (in units)")]
    public float maxProximityDistance = 5f;
    [Tooltip("Tag of the player GameObject for distance calculation")]
    public string playerTag = "Player";

    private bool isTakingOut = false;
    private bool isUsing = false;
    private bool isTakingBack = false;
    private bool isActivated = false;
    private bool isTakenOut = false;
    private float usingTimer = 0f;
    private Transform playerTransform;

    void Start()
    {
        // Get the animator component if not assigned
        if (teaserAnimator == null)
        {
            teaserAnimator = GetComponent<Animator>();
        }
        
        // Find player for proximity checking
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning($"ElephantTeaserAnimationManager: No GameObject with tag '{playerTag}' found!");
        }
        
        // Deactivate the teaser by default if configured
        if (startDeactivated)
        {
            DeactivateTeaser();
        }
        else
        {
            ActivateTeaser();
        }
        
        // Ensure we start with no animations playing
        if (teaserAnimator != null)
        {
            teaserAnimator.SetBool(takeOutTrigger, false);
            teaserAnimator.SetBool(usingTrigger, false);
            teaserAnimator.SetBool(takeBackTrigger, false);
        }
        else
        {
            Debug.LogError("ElephantTeaserAnimationManager: No Animator component found!");
        }
    }

    void Update()
    {
        // Handle using animation timer
        if (isUsing)
        {
            usingTimer += Time.deltaTime;
            if (usingTimer >= usingAnimationDuration)
            {
                StopUsingAnimation();
            }
        }
        
        // Check for animation completion
        CheckAnimationCompletion();
    }
    
    void CheckAnimationCompletion()
    {
        if (teaserAnimator == null) return;
        
        // Check if take out animation completed
        if (isTakingOut && teaserAnimator.GetCurrentAnimatorStateInfo(0).IsName("TakeOut"))
        {
            // Check if animation has finished
            if (teaserAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                OnTakeOutAnimationComplete();
            }
        }
        
        // Check if take back animation completed
        if (isTakingBack && teaserAnimator.GetCurrentAnimatorStateInfo(0).IsName("TakeBack"))
        {
            // Check if animation has finished
            if (teaserAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                OnTakeBackAnimationComplete();
            }
        }
    }

    // Main method to handle teaser input from FirstPersonController
    public void HandleTeaserInput()
    {
        if (!isActivated)
        {
            // Activate the teaser and start take out animation
            ActivateTeaser();
            StartTakeOutAnimation();
        }
        else if (isTakenOut)
        {
            // If already taken out, start take back animation
            StartTakeBackAnimation();
        }
        else
        {
            // If not taken out, start take out animation
            StartTakeOutAnimation();
        }
    }
    
    // Method to handle mouse input from FirstPersonController
    public void HandleTeaserMouseInput()
    {
        if (isTakenOut && !isUsing)
        {
            StartUsingAnimation();
        }
    }

    void StartTakeOutAnimation()
    {
        if (teaserAnimator == null) return;

        // Reset all states
        teaserAnimator.SetBool(usingTrigger, false);
        teaserAnimator.SetBool(takeBackTrigger, false);
        
        // Start take out animation
        teaserAnimator.SetBool(takeOutTrigger, true);
        
        isTakingOut = true;
        isUsing = false;
        isTakingBack = false;
        

    }

    void StartUsingAnimation()
    {
        if (teaserAnimator == null) return;

        // Reset all states
        teaserAnimator.SetBool(takeOutTrigger, false);
        teaserAnimator.SetBool(takeBackTrigger, false);
        
        // Start using animation
        teaserAnimator.SetBool(usingTrigger, true);
        
        isTakingOut = false;
        isUsing = true;
        isTakingBack = false;
        usingTimer = 0f; // Reset timer
        
        // Increase elephant happiness when using the teaser
        IncreaseElephantHappiness();
    }

    void StopUsingAnimation()
    {
        if (teaserAnimator == null) return;

        // Stop using animation
        teaserAnimator.SetBool(usingTrigger, false);
        
        isTakingOut = false;
        isUsing = false;
        isTakingBack = false;
    }

    void StartTakeBackAnimation()
    {
        if (teaserAnimator == null) return;

        // Reset all states
        teaserAnimator.SetBool(takeOutTrigger, false);
        teaserAnimator.SetBool(usingTrigger, false);
        
        // Start take back animation
        teaserAnimator.SetBool(takeBackTrigger, true);
        
        isTakingOut = false;
        isUsing = false;
        isTakingBack = true;
    }

    void ReturnToIdle()
    {
        if (teaserAnimator == null) return;

        // Reset all states
        teaserAnimator.SetBool(takeOutTrigger, false);
        teaserAnimator.SetBool(usingTrigger, false);
        teaserAnimator.SetBool(takeBackTrigger, false);
        
        isTakingOut = false;
        isUsing = false;
        isTakingBack = false;
        isTakenOut = false;
        
        // Deactivate the teaser after take back if configured
        if (deactivateAfterTakeBack)
        {
            DeactivateTeaser();
        }
    }

    // Public methods for external control
    public void ForceTakeOut()
    {
        if (!isActivated)
        {
            ActivateTeaser();
        }
        StartTakeOutAnimation();
    }

    public void ForceUsing()
    {
        if (!isActivated)
        {
            ActivateTeaser();
        }
        if (isTakenOut)
        {
            StartUsingAnimation();
        }
    }

    public void ForceTakeBack()
    {
        if (!isActivated)
        {
            ActivateTeaser();
        }
        StartTakeBackAnimation();
    }

    public void ForceIdle()
    {
        if (!isActivated)
        {
            ActivateTeaser();
        }
        ReturnToIdle();
    }
    
    // Activation control methods
    public void ActivateTeaser()
    {
        gameObject.SetActive(true);
        isActivated = true;
    }
    
    public void DeactivateTeaser()
    {
        gameObject.SetActive(false);
        isActivated = false;
    }

    // Animation event callbacks (can be called from animation events)
    public void OnTakeOutAnimationComplete()
    {
        isTakenOut = true;
        isTakingOut = false;
        
        // Reset animation states but keep isTakenOut = true
        if (teaserAnimator != null)
        {
            teaserAnimator.SetBool(takeOutTrigger, false);
            teaserAnimator.SetBool(usingTrigger, false);
            teaserAnimator.SetBool(takeBackTrigger, false);
        }
    }

    public void OnTakeBackAnimationComplete()
    {
        isTakenOut = false;
        isTakingBack = false;
        ReturnToIdle();
    }

    // Getter methods for external scripts to check current state
    public bool IsTakingOut => isTakingOut;
    public bool IsUsing => isUsing;
    public bool IsTakingBack => isTakingBack;
    public bool IsActivated => isActivated;
    public bool IsTakenOut => isTakenOut;
    
    /// <summary>
    /// Increase elephant happiness when using the teaser (only if player is close enough)
    /// </summary>
    private void IncreaseElephantHappiness()
    {
        // Check if player is close enough to the elephant
        if (!IsPlayerNearElephant())
        {
            Debug.Log($"ElephantTeaserAnimationManager: Player too far from elephant. Distance: {GetDistanceToElephant():F1}m, Max: {maxProximityDistance}m");
            return;
        }
        
        if (ElephantStateController.Instance != null)
        {
            // Use the new ModifyHappiness method to directly control the happiness increase
            ElephantStateController.Instance.ModifyHappiness(happinessIncrease, teaserEventName);
            
            Debug.Log($"ElephantTeaserAnimationManager: Increased elephant happiness by {happinessIncrease} using event: {teaserEventName}");
        }
        else
        {
            Debug.LogWarning("ElephantTeaserAnimationManager: ElephantStateController not found!");
        }
    }
    
    /// <summary>
    /// Check if the player is within the proximity range of the elephant
    /// </summary>
    /// <returns>True if player is close enough, false otherwise</returns>
    private bool IsPlayerNearElephant()
    {
        if (playerTransform == null)
        {
            Debug.LogWarning("ElephantTeaserAnimationManager: Player transform not found!");
            return false;
        }
        
        if (elephantObject == null)
        {
            Debug.LogWarning("ElephantTeaserAnimationManager: Elephant object not assigned!");
            return false;
        }
        
        float distance = Vector3.Distance(playerTransform.position, elephantObject.transform.position);
        return distance <= maxProximityDistance;
    }
    
    /// <summary>
    /// Get the current distance between player and elephant
    /// </summary>
    /// <returns>Distance in units, or -1 if either object is missing</returns>
    private float GetDistanceToElephant()
    {
        if (playerTransform == null || elephantObject == null)
        {
            return -1f;
        }
        
        return Vector3.Distance(playerTransform.position, elephantObject.transform.position);
    }
    
    /// <summary>
    /// Draw gizmos to visualize the proximity range in the Scene view
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (elephantObject != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(elephantObject.transform.position, maxProximityDistance);
        }
    }
}
