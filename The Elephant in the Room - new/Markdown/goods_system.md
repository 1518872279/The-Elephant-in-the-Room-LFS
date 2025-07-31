# Goods Management System

This system provides a complete goods management solution with data storage and UI preview functionality.

## Overview

The system consists of four main components:
1. **Goods** - Data structure for goods information
2. **GoodsManager** - Central manager for storing and managing goods
3. **GoodsPreview** - UI component for displaying goods information (secondary interface)
4. **GoodsDetailView** - UI component for displaying full goods details with purchase options (tertiary interface)

## Components

### 1. Goods Data Structure

The `Goods` class stores all information about a goods item:

```csharp
[System.Serializable]
public class Goods
{
    public string goodsName;           // Name of the goods
    public string goodsInformation;    // Full description
    public string goodsShortIntro;     // Short version for preview
    public float goodsPrice;           // Price of the goods
    public Sprite goodsIcon;           // Optional icon
    public bool isAvailable;           // Availability status
}
```

### 2. GoodsManager

Central singleton manager that stores and manages all goods:

**Key Features:**
- Add/remove goods
- Search and filter goods
- Price range filtering
- Availability management
- Automatic initialization with default goods

**Main Methods:**
- `AddGoods()` - Add new goods
- `GetGoods()` - Get goods by name
- `GetAvailableGoods()` - Get only available goods
- `SetGoodsAvailability()` - Update availability
- `UpdateGoodsPrice()` - Update price

### 3. GoodsPreview (Secondary Interface)

UI component that displays goods information:

**UI Components:**
- `goodsNameText` - Displays the goods name
- `goodsShortIntroText` - Shows the short introduction
- `goodsPriceText` - Shows the price
- `goodsIconImage` - Optional icon display
- `selectButton` - Button for selection

**Features:**
- Automatic display updates
- Selection event handling
- Availability status handling
- Integration with GoodsDetailView
- Test methods for debugging

### 4. GoodsDetailView (Tertiary Interface)

UI component that displays full goods information with purchase options:

**UI Components:**
- `detailPanel` - Main panel for the detail view
- `goodsNameText` - Full goods name
- `goodsInfoText` - Complete goods information
- `goodsPriceText` - Price display
- `goodsIconImage` - Goods icon
- `confirmPurchaseButton` - Confirm purchase button
- `cancelPurchaseButton` - Cancel purchase button
- `confirmationPanel` - Optional purchase confirmation panel

**Features:**
- Full goods information display
- Purchase confirmation system
- Availability checking
- Event system for purchase handling
- Integration with GoodsPreview

## Setup

### 1. Create GoodsManager

1. Create an empty GameObject in your scene
2. Add the `GoodsManager` script to it
3. The manager will automatically initialize with default goods

### 2. Create GoodsPreview UI (Secondary Interface)

1. Create UI elements for displaying goods:
   - TextMeshProUGUI for goods name
   - TextMeshProUGUI for short introduction
   - TextMeshProUGUI for price
   - Image for icon (optional)
   - Button for selection

2. Add the `GoodsPreview` script to a GameObject

3. Assign the UI references in the inspector

### 3. Create GoodsDetailView UI (Tertiary Interface)

1. Create UI elements for the detail view:
   - Panel for the main detail view
   - TextMeshProUGUI for full goods name
   - TextMeshProUGUI for complete goods information
   - TextMeshProUGUI for price
   - Image for goods icon
   - Button for confirm purchase
   - Button for cancel purchase
   - Optional confirmation panel with Yes/No buttons

2. Add the `GoodsDetailView` script to a GameObject

3. Assign the UI references in the inspector

### 4. Configure Integration

**Option A: Connect via Inspector**
- Set the `detailView` reference in GoodsPreview
- The preview will automatically open the detail view when selected

