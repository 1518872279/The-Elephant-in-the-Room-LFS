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
        
        if (state == null)
        {
            Debug.LogError("ElephantStateController not found in scene!");
            return;
        }

        // Initialize sliders
        if (happinessBar != null)
        {
            happinessBar.minValue = 0;
            happinessBar.maxValue = 100;
        }
        
        if (stabilityBar != null)
        {
            stabilityBar.minValue = 0;
            stabilityBar.maxValue = 100;
        }

        UpdateBars();
        
        // Subscribe to changes
        state.OnReactionTriggered += _ => UpdateBars();
    }

    void UpdateBars()
    {
        if (state == null) return;
        
        if (happinessBar != null)
            happinessBar.value = state.happiness;
            
        if (stabilityBar != null)
            stabilityBar.value = state.stability;
    }

    void OnDestroy()
    {
        if (state != null)
            state.OnReactionTriggered -= _ => UpdateBars();
    }

    // Optional: Update bars every frame for real-time display
    void Update()
    {
        if (state != null)
            UpdateBars();
    }
} 