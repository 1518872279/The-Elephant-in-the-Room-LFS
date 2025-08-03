using UnityEngine;
using System;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [Header("Money Configuration")]
    [Tooltip("Starting money for the player")]
    public float startingMoney = 100f;
    
    [Tooltip("Daily income amount")]
    public float dailyIncome = 50f;
    
    [Tooltip("Current player money")]
    public float currentMoney = 100f;
    
    [Header("Day Tracking")]
    [Tooltip("Last day when money was given")]
    public int lastMoneyDay = 0;
    
    // Events
    public event Action<float> OnMoneyChanged;
    public event Action<float> OnMoneyEarned;
    public event Action<float> OnMoneySpent;
    public event Action OnInsufficientFunds;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Initialize money
        currentMoney = startingMoney;
        lastMoneyDay = DayPartManager.Instance.daysElapsed;
        
        // Subscribe to day changes
        if (DayPartManager.Instance != null)
        {
            DayPartManager.Instance.OnDayPartChanged += OnDayPartChanged;
        }
        
        Debug.Log($"MoneyManager: Initialized with ${currentMoney}");
    }

    void OnDestroy()
    {
        if (DayPartManager.Instance != null)
            DayPartManager.Instance.OnDayPartChanged -= OnDayPartChanged;
    }

    /// <summary>
    /// Handle day part changes to give daily income
    /// </summary>
    private void OnDayPartChanged(DayPartManager.DayPart newDayPart)
    {
        int currentDay = DayPartManager.Instance.daysElapsed;
        
        // Give daily income if it's a new day
        if (currentDay > lastMoneyDay)
        {
            GiveDailyIncome();
            lastMoneyDay = currentDay;
        }
    }

    /// <summary>
    /// Give daily income to the player
    /// </summary>
    public void GiveDailyIncome()
    {
        AddMoney(dailyIncome);
        Debug.Log($"MoneyManager: Gave daily income of ${dailyIncome}");
    }

    /// <summary>
    /// Add money to player's balance
    /// </summary>
    public void AddMoney(float amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("MoneyManager: Cannot add negative or zero money!");
            return;
        }

        float oldMoney = currentMoney;
        currentMoney += amount;
        
        OnMoneyChanged?.Invoke(currentMoney);
        OnMoneyEarned?.Invoke(amount);
        
        Debug.Log($"MoneyManager: Added ${amount}. Balance: ${oldMoney} → ${currentMoney}");
    }

    /// <summary>
    /// Spend money from player's balance
    /// </summary>
    public bool SpendMoney(float amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("MoneyManager: Cannot spend negative or zero money!");
            return false;
        }

        if (currentMoney < amount)
        {
            Debug.LogWarning($"MoneyManager: Insufficient funds! Need ${amount}, have ${currentMoney}");
            OnInsufficientFunds?.Invoke();
            return false;
        }

        float oldMoney = currentMoney;
        currentMoney -= amount;
        
        OnMoneyChanged?.Invoke(currentMoney);
        OnMoneySpent?.Invoke(amount);
        
        Debug.Log($"MoneyManager: Spent ${amount}. Balance: ${oldMoney} → ${currentMoney}");
        return true;
    }

    /// <summary>
    /// Check if player can afford a purchase
    /// </summary>
    public bool CanAfford(float amount)
    {
        return currentMoney >= amount;
    }

    /// <summary>
    /// Get current money balance
    /// </summary>
    public float GetCurrentMoney()
    {
        return currentMoney;
    }

    /// <summary>
    /// Set money balance (for testing or save/load)
    /// </summary>
    public void SetMoney(float amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("MoneyManager: Cannot set negative money!");
            return;
        }

        float oldMoney = currentMoney;
        currentMoney = amount;
        
        OnMoneyChanged?.Invoke(currentMoney);
        
        Debug.Log($"MoneyManager: Set money to ${amount}. Previous: ${oldMoney}");
    }

    /// <summary>
    /// Reset money to starting amount
    /// </summary>
    public void ResetMoney()
    {
        SetMoney(startingMoney);
        Debug.Log($"MoneyManager: Reset money to starting amount ${startingMoney}");
    }

    /// <summary>
    /// Test method to add money (for testing)
    /// </summary>
    [ContextMenu("Add $100")]
    public void AddTestMoney()
    {
        AddMoney(100f);
    }

    /// <summary>
    /// Test method to spend money (for testing)
    /// </summary>
    [ContextMenu("Spend $50")]
    public void SpendTestMoney()
    {
        SpendMoney(50f);
    }

    /// <summary>
    /// Test method to give daily income (for testing)
    /// </summary>
    [ContextMenu("Give Daily Income")]
    public void TestGiveDailyIncome()
    {
        GiveDailyIncome();
    }
} 