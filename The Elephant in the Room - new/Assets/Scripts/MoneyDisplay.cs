using UnityEngine;
using TMPro;

public class MoneyDisplay : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI moneyText;
    public string moneyFormat = "Money: ${0}";
    
    private MoneyManager moneyManager;

    void Start()
    {
        // Get reference to MoneyManager
        moneyManager = MoneyManager.Instance;
        
        if (moneyManager == null)
        {
            Debug.LogError("MoneyDisplay: MoneyManager not found!");
            return;
        }

        // Check UI component
        if (moneyText == null)
        {
            Debug.LogWarning("MoneyDisplay: MoneyText UI component is not assigned!");
            return;
        }

        // Subscribe to money changes
        moneyManager.OnMoneyChanged += OnMoneyChanged;
        
        // Initial update
        UpdateMoneyDisplay();
    }

    void OnDestroy()
    {
        if (moneyManager != null)
            moneyManager.OnMoneyChanged -= OnMoneyChanged;
    }

    /// <summary>
    /// Handle money changes
    /// </summary>
    private void OnMoneyChanged(int newAmount)
    {
        UpdateMoneyDisplay();
    }

    /// <summary>
    /// Update the money display
    /// </summary>
    private void UpdateMoneyDisplay()
    {
        if (moneyText != null && moneyManager != null)
        {
            moneyText.text = string.Format(moneyFormat, moneyManager.GetCurrentMoney());
        }
    }

    /// <summary>
    /// Manually refresh the display (for testing)
    /// </summary>
    [ContextMenu("Refresh Money Display")]
    public void RefreshDisplay()
    {
        UpdateMoneyDisplay();
    }
} 