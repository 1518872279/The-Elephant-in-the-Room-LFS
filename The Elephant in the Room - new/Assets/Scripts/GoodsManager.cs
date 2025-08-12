using UnityEngine;
using System.Collections.Generic;
using System;

public class GoodsManager : MonoBehaviour
{
    public static GoodsManager Instance { get; private set; }

    [Header("Goods Database")]
    public List<Goods> allGoods = new List<Goods>();
    
    [Header("Delivery System")]
    [Tooltip("Transform that defines where delivered goods will be instantiated")]
    public Transform deliveryPosition;
    
    [Tooltip("Default prefab to use if goods doesn't have a specific delivery prefab")]
    public GameObject defaultGoodsPrefab;
    
    [Tooltip("List of pending deliveries")]
    public List<PendingDelivery> pendingDeliveries = new List<PendingDelivery>();
    
    // Events
    public event Action<Goods> OnGoodsPurchased;
    public event Action<Goods> OnGoodsDelivered;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    [System.Serializable]
    public class PendingDelivery
    {
        public Goods goods;
        public int deliveryDay;
        
        public PendingDelivery(Goods goods, int deliveryDay)
        {
            this.goods = goods;
            this.deliveryDay = deliveryDay;
        }
    }
    


    void Start()
    {
        // Initialize with some default goods if none exist
        if (allGoods.Count == 0)
        {
            InitializeDefaultGoods();
        }
        
        // Subscribe to day changes for delivery system
        if (DayPartManager.Instance != null)
        {
            DayPartManager.Instance.OnDayPartChanged += OnDayPartChanged;
        }
    }
    
    void OnDestroy()
    {
        if (DayPartManager.Instance != null)
            DayPartManager.Instance.OnDayPartChanged -= OnDayPartChanged;
    }
    
    /// <summary>
    /// Handle day part changes to process deliveries
    /// </summary>
    private void OnDayPartChanged(DayPartManager.DayPart newDayPart)
    {
        int currentDay = DayPartManager.Instance.daysElapsed;
        
        // Check for deliveries at the start of a new day (Morning)
        if (newDayPart == DayPartManager.DayPart.Morning)
        {
            ProcessDeliveries(currentDay);
        }
    }

    /// <summary>
    /// Initialize with some default goods for testing
    /// </summary>
    private void InitializeDefaultGoods()
    {
        AddGoods("Coffee", 
            "A warm cup of freshly brewed coffee. Perfect for starting your day with energy and focus.", 
            "Fresh brewed coffee to start your day.", 
            4);
            
        AddGoods("Sandwich", 
            "A delicious sandwich with fresh ingredients. Ham, cheese, lettuce, and tomato on whole grain bread.", 
            "Fresh sandwich with premium ingredients.", 
            9);
            
        AddGoods("Tea", 
            "A soothing cup of herbal tea. Made with natural herbs and perfect for relaxation.", 
            "Soothing herbal tea for relaxation.", 
            3);
            
        AddGoods("Cake", 
            "A decadent chocolate cake with rich frosting. Perfect for celebrations or as a sweet treat.", 
            "Decadent chocolate cake with rich frosting.", 
            13);
            
        AddGoods("Water", 
            "Pure spring water, bottled at the source. Essential for staying hydrated throughout the day.", 
            "Pure spring water for hydration.", 
            2);
            
        AddGoods("Companion Robot", 
            "A friendly companion robot that helps increase elephant stability but may affect happiness. Press 6 to spawn after purchase.", 
            "Companion robot for elephant stability.", 
            25);
    }

    /// <summary>
    /// Add a new goods item to the database
    /// </summary>
    public void AddGoods(string name, string info, string shortIntro, int price)
    {
        Goods newGoods = new Goods(name, info, shortIntro, price);
        allGoods.Add(newGoods);
        Debug.Log($"GoodsManager: Added goods '{name}' with price ${price}");
    }

    /// <summary>
    /// Add a new goods item with icon
    /// </summary>
    public void AddGoods(string name, string info, string shortIntro, int price, Sprite icon)
    {
        Goods newGoods = new Goods(name, info, shortIntro, price, icon);
        allGoods.Add(newGoods);
        Debug.Log($"GoodsManager: Added goods '{name}' with price ${price}");
    }
    
    /// <summary>
    /// Add a new goods item with delivery prefab
    /// </summary>
    public void AddGoods(string name, string info, string shortIntro, int price, GameObject deliveryPrefab)
    {
        Goods newGoods = new Goods(name, info, shortIntro, price, deliveryPrefab);
        allGoods.Add(newGoods);
        Debug.Log($"GoodsManager: Added goods '{name}' with price ${price} and delivery prefab '{deliveryPrefab.name}'");
    }

