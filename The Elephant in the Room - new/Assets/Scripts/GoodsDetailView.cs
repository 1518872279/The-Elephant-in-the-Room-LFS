using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoodsDetailView : MonoBehaviour
{
    [Header("UI References")]
    public GameObject detailPanel;              // The main panel for the detail view
    public TextMeshProUGUI goodsNameText;      // Full goods name
    public TextMeshProUGUI goodsInfoText;      // Full goods information
    public TextMeshProUGUI goodsPriceText;     // Price display
    public Image goodsIconImage;                // Goods icon
    public Button confirmPurchaseButton;        // Confirm purchase button
    public Button cancelPurchaseButton;         // Cancel purchase button
    
    [Header("Purchase Confirmation")]
    public GameObject confirmationPanel;        // Optional confirmation panel
    public TextMeshProUGUI confirmationText;   // Confirmation message
    public Button confirmYesButton;            // Yes button for confirmation
    public Button confirmNoButton;             // No button for confirmation
    
    [Header("Money Integration")]
    [Tooltip("Reference to MoneyManager for purchase validation")]
    public MoneyManager moneyManager;
    
    private Goods currentGoods;
    private GoodsManager goodsManager;
    
    // Events
    public System.Action<Goods> OnPurchaseConfirmed;
    public System.Action<Goods> OnPurchaseCancelled;
    public System.Action OnDetailViewClosed;

    void Start()
    {
        // Get reference to GoodsManager
        goodsManager = GoodsManager.Instance;
        
        if (goodsManager == null)
        {
            Debug.LogError("GoodsDetailView: GoodsManager not found!");
            return;
        }
        
        // Get reference to MoneyManager
        if (moneyManager == null)
        {
            moneyManager = MoneyManager.Instance;
        }
        
        if (moneyManager == null)
        {
            Debug.LogWarning("GoodsDetailView: MoneyManager not found! Purchase validation will be disabled.");
        }

        // Check UI components
        if (detailPanel == null)
            Debug.LogWarning("GoodsDetailView: DetailPanel is not assigned!");
        if (goodsNameText == null)
            Debug.LogWarning("GoodsDetailView: GoodsNameText is not assigned!");
        if (goodsInfoText == null)
            Debug.LogWarning("GoodsDetailView: GoodsInfoText is not assigned!");
        if (goodsPriceText == null)
            Debug.LogWarning("GoodsDetailView: GoodsPriceText is not assigned!");
        if (confirmPurchaseButton == null)
            Debug.LogWarning("GoodsDetailView: ConfirmPurchaseButton is not assigned!");
        if (cancelPurchaseButton == null)
            Debug.LogWarning("GoodsDetailView: CancelPurchaseButton is not assigned!");

        // Setup button listeners
        SetupButtonListeners();
        
        // Hide the panel initially
        if (detailPanel != null)
            detailPanel.SetActive(false);
            
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
    }

    /// <summary>
    /// Setup button click listeners
    /// </summary>
    private void SetupButtonListeners()
    {
        if (confirmPurchaseButton != null)
        {
            confirmPurchaseButton.onClick.RemoveAllListeners();
            confirmPurchaseButton.onClick.AddListener(OnConfirmPurchaseClicked);
        }
        
        if (cancelPurchaseButton != null)
        {
            cancelPurchaseButton.onClick.RemoveAllListeners();
            cancelPurchaseButton.onClick.AddListener(OnCancelPurchaseClicked);
        }
        
        if (confirmYesButton != null)
        {
            confirmYesButton.onClick.RemoveAllListeners();
            confirmYesButton.onClick.AddListener(OnConfirmYesClicked);
        }
        
        if (confirmNoButton != null)
        {
            confirmNoButton.onClick.RemoveAllListeners();
            confirmNoButton.onClick.AddListener(OnConfirmNoClicked);
        }
    }

    /// <summary>
    /// Show the detail view for a specific goods
    /// </summary>
    public void ShowGoodsDetail(Goods goods)
    {
        if (goods == null)
        {
            Debug.LogError("GoodsDetailView: Cannot show detail for null goods!");
            return;
        }

        currentGoods = goods;
        UpdateDisplay();
        
        if (detailPanel != null)
        {
            detailPanel.SetActive(true);
            Debug.Log($"GoodsDetailView: Showing detail for '{goods.goodsName}'");
            
            // Ensure buttons are visible when panel opens
            if (confirmPurchaseButton != null)
            {
                confirmPurchaseButton.gameObject.SetActive(true);
                Debug.Log("GoodsDetailView: Confirm purchase button activated on panel open");
            }
            
            if (cancelPurchaseButton != null)
            {
                cancelPurchaseButton.gameObject.SetActive(true);
                Debug.Log("GoodsDetailView: Cancel purchase button activated on panel open");
            }
        }
    }

    /// <summary>
    /// Show the detail view by goods name
    /// </summary>
    public void ShowGoodsDetail(string goodsName)
    {
        if (goodsManager == null)
        {
            Debug.LogError("GoodsDetailView: GoodsManager not found!");
            return;
        }

        Goods goods = goodsManager.GetGoods(goodsName);
        if (goods != null)
        {
            ShowGoodsDetail(goods);
        }
        else
        {
            Debug.LogWarning($"GoodsDetailView: Could not find goods '{goodsName}'");
        }
    }

    /// <summary>
    /// Update the display with current goods information
    /// </summary>
    private void UpdateDisplay()
    {
        if (currentGoods == null)
        {
            Debug.LogError("GoodsDetailView: No goods to display!");
            return;
        }

        // Update goods name
        if (goodsNameText != null)
        {
            goodsNameText.text = currentGoods.goodsName;
        }

        // Update full information
        if (goodsInfoText != null)
        {
            goodsInfoText.text = currentGoods.goodsInformation;
        }

        // Update price
        if (goodsPriceText != null)
        {
            goodsPriceText.text = $"Price: ${currentGoods.goodsPrice:F2}";
        }

        // Update icon
        if (goodsIconImage != null)
        {
            if (currentGoods.goodsIcon != null)
            {
                goodsIconImage.sprite = currentGoods.goodsIcon;
                goodsIconImage.gameObject.SetActive(true);
            }
            else
            {
                goodsIconImage.gameObject.SetActive(false);
            }
        }

        // Update button availability and visibility
        if (confirmPurchaseButton != null)
        {
            bool canAfford = moneyManager == null || moneyManager.CanAfford(currentGoods.goodsPrice);
            bool isAvailable = currentGoods.isAvailable;
            bool canPurchase = isAvailable && canAfford;
            
            confirmPurchaseButton.interactable = canPurchase;
            confirmPurchaseButton.gameObject.SetActive(true);
            
            if (!isAvailable)
            {
                Debug.Log($"GoodsDetailView: Confirm purchase button disabled - goods not available");
            }
            else if (!canAfford)
            {
                Debug.Log($"GoodsDetailView: Confirm purchase button disabled - insufficient funds (Need ${currentGoods.goodsPrice:F2}, have ${moneyManager?.GetCurrentMoney():F2})");
            }
            else
            {
                Debug.Log($"GoodsDetailView: Confirm purchase button enabled - can purchase");
            }
        }
        else
        {
            Debug.LogError("GoodsDetailView: ConfirmPurchaseButton is null!");
        }
        
        if (cancelPurchaseButton != null)
        {
            cancelPurchaseButton.gameObject.SetActive(true);
            Debug.Log("GoodsDetailView: Cancel purchase button activated");
        }
        else
        {
            Debug.LogError("GoodsDetailView: CancelPurchaseButton is null!");
        }

        Debug.Log($"GoodsDetailView: Updated display for '{currentGoods.goodsName}' - Price: ${currentGoods.goodsPrice}, Available: {currentGoods.isAvailable}");
    }

    /// <summary>
    /// Handle confirm purchase button click
    /// </summary>
    private void OnConfirmPurchaseClicked()
    {
        if (currentGoods == null)
        {
            Debug.LogWarning("GoodsDetailView: No goods selected for purchase!");
            return;
        }

        if (!currentGoods.isAvailable)
        {
            Debug.LogWarning($"GoodsDetailView: Cannot purchase '{currentGoods.goodsName}' - not available!");
            return;
        }

        // Check if player can afford the purchase
        if (moneyManager != null && !moneyManager.CanAfford(currentGoods.goodsPrice))
        {
            Debug.LogWarning($"GoodsDetailView: Cannot purchase '{currentGoods.goodsName}' - insufficient funds! Need ${currentGoods.goodsPrice:F2}, have ${moneyManager.GetCurrentMoney():F2}");
            return;
        }

        // Show confirmation panel if available
        if (confirmationPanel != null && confirmationText != null)
        {
            string moneyInfo = moneyManager != null ? $" (You have ${moneyManager.GetCurrentMoney():F2})" : "";
            confirmationText.text = $"Are you sure you want to purchase {currentGoods.goodsName} for ${currentGoods.goodsPrice:F2}?{moneyInfo}";
            confirmationPanel.SetActive(true);
        }
        else
        {
            // Direct purchase without confirmation
            ProcessPurchase();
        }
    }

    /// <summary>
    /// Handle cancel purchase button click
    /// </summary>
    private void OnCancelPurchaseClicked()
    {
        Debug.Log($"GoodsDetailView: Purchase cancelled for '{currentGoods?.goodsName}'");
        OnPurchaseCancelled?.Invoke(currentGoods);
        CloseDetailView();
    }

    /// <summary>
    /// Handle confirm yes button click (from confirmation panel)
    /// </summary>
    private void OnConfirmYesClicked()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
        ProcessPurchase();
    }

    /// <summary>
    /// Handle confirm no button click (from confirmation panel)
    /// </summary>
    private void OnConfirmNoClicked()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
        Debug.Log($"GoodsDetailView: Purchase cancelled in confirmation for '{currentGoods?.goodsName}'");
    }

    /// <summary>
    /// Process the actual purchase
    /// </summary>
    private void ProcessPurchase()
    {
        if (currentGoods == null)
        {
            Debug.LogError("GoodsDetailView: Cannot process purchase - no goods selected!");
            return;
        }

        // Spend the money
        if (moneyManager != null)
        {
            if (!moneyManager.SpendMoney(currentGoods.goodsPrice))
            {
                Debug.LogError("GoodsDetailView: Failed to spend money for purchase!");
                return;
            }
        }

        Debug.Log($"GoodsDetailView: Purchase confirmed for '{currentGoods.goodsName}' at ${currentGoods.goodsPrice:F2}");
        
        // Set the goods to unavailable after purchase
        if (goodsManager != null)
        {
            goodsManager.SetGoodsAvailability(currentGoods.goodsName, false);
            Debug.Log($"GoodsDetailView: Set '{currentGoods.goodsName}' to unavailable after purchase");
        }
        
        // Refresh all GoodsPreview components to reflect the availability change
        RefreshAllGoodsPreviews();
        
        // Fire the purchase confirmed event
        OnPurchaseConfirmed?.Invoke(currentGoods);
        
        // Close the detail view
        CloseDetailView();
    }

    /// <summary>
    /// Close the detail view
    /// </summary>
    public void CloseDetailView()
    {
        if (detailPanel != null)
            detailPanel.SetActive(false);
            
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
            
        currentGoods = null;
        OnDetailViewClosed?.Invoke();
        
        Debug.Log("GoodsDetailView: Detail view closed");
    }

    /// <summary>
    /// Get the currently displayed goods
    /// </summary>
    public Goods GetCurrentGoods()
    {
        return currentGoods;
    }

    /// <summary>
    /// Check if the detail view is currently open
    /// </summary>
    public bool IsDetailViewOpen()
    {
        return detailPanel != null && detailPanel.activeInHierarchy;
    }

    /// <summary>
    /// Test method to show a random goods detail (for testing)
    /// </summary>
    [ContextMenu("Show Random Goods Detail")]
    public void ShowRandomGoodsDetail()
    {
        if (goodsManager == null)
        {
            Debug.LogError("GoodsDetailView: GoodsManager not found!");
            return;
        }

        var availableGoods = goodsManager.GetAvailableGoods();
        if (availableGoods.Count > 0)
        {
            int randomIndex = Random.Range(0, availableGoods.Count);
            ShowGoodsDetail(availableGoods[randomIndex]);
        }
        else
        {
            Debug.LogWarning("GoodsDetailView: No available goods to show");
        }
    }

    /// <summary>
    /// Test method to simulate purchase (for testing)
    /// </summary>
    [ContextMenu("Simulate Purchase")]
    public void SimulatePurchase()
    {
        if (currentGoods != null)
        {
            ProcessPurchase();
        }
        else
        {
            Debug.LogWarning("GoodsDetailView: No goods selected for purchase simulation");
        }
    }
    
    /// <summary>
    /// Test method to force show buttons (for debugging)
    /// </summary>
    [ContextMenu("Force Show Buttons")]
    public void ForceShowButtons()
    {
        if (confirmPurchaseButton != null)
        {
            confirmPurchaseButton.gameObject.SetActive(true);
            confirmPurchaseButton.interactable = true;
            Debug.Log("GoodsDetailView: Force activated confirm purchase button");
        }
        else
        {
            Debug.LogError("GoodsDetailView: ConfirmPurchaseButton is null!");
        }
        
        if (cancelPurchaseButton != null)
        {
            cancelPurchaseButton.gameObject.SetActive(true);
            Debug.Log("GoodsDetailView: Force activated cancel purchase button");
        }
        else
        {
            Debug.LogError("GoodsDetailView: CancelPurchaseButton is null!");
        }
    }
    
    /// <summary>
    /// Test method to check button status (for debugging)
    /// </summary>
    [ContextMenu("Check Button Status")]
    public void CheckButtonStatus()
    {
        Debug.Log($"GoodsDetailView: ConfirmPurchaseButton assigned: {confirmPurchaseButton != null}");
        if (confirmPurchaseButton != null)
        {
            Debug.Log($"GoodsDetailView: ConfirmPurchaseButton active: {confirmPurchaseButton.gameObject.activeInHierarchy}");
            Debug.Log($"GoodsDetailView: ConfirmPurchaseButton interactable: {confirmPurchaseButton.interactable}");
        }
        
        Debug.Log($"GoodsDetailView: CancelPurchaseButton assigned: {cancelPurchaseButton != null}");
        if (cancelPurchaseButton != null)
        {
            Debug.Log($"GoodsDetailView: CancelPurchaseButton active: {cancelPurchaseButton.gameObject.activeInHierarchy}");
        }
    }
    
    /// <summary>
    /// Refresh all GoodsPreview components to reflect availability changes
    /// </summary>
    private void RefreshAllGoodsPreviews()
    {
        // Find all GoodsPreview components in the scene
        GoodsPreview[] allPreviews = FindObjectsOfType<GoodsPreview>();
        
        foreach (GoodsPreview preview in allPreviews)
        {
            preview.RefreshGoodsAvailability();
        }
        
        Debug.Log($"GoodsDetailView: Refreshed {allPreviews.Length} GoodsPreview components");
    }
} 