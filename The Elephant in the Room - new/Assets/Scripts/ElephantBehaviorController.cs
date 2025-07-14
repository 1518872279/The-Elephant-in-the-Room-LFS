using UnityEngine;

public class ElephantBehaviorController : MonoBehaviour
{
    [Tooltip("Animator with reaction triggers per event name")]
    public Animator animator;

    void OnEnable()
    {
        if (ElephantStateController.Instance != null)
        {
            ElephantStateController.Instance.OnReactionTriggered += HandleReaction;
        }
        else
        {
            Debug.LogWarning("ElephantStateController.Instance is null. Make sure ElephantStateController is in the scene.");
        }
    }

    void OnDisable()
    {
        if (ElephantStateController.Instance != null)
        {
            ElephantStateController.Instance.OnReactionTriggered -= HandleReaction;
        }
    }

    void HandleReaction(string eventName)
    {
        if (animator != null)
        {
            // Use eventName to select trigger, e.g., "BreakfastReaction" or "GarbageCleanupReaction"
            string trigger = eventName + "Reaction";
            animator.SetTrigger(trigger);
        }
        else
        {
            Debug.LogWarning("Animator is not assigned to ElephantBehaviorController.");
        }
    }
} 