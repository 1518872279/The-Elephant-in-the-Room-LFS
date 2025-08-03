using UnityEngine;

/// <summary>
/// Component that represents a physical goods item in the world
/// Attach this to prefabs that will be instantiated when goods are delivered
/// </summary>
public class GoodsItem : MonoBehaviour
{
    [Header("Goods Data")]
    public Goods goodsData;
    
    [Header("Visual Settings")]
    [Tooltip("Optional mesh renderer to change material based on goods")]
    public MeshRenderer meshRenderer;
    
    [Tooltip("Optional text component to display goods name")]
    public TMPro.TextMeshPro nameText;
    
    [Header("Interaction")]
    [Tooltip("Whether the player can interact with this item")]
    public bool isInteractable = true;
    
    // Events
    public System.Action<GoodsItem> OnItemInteracted;
    public System.Action<GoodsItem> OnItemCollected;

    void Start()
    {
        // Update visual representation if goods data is set
        if (goodsData != null)
        {
            UpdateVisuals();
        }
    }

    /// <summary>
    /// Set the goods data for this item
    /// </summary>
    public void SetGoods(Goods goods)
    {
        goodsData = goods;
        UpdateVisuals();
        
        Debug.Log($"GoodsItem: Set goods data for '{goods.goodsName}'");
    }

    /// <summary>
    /// Update the visual representation of the goods
    /// </summary>
    private void UpdateVisuals()
    {
        if (goodsData == null) return;
        
        // Update name text if available
        if (nameText != null)
        {
            nameText.text = goodsData.goodsName;
        }
        
        // You can add more visual customization here, such as:
        // - Changing materials based on goods type
        // - Scaling based on goods size
        // - Adding particle effects
        // - Playing delivery sound
    }

    /// <summary>
    /// Handle player interaction with the item
    /// </summary>
    public void Interact()
    {
        if (!isInteractable || goodsData == null) return;
        
        Debug.Log($"GoodsItem: Player interacted with '{goodsData.goodsName}'");
        OnItemInteracted?.Invoke(this);
    }

    /// <summary>
    /// Collect the item (remove from world)
    /// </summary>
    public void Collect()
    {
        if (goodsData == null) return;
        
        Debug.Log($"GoodsItem: Collected '{goodsData.goodsName}'");
        OnItemCollected?.Invoke(this);
        
        // Destroy the GameObject
        Destroy(gameObject);
    }

    /// <summary>
    /// Get the goods data
    /// </summary>
    public Goods GetGoodsData()
    {
        return goodsData;
    }

    /// <summary>
    /// Check if this item can be interacted with
    /// </summary>
    public bool CanInteract()
    {
        return isInteractable && goodsData != null;
    }

    /// <summary>
    /// Test method to simulate interaction (for testing)
    /// </summary>
    [ContextMenu("Test Interact")]
    public void TestInteract()
    {
        Interact();
    }

    /// <summary>
    /// Test method to collect the item (for testing)
    /// </summary>
    [ContextMenu("Test Collect")]
    public void TestCollect()
    {
        Collect();
    }
} 