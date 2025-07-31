using UnityEngine;

/// <summary>
/// Example integration script showing how to connect GoodsPreview with GoodsDetailView
/// </summary>
public class GoodsSystemIntegration : MonoBehaviour
{
    [Header("System References")]
    public GoodsPreview goodsPreview;
    public GoodsDetailView goodsDetailView;
    
    [Header("Purchase Handling")]
    public bool enablePurchaseLogging = true;

    void Start()
    {
        // Subscribe to goods selection events
        if (goodsPreview != null)
        {
            goodsPreview.OnGoodsSelected += OnGoodsSelected;
        }
        
        // Subscribe to purchase events
        if (goodsDetailView != null)
        {
            goodsDetailView.OnPurchaseConfirmed += OnPurchaseConfirmed;
            goodsDetailView.OnPurchaseCancelled += OnPurchaseCancelled;
            goodsDetailView.OnDetailViewClosed += OnDetailViewClosed;
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (goodsPreview != null)
        {
            goodsPreview.OnGoodsSelected -= OnGoodsSelected;
        }
        
        if (goodsDetailView != null)
        {
            goodsDetailView.OnPurchaseConfirmed -= OnPurchaseConfirmed;
            goodsDetailView.OnPurchaseCancelled -= OnPurchaseCancelled;
            goodsDetailView.OnDetailViewClosed -= OnDetailViewClosed;
        }
    }

    /// <summary>
    /// Handle goods selection from preview
    /// </summary>
    private void OnGoodsSelected(Goods selectedGoods)
    {
        if (enablePurchaseLogging)
        {
            Debug.Log($"GoodsSystemIntegration: Goods selected - {selectedGoods.goodsName}");
        }
        
        // You can add additional logic here, such as:
        // - Playing selection sound
        // - Updating UI state
        // - Triggering animations
    }

    /// <summary>
    /// Handle purchase confirmation
    /// </summary>
    private void OnPurchaseConfirmed(Goods purchasedGoods)
    {
        if (enablePurchaseLogging)
        {
            Debug.Log($"GoodsSystemIntegration: Purchase confirmed - {purchasedGoods.goodsName} for ${purchasedGoods.goodsPrice:F2}");
        }
        
        // Add your purchase logic here, such as:
        // - Deducting money from player
        // - Adding item to inventory
        // - Playing purchase sound/effect
        // - Updating UI
        // - Saving game state
    }

    /// <summary>
    /// Handle purchase cancellation
    /// </summary>
    private void OnPurchaseCancelled(Goods cancelledGoods)
    {
        if (enablePurchaseLogging)
        {
            Debug.Log($"GoodsSystemIntegration: Purchase cancelled - {cancelledGoods?.goodsName}");
        }
        
        // Add your cancellation logic here, such as:
        // - Playing cancellation sound
        // - Showing cancellation message
    }

    /// <summary>
    /// Handle detail view closing
    /// </summary>
    private void OnDetailViewClosed()
    {
        if (enablePurchaseLogging)
        {
            Debug.Log("GoodsSystemIntegration: Detail view closed");
        }
        
        // Add your close logic here, such as:
        // - Resetting UI state
        // - Playing close animation
        // - Updating other UI elements
    }

    /// <summary>
    /// Test method to show a specific goods detail
    /// </summary>
    [ContextMenu("Show Coffee Detail")]
    public void ShowCoffeeDetail()
    {
        if (goodsDetailView != null)
        {
            goodsDetailView.ShowGoodsDetail("Coffee");
        }
        else
        {
            Debug.LogWarning("GoodsSystemIntegration: GoodsDetailView not assigned!");
        }
    }

    /// <summary>
    /// Test method to show a random goods detail
    /// </summary>
    [ContextMenu("Show Random Detail")]
    public void ShowRandomDetail()
    {
        if (goodsDetailView != null)
        {
            goodsDetailView.ShowRandomGoodsDetail();
        }
        else
        {
            Debug.LogWarning("GoodsSystemIntegration: GoodsDetailView not assigned!");
        }
    }
} 