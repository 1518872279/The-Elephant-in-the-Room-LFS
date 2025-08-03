using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoodsPreview : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI goodsNameText;
    public TextMeshProUGUI goodsShortIntroText;
    public TextMeshProUGUI goodsPriceText;
    public Image goodsIconImage;
    public Button selectButton;
    
    [Header("Configuration")]
    [Tooltip("The goods to display. Leave empty to set via code.")]
    public string goodsName = "";
    
    [Header("Detail View Integration")]
    [Tooltip("Reference to GoodsDetailView for showing full details")]
    public GoodsDetailView detailView;
    
    private Goods currentGoods;
    private GoodsManager goodsManager;
    
    // Event for when goods is selected
    public System.Action<Goods> OnGoodsSelected;

    void Start()
    {
        // Get reference to GoodsManager
        goodsManager = GoodsManager.Instance;
        
        if (goodsManager == null)
        {
            Debug.LogError("GoodsPreview: GoodsManager not found!");
            return;
        }

        // Check UI components
        if (goodsNameText == null)
            Debug.LogWarning("GoodsPreview: GoodsNameText UI component is not assigned!");
        if (goodsShortIntroText == null)
            Debug.LogWarning("GoodsPreview: GoodsShortIntroText UI component is not assigned!");
        if (goodsPriceText == null)
            Debug.LogWarning("GoodsPreview: GoodsPriceText UI component is not assigned!");
        if (selectButton == null)
            Debug.LogWarning("GoodsPreview: SelectButton UI component is not assigned!");

        // Setup button click handler
        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnSelectButtonClicked);
        }

        // If goods name is set in inspector, load it
        if (!string.IsNullOrEmpty(goodsName))
        {
            SetGoods(goodsName);
        }
    }

    /// <summary>
    /// Set the goods to display by name
    /// </summary>
    public void SetGoods(string goodsName)
    {
        if (goodsManager == null)
        {
            Debug.LogError("GoodsPreview: GoodsManager not found!");
            return;
        }

        Goods goods = goodsManager.GetGoods(goodsName);
        if (goods != null)
        {
            SetGoods(goods);
        }
        else
        {
            Debug.LogWarning($"GoodsPreview: Could not find goods '{goodsName}'");
            ClearDisplay();
        }
    }

    /// <summary>
    /// Set the goods to display by Goods object
    /// </summary>
    public void SetGoods(Goods goods)
    {
        currentGoods = goods;
        UpdateDisplay();
    }

    /// <summary>
    /// Update the UI display with current goods
    /// </summary>
    private void UpdateDisplay()
    {
        if (currentGoods == null)
        {
            ClearDisplay();
            return;
        }

        // Update goods name
        if (goodsNameText != null)
        {
            goodsNameText.text = currentGoods.goodsName;
        }

        // Update short introduction
        if (goodsShortIntroText != null)
        {
            goodsShortIntroText.text = currentGoods.goodsShortIntro;
        }

        // Update price
        if (goodsPriceText != null)
        {
            goodsPriceText.text = $"${currentGoods.goodsPrice:F0}";
        }

        // Update icon if available
        if (goodsIconImage != null && currentGoods.goodsIcon != null)
        {
            goodsIconImage.sprite = currentGoods.goodsIcon;
            goodsIconImage.gameObject.SetActive(true);
        }
        else if (goodsIconImage != null)
        {
            goodsIconImage.gameObject.SetActive(false);
        }

        // Update button availability
        if (selectButton != null)
        {
            selectButton.interactable = currentGoods.isAvailable;
            
            // Visual feedback for unavailable goods
            if (!currentGoods.isAvailable)
            {
                // You can add visual effects here, such as:
                // - Changing button color to gray
                // - Adding "SOLD OUT" text
                // - Disabling the button
                Debug.Log($"GoodsPreview: '{currentGoods.goodsName}' is now unavailable");
            }
        }

        Debug.Log($"GoodsPreview: Updated display for '{currentGoods.goodsName}' - Price: ${currentGoods.goodsPrice}, Available: {currentGoods.isAvailable}");
    }

    /// <summary>
    /// Clear the display
    /// </summary>
    private void ClearDisplay()
    {
        if (goodsNameText != null)
            goodsNameText.text = "";
        if (goodsShortIntroText != null)
            goodsShortIntroText.text = "";
        if (goodsPriceText != null)
            goodsPriceText.text = "";
        if (goodsIconImage != null)
            goodsIconImage.gameObject.SetActive(false);
        if (selectButton != null)
            selectButton.interactable = false;
    }

    /// <summary>
    /// Handle select button click
    /// </summary>
    private void OnSelectButtonClicked()
    {
        if (currentGoods != null && currentGoods.isAvailable)
        {
            Debug.Log($"GoodsPreview: Selected goods '{currentGoods.goodsName}'");
            
            // Show detail view if available
            if (detailView != null)
            {
                detailView.ShowGoodsDetail(currentGoods);
            }
            
            OnGoodsSelected?.Invoke(currentGoods);
        }
        else
        {
            Debug.LogWarning("GoodsPreview: Cannot select unavailable goods");
        }
    }

    /// <summary>
    /// Get the currently displayed goods
    /// </summary>
    public Goods GetCurrentGoods()
    {
        return currentGoods;
    }

    /// <summary>
    /// Check if goods is available
    /// </summary>
    public bool IsGoodsAvailable()
    {
        return currentGoods != null && currentGoods.isAvailable;
    }

    /// <summary>
    /// Manually refresh the display (for testing)
    /// </summary>
    [ContextMenu("Refresh Display")]
    public void RefreshDisplay()
    {
        UpdateDisplay();
    }
    
    /// <summary>
    /// Refresh the display and update availability status
    /// </summary>
    public void RefreshGoodsAvailability()
    {
        if (currentGoods != null && goodsManager != null)
        {
            // Get the updated goods data from the manager
            Goods updatedGoods = goodsManager.GetGoods(currentGoods.goodsName);
            if (updatedGoods != null)
            {
                currentGoods = updatedGoods;
                UpdateDisplay();
                Debug.Log($"GoodsPreview: Refreshed availability for '{currentGoods.goodsName}' - Available: {currentGoods.isAvailable}");
            }
        }
    }

    /// <summary>
    /// Test method to set a random goods (for testing)
    /// </summary>
    [ContextMenu("Set Random Goods")]
    public void SetRandomGoods()
    {
        if (goodsManager == null)
        {
            Debug.LogError("GoodsPreview: GoodsManager not found!");
            return;
        }

        var availableGoods = goodsManager.GetAvailableGoods();
        if (availableGoods.Count > 0)
        {
            int randomIndex = Random.Range(0, availableGoods.Count);
            SetGoods(availableGoods[randomIndex]);
        }
        else
        {
            Debug.LogWarning("GoodsPreview: No available goods to set");
        }
    }

    /// <summary>
    /// Test method to cycle through all goods (for testing)
    /// </summary>
    [ContextMenu("Cycle Next Goods")]
    public void CycleNextGoods()
    {
        if (goodsManager == null)
        {
            Debug.LogError("GoodsPreview: GoodsManager not found!");
            return;
        }

        var allGoods = goodsManager.allGoods;
        if (allGoods.Count == 0)
        {
            Debug.LogWarning("GoodsPreview: No goods available to cycle");
            return;
        }

        if (currentGoods == null)
        {
            SetGoods(allGoods[0]);
        }
        else
        {
            int currentIndex = allGoods.IndexOf(currentGoods);
            int nextIndex = (currentIndex + 1) % allGoods.Count;
            SetGoods(allGoods[nextIndex]);
        }
    }
}
