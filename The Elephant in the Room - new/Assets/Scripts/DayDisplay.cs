using UnityEngine;
using TMPro;

public class DayDisplay : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dayText;
    
    [Header("Display Settings")]
    [Tooltip("Text format for day display")]
    public string dayFormat = "Day {0}";
    
    [Header("Styling")]
    [Tooltip("Color for the day text")]
    public Color dayTextColor = Color.white;
    [Tooltip("Font size for the day text")]
    public int fontSize = 24;
    [Tooltip("Whether to use bold font weight")]
    public bool useBold = true;
    
    private TimeManager timeManager;
    
    void Start()
    {
        timeManager = TimeManager.Instance;
        if (timeManager != null)
        {
            // Subscribe to day changes
            timeManager.OnDayChanged += OnDayChanged;
            
            // Set initial styling
            ApplyStyling();
            
            // Set initial day display
            OnDayChanged(timeManager.GetCurrentDay());
        }
        else
        {
            Debug.LogError("DayDisplay: TimeManager not found!");
        }
    }
    
    void OnDestroy()
    {
        if (timeManager != null)
            timeManager.OnDayChanged -= OnDayChanged;
    }
    
    private void OnDayChanged(int newDay)
    {
        UpdateDayDisplay(newDay);
    }
    
    private void UpdateDayDisplay(int day)
    {
        if (dayText == null) return;
        
        dayText.text = string.Format(dayFormat, day);
    }
    
    private void ApplyStyling()
    {
        if (dayText != null)
        {
            dayText.color = dayTextColor;
            dayText.fontSize = fontSize;
            
            if (useBold)
            {
                dayText.fontStyle = FontStyles.Bold;
            }
        }
    }
    
    /// <summary>Manually update the day display</summary>
    public void RefreshDisplay()
    {
        if (timeManager != null)
        {
            OnDayChanged(timeManager.GetCurrentDay());
        }
    }
} 