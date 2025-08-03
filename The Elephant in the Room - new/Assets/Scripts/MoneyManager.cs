using UnityEngine;
using System;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [Header("Money Configuration")]
    [Tooltip("Starting money for the player")]
    public int startingMoney = 100;
    
    [Tooltip("Daily income amount")]
    public int dailyIncome = 50;
    
    [Tooltip("Current player money")]
    public int currentMoney = 100;
    
    [Header("Day Tracking")]
    [Tooltip("Last day when money was given")]
    public int lastMoneyDay = 0;
    
    // Events
    public event Action<int> OnMoneyChanged;
    public event Action<int> OnMoneyEarned;
    public event Action<int> OnMoneySpent;
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
    public void AddMoney(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("MoneyManager: Cannot add negative or zero money!");
            return;
        }

        int oldMoney = currentMoney;
        currentMoney += amount;
        
        OnMoneyChanged?.Invoke(currentMoney);
        OnMoneyEarned?.Invoke(amount);
        
        Debug.Log($"MoneyManager: Added ${amount}. Balance: ${oldMoney} → ${currentMoney}");
    }

    /// <summary>
    /// Spend money from player's balance
    /// </summary>
    public bool SpendMoney(int amount)
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

        int oldMoney = currentMoney;
        currentMoney -= amount;
        
        OnMoneyChanged?.Invoke(currentMoney);
        OnMoneySpent?.Invoke(amount);
        
        Debug.Log($"MoneyManager: Spent ${amount}. Balance: ${oldMoney} → ${currentMoney}");
        return true;
    }

    /// <summary>
    /// Check if player can afford a purchase
    /// </summary>
    public bool CanAfford(int amount)
    {
        return currentMoney >= amount;
    }

    /// <summary>
    /// Get current money balance
    /// </summary>
    public int GetCurrentMoney()
    {
        return currentMoney;
    }

    /// <summary>
    /// Set money balance (for testing or save/load)
    /// </summary>
    public void SetMoney(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("MoneyManager: Cannot set negative money!");
            return;
        }

        int oldMoney = currentMoney;
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
        AddMoney(100);
    }

    /// <summary>
    /// Test method to spend money (for testing)
    /// </summary>
    [ContextMenu("Spend $50")]
    public void SpendTestMoney()
    {
        SpendMoney(50);
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