    /// <summary>
    /// Add a goods object directly
    /// </summary>
    public void AddGoods(Goods goods)
    {
        allGoods.Add(goods);
        Debug.Log($"GoodsManager: Added goods '{goods.goodsName}' with price ${goods.goodsPrice}");
    }
    
    /// <summary>
    /// Schedule a goods for delivery on the next day
    /// </summary>
    public void ScheduleDelivery(Goods goods)
    {
        if (goods == null)
        {
            Debug.LogError("GoodsManager: Cannot schedule delivery for null goods!");
            return;
        }
        
        int currentDay = DayPartManager.Instance.daysElapsed;
        int deliveryDay = currentDay + 1; // Deliver on next day
        
        PendingDelivery delivery = new PendingDelivery(goods, deliveryDay);
        pendingDeliveries.Add(delivery);
        
        Debug.Log($"GoodsManager: Scheduled delivery for '{goods.goodsName}' on day {deliveryDay}");
        
        // Fire purchase event
        OnGoodsPurchased?.Invoke(goods);
    }
    
    /// <summary>
    /// Process all pending deliveries for the current day
    /// </summary>
    private void ProcessDeliveries(int currentDay)
    {
        List<PendingDelivery> deliveriesToProcess = new List<PendingDelivery>();
        
        // Find all deliveries for today
        foreach (PendingDelivery delivery in pendingDeliveries)
        {
            if (delivery.deliveryDay == currentDay)
            {
                deliveriesToProcess.Add(delivery);
            }
        }
        
        // Process the deliveries
        foreach (PendingDelivery delivery in deliveriesToProcess)
        {
            DeliverGoods(delivery.goods);
            pendingDeliveries.Remove(delivery);
        }
        
        if (deliveriesToProcess.Count > 0)
        {
            Debug.Log($"GoodsManager: Processed {deliveriesToProcess.Count} deliveries on day {currentDay}");
        }
    }
    
    /// <summary>
    /// Instantiate the goods at the delivery position
    /// </summary>
    private void DeliverGoods(Goods goods)
    {
        // Find the appropriate prefab for this goods
        GameObject prefabToUse = GetPrefabForGoods(goods);
        
        if (prefabToUse == null)
        {
            Debug.LogError($"GoodsManager: No prefab found for goods '{goods.goodsName}' and no default prefab assigned!");
            return;
        }
        
        try
        {
            // Get the delivery position from the transform
            Vector3 deliveryPos = deliveryPosition != null ? deliveryPosition.position : Vector3.zero;
            
            // Instantiate the goods at the delivery position
            GameObject deliveredGoods = Instantiate(prefabToUse, deliveryPos, Quaternion.identity);
            
            // Set the goods data on the instantiated object
            GoodsItem goodsItem = deliveredGoods.GetComponent<GoodsItem>();
            if (goodsItem != null)
            {
                goodsItem.SetGoods(goods);
            }
            else
            {
                Debug.LogWarning($"GoodsManager: Instantiated prefab for '{goods.goodsName}' doesn't have a GoodsItem component!");
            }
            
            Debug.Log($"GoodsManager: Delivered '{goods.goodsName}' using prefab '{prefabToUse.name}' at position {deliveryPos}");
            
            // Fire delivery event
            OnGoodsDelivered?.Invoke(goods);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GoodsManager: Failed to deliver goods '{goods.goodsName}': {e.Message}");
        }
    }
    
    /// <summary>
    /// Get the appropriate prefab for a goods
    /// </summary>
    private GameObject GetPrefabForGoods(Goods goods)
    {
        // First, check if the goods has its own delivery prefab
        if (goods.deliveryPrefab != null)
        {
            return goods.deliveryPrefab;
        }
        
        // If no specific prefab found, use the default prefab
        if (defaultGoodsPrefab != null)
        {
            Debug.Log($"GoodsManager: Using default prefab for '{goods.goodsName}'");
            return defaultGoodsPrefab;
        }
        
        return null;
    }
    


    /// <summary>
    /// Remove a goods item by name
    /// </summary>
    public bool RemoveGoods(string goodsName)
    {
        Goods goodsToRemove = allGoods.Find(g => g.goodsName == goodsName);
        if (goodsToRemove != null)
        {
            allGoods.Remove(goodsToRemove);
            Debug.Log($"GoodsManager: Removed goods '{goodsName}'");
            return true;
        }
        Debug.LogWarning($"GoodsManager: Could not find goods '{goodsName}' to remove");
        return false;
    }

    /// <summary>
    /// Get a goods item by name
    /// </summary>
    public Goods GetGoods(string goodsName)
    {
        return allGoods.Find(g => g.goodsName == goodsName);
    }

    /// <summary>
    /// Get all available goods (where isAvailable is true)
    /// </summary>
    public List<Goods> GetAvailableGoods()
    {
        return allGoods.FindAll(g => g.isAvailable);
    }





    /// <summary>
    /// Set goods availability
    /// </summary>
    public void SetGoodsAvailability(string goodsName, bool isAvailable)
    {
        Goods goods = GetGoods(goodsName);
        if (goods != null)
        {
            goods.isAvailable = isAvailable;
            Debug.Log($"GoodsManager: Set '{goodsName}' availability to {isAvailable}");
        }
        else
        {
            Debug.LogWarning($"GoodsManager: Could not find goods '{goodsName}' to update availability");
        }
    }

    /// <summary>
    /// Update goods price
    /// </summary>
    public void UpdateGoodsPrice(string goodsName, int newPrice)
    {
        Goods goods = GetGoods(goodsName);
        if (goods != null)
        {
            int oldPrice = goods.goodsPrice;
            goods.goodsPrice = newPrice;
            Debug.Log($"GoodsManager: Updated '{goodsName}' price from ${oldPrice} to ${newPrice}");
        }
        else
        {
            Debug.LogWarning($"GoodsManager: Could not find goods '{goodsName}' to update price");
        }
    }

    /// <summary>
    /// Get total number of goods
    /// </summary>
    public int GetGoodsCount()
    {
        return allGoods.Count;
    }

    /// <summary>
    /// Clear all goods
    /// </summary>
    public void ClearAllGoods()
    {
        allGoods.Clear();
        Debug.Log("GoodsManager: Cleared all goods");
    }

    /// <summary>
    /// Test method to print all goods (for debugging)
    /// </summary>
    [ContextMenu("Print All Goods")]
    public void PrintAllGoods()
    {
        Debug.Log($"GoodsManager: Total goods count: {allGoods.Count}");
        for (int i = 0; i < allGoods.Count; i++)
        {
            var goods = allGoods[i];
            Debug.Log($"Goods {i}: {goods.goodsName} - ${goods.goodsPrice} - Available: {goods.isAvailable}");
        }
    }
    
    /// <summary>
    /// Test method to schedule a delivery (for testing)
    /// </summary>
    [ContextMenu("Schedule Test Delivery")]
    public void ScheduleTestDelivery()
    {
        if (allGoods.Count > 0)
        {
            Goods testGoods = allGoods[0];
            ScheduleDelivery(testGoods);
        }
        else
        {
            Debug.LogWarning("GoodsManager: No goods available for test delivery");
        }
    }
    
    /// <summary>
    /// Test method to print pending deliveries (for debugging)
    /// </summary>
    [ContextMenu("Print Pending Deliveries")]
    public void PrintPendingDeliveries()
    {
        Debug.Log($"GoodsManager: Total pending deliveries: {pendingDeliveries.Count}");
        for (int i = 0; i < pendingDeliveries.Count; i++)
        {
            var delivery = pendingDeliveries[i];
            Debug.Log($"Delivery {i}: {delivery.goods.goodsName} - Day {delivery.deliveryDay}");
        }
    }
    
    /// <summary>
    /// Test method to force process deliveries (for testing)
    /// </summary>
    [ContextMenu("Force Process Deliveries")]
    public void ForceProcessDeliveries()
    {
        int currentDay = DayPartManager.Instance.daysElapsed;
        ProcessDeliveries(currentDay);
    }
    
    /// <summary>
    /// Test method to print all goods prefab assignments (for debugging)
    /// </summary>
    [ContextMenu("Print Goods Prefab Assignments")]
    public void PrintGoodsPrefabAssignments()
    {
        Debug.Log($"GoodsManager: Total goods: {allGoods.Count}");
        for (int i = 0; i < allGoods.Count; i++)
        {
            var goods = allGoods[i];
            string prefabName = goods.deliveryPrefab != null ? goods.deliveryPrefab.name : "NULL";
            Debug.Log($"Goods {i}: '{goods.goodsName}' -> Prefab '{prefabName}'");
        }
        
        if (defaultGoodsPrefab != null)
        {
            Debug.Log($"Default prefab: '{defaultGoodsPrefab.name}'");
        }
        else
        {
            Debug.LogWarning("No default prefab assigned!");
        }
    }
    
    /// <summary>
    /// Test method to test prefab lookup for all goods (for debugging)
    /// </summary>
    [ContextMenu("Test Prefab Lookup")]
    public void TestPrefabLookup()
    {
        Debug.Log("GoodsManager: Testing prefab lookup for all goods...");
        foreach (Goods goods in allGoods)
        {
            GameObject prefab = GetPrefabForGoods(goods);
            if (prefab != null)
            {
                Debug.Log($"Goods '{goods.goodsName}' -> Prefab '{prefab.name}'");
            }
            else
            {
                Debug.LogWarning($"Goods '{goods.goodsName}' -> No prefab found!");
            }
        }
    }
} 