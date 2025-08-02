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

    private bool isTakingOut = false;
    private bool isUsing = false;
    private bool isTakingBack = false;
    private bool isActivated = false;
    private bool isTakenOut = false;
    private float usingTimer = 0f;

    void Start()
    {
        // Get the animator component if not assigned
        if (teaserAnimator == null)
        {
            teaserAnimator = GetComponent<Animator>();
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
}