**Option B: Set via Code**
```csharp
GoodsPreview preview = GetComponent<GoodsPreview>();
GoodsDetailView detailView = GetComponent<GoodsDetailView>();
preview.detailView = detailView;
```

## Usage Examples

### Adding Goods

```csharp
// Add goods via GoodsManager
GoodsManager.Instance.AddGoods("Pizza", 
    "Delicious Italian pizza with fresh ingredients.", 
    "Fresh Italian pizza.", 
    15.99f);
```

### Displaying Goods

```csharp
// Set goods to display
GoodsPreview preview = GetComponent<GoodsPreview>();
preview.SetGoods("Coffee");

// Listen for selection
preview.OnGoodsSelected += (goods) => {
    Debug.Log($"Selected: {goods.goodsName} - ${goods.goodsPrice}");
};

// Handle purchase events
GoodsDetailView detailView = GetComponent<GoodsDetailView>();
detailView.OnPurchaseConfirmed += (goods) => {
    Debug.Log($"Purchased: {goods.goodsName} for ${goods.goodsPrice}");
    // Add your purchase logic here
};
```

### Getting Goods

```csharp
// Get all available goods
var availableItems = GoodsManager.Instance.GetAvailableGoods();

// Get a specific goods by name
var coffee = GoodsManager.Instance.GetGoods("Coffee");
```

## Testing

### Context Menu Options

**GoodsManager:**
- "Print All Goods" - Print all goods to console

**GoodsPreview:**
- "Refresh Display" - Manually refresh the display
- "Set Random Goods" - Set a random available goods
- "Cycle Next Goods" - Cycle to the next goods in the list

**GoodsDetailView:**
- "Show Random Goods Detail" - Show a random goods detail
- "Simulate Purchase" - Simulate a purchase for testing

### Debug Features

- Automatic logging of all operations
- UI component validation warnings
- Availability status checking
- Price formatting

## Default Goods

The system initializes with these default goods:
- **Coffee** - $3.50 - Fresh brewed coffee
- **Sandwich** - $8.99 - Fresh sandwich with premium ingredients
- **Tea** - $2.99 - Soothing herbal tea
- **Cake** - $12.99 - Decadent chocolate cake
- **Water** - $1.99 - Pure spring water

## Event System

The system provides comprehensive event handling:

### GoodsPreview Events
```csharp
// Subscribe to goods selection
preview.OnGoodsSelected += OnGoodsSelected;

private void OnGoodsSelected(Goods selectedGoods)
{
    // Handle goods selection
    Debug.Log($"Selected: {selectedGoods.goodsName}");
}
```

### GoodsDetailView Events
```csharp
// Subscribe to purchase events
detailView.OnPurchaseConfirmed += OnPurchaseConfirmed;
detailView.OnPurchaseCancelled += OnPurchaseCancelled;
detailView.OnDetailViewClosed += OnDetailViewClosed;

private void OnPurchaseConfirmed(Goods purchasedGoods)
{
    // Handle purchase confirmation
    Debug.Log($"Purchased: {purchasedGoods.goodsName}");
}

private void OnPurchaseCancelled(Goods cancelledGoods)
{
    // Handle purchase cancellation
    Debug.Log($"Cancelled: {cancelledGoods.goodsName}");
}

private void OnDetailViewClosed()
{
    // Handle detail view closing
    Debug.Log("Detail view closed");
}
```

## Best Practices

1. **Use the singleton pattern** - Always access GoodsManager via `GoodsManager.Instance`
2. **Check availability** - Use `IsGoodsAvailable()` before allowing selection
3. **Handle missing goods** - Always check if goods exist before displaying
4. **Use events** - Subscribe to `OnGoodsSelected` for user interaction
5. **Validate UI components** - Check console for missing UI component warnings

## Extending the System

You can easily extend the system by:
- Adding new fields to the Goods class
- Creating specialized preview components
- Adding categories or tags to goods
- Implementing inventory management
- Adding purchase/sale functionality 