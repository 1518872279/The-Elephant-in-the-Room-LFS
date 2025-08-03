using UnityEngine;

[System.Serializable]
public class Goods
{
    [Header("Goods Information")]
    public string goodsName;
    [TextArea(3, 5)]
    public string goodsInformation;
    [TextArea(2, 3)]
    public string goodsShortIntro;  // Short version for preview
    public int goodsPrice;
    
    [Header("Optional Details")]
    public Sprite goodsIcon;
    public bool isAvailable = true;
    
    [Header("Delivery Prefab")]
    [Tooltip("Prefab to instantiate when this goods is delivered")]
    public GameObject deliveryPrefab;
    
    /// <summary>
    /// Creates a new goods item
    /// </summary>
    public Goods(string name, string info, string shortIntro, int price)
    {
        goodsName = name;
        goodsInformation = info;
        goodsShortIntro = shortIntro;
        goodsPrice = price;
        isAvailable = true;
    }
    
    /// <summary>
    /// Creates a new goods item with all parameters
    /// </summary>
    public Goods(string name, string info, string shortIntro, int price, Sprite icon)
    {
        goodsName = name;
        goodsInformation = info;
        goodsShortIntro = shortIntro;
        goodsPrice = price;
        goodsIcon = icon;
        isAvailable = true;
    }
    
    /// <summary>
    /// Creates a new goods item with delivery prefab
    /// </summary>
    public Goods(string name, string info, string shortIntro, int price, GameObject prefab)
    {
        goodsName = name;
        goodsInformation = info;
        goodsShortIntro = shortIntro;
        goodsPrice = price;
        deliveryPrefab = prefab;
        isAvailable = true;
    }
} 