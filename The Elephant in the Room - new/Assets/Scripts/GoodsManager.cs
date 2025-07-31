using UnityEngine;
using System.Collections.Generic;

public class GoodsManager : MonoBehaviour
{
    public static GoodsManager Instance { get; private set; }

    [Header("Goods Database")]
    public List<Goods> allGoods = new List<Goods>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Initialize with some default goods if none exist
        if (allGoods.Count == 0)
        {
            InitializeDefaultGoods();
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
            3.50f);
            
        AddGoods("Sandwich", 
            "A delicious sandwich with fresh ingredients. Ham, cheese, lettuce, and tomato on whole grain bread.", 
            "Fresh sandwich with premium ingredients.", 
            8.99f);
            
        AddGoods("Tea", 
            "A soothing cup of herbal tea. Made with natural herbs and perfect for relaxation.", 
            "Soothing herbal tea for relaxation.", 
            2.99f);
            
        AddGoods("Cake", 
            "A decadent chocolate cake with rich frosting. Perfect for celebrations or as a sweet treat.", 
            "Decadent chocolate cake with rich frosting.", 
            12.99f);
            
        AddGoods("Water", 
            "Pure spring water, bottled at the source. Essential for staying hydrated throughout the day.", 
            "Pure spring water for hydration.", 
            1.99f);
    }

    /// <summary>
    /// Add a new goods item to the database
    /// </summary>
    public void AddGoods(string name, string info, string shortIntro, float price)
    {
        Goods newGoods = new Goods(name, info, shortIntro, price);
        allGoods.Add(newGoods);
        Debug.Log($"GoodsManager: Added goods '{name}' with price ${price}");
    }

    /// <summary>
    /// Add a new goods item with icon
    /// </summary>
    public void AddGoods(string name, string info, string shortIntro, float price, Sprite icon)
    {
        Goods newGoods = new Goods(name, info, shortIntro, price, icon);
        allGoods.Add(newGoods);
        Debug.Log($"GoodsManager: Added goods '{name}' with price ${price}");
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
    public void UpdateGoodsPrice(string goodsName, float newPrice)
    {
        Goods goods = GetGoods(goodsName);
        if (goods != null)
        {
            float oldPrice = goods.goodsPrice;
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
} 