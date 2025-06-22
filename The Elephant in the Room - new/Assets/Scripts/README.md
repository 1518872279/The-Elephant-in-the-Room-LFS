# Core Systems Setup Guide

This guide provides step-by-step instructions for setting up the core systems in your Unity project.

## 📋 Prerequisites

- Unity 2022.3 LTS or newer
- Basic understanding of Unity's UI system
- Project using URP (Universal Render Pipeline) recommended

## 🎮 1. First-Person Controller Setup

### Step 1: Create Player GameObject
1. Create an empty GameObject in your scene
2. Rename it to "Player"
3. Position it at the desired starting location

### Step 2: Add Character Controller
1. Select the Player GameObject
2. Add Component → Physics → Character Controller
3. Adjust the Character Controller settings as needed:
   - Height: 2.0
   - Radius: 0.5
   - Center: (0, 1, 0)

### Step 3: Add Camera
1. Create a Camera as a child of the Player GameObject
2. Position it at (0, 1.6, 0) for eye-level view
3. Set the camera's rotation to (0, 0, 0)

### Step 4: Add FirstPersonController Script
1. Select the Player GameObject
2. Add Component → Scripts → FirstPersonController
3. Configure the script in the Inspector:
   - **Walk Speed**: 5 (adjust as needed)
   - **Look Speed**: 2 (adjust as needed)
   - **Camera Transform**: Drag the camera child object here
   - **Interact Distance**: 3 (adjust as needed)
   - **Interact Layer**: Set to the layer you want for interactable objects

### Step 5: Create Interactable Layer
1. Go to Edit → Project Settings → Tags and Layers
2. Create a new layer called "Interactable" (layer 8)
3. Assign this layer to objects you want to be interactable

## 🎒 2. Inventory System Setup

### Step 1: Create Inventory Manager
1. Create an empty GameObject in your scene
2. Rename it to "InventoryManager"
3. Add the `Inventory` script component

### Step 2: Create Item ScriptableObjects
1. Right-click in Project window → Create → Inventory → Item
2. Configure each item:
   - **Item Name**: Enter the item's name
   - **Icon**: Assign a sprite for the item

### Step 3: Create Inventory UI
1. Create a Canvas (Right-click in Hierarchy → UI → Canvas)
2. Create a Panel as a child of the Canvas
3. Rename the Panel to "InventoryPanel"
4. Add the `InventoryUI` script to the Panel
5. Configure the script:
   - **Panel**: Drag the InventoryPanel here
   - **Items Parent**: Create an empty GameObject as child of the panel and assign it here
   - **Slot Prefab**: Create a UI Image prefab for inventory slots

### Step 4: Create Inventory Slot Prefab
1. Create a UI Image in your Canvas
2. Set its size to 64x64 pixels
3. Add an Image component if not present
4. Drag this to your Project window to create a prefab
5. Delete the original from the scene
6. Assign this prefab to the InventoryUI's Slot Prefab field

## 📱 3. Phone UI Setup

### Step 1: Create Phone Panel
1. Create a Panel as a child of your Canvas
2. Rename it to "PhonePanel"
3. Style it to look like a phone interface
4. Add the `PhoneUIController` script
5. Assign the PhonePanel to the script's Phone Panel field

### Step 2: Add Phone Content
1. Add UI elements inside the PhonePanel as needed:
   - Buttons for apps
   - Text for messages
   - Images for backgrounds
2. Organize the content as desired

## ⏰ 4. Time Manager Setup

### Step 1: Create Time Manager
1. Create an empty GameObject in your scene
2. Rename it to "TimeManager"
3. Add the `TimeManager` script component

## 🔧 5. Creating Interactable Objects

### Step 1: Create Interactable Script
Create a new script that implements `IInteractable`:

```csharp
using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    public Item itemToPickup;
    
    public void Interact()
    {
        Inventory.Instance.Add(itemToPickup);
        Destroy(gameObject);
    }
}
```

### Step 2: Setup Interactable Object
1. Create a 3D object (cube, sphere, etc.)
2. Add a Collider component
3. Set the object's layer to "Interactable"
4. Add your interactable script component
5. Assign the item to pick up in the inspector

## 🎯 6. Input Configuration

### Default Controls:
- **WASD**: Move
- **Mouse**: Look around
- **Left Click**: Interact with objects
- **I**: Toggle inventory
- **P**: Toggle phone
- **Escape**: Unlock cursor (you may want to add this)

### Adding Escape Key Support:
Add this to your FirstPersonController script:

```csharp
void Update()
{
    if (Input.GetKeyDown(KeyCode.Escape))
    {
        Cursor.lockState = Cursor.lockState == CursorLockMode.Locked 
            ? CursorLockMode.None 
            : CursorLockMode.Locked;
    }
    
    // ... rest of your update code
}
```

## 🎨 7. UI Styling Tips

### Inventory Panel:
- Use a semi-transparent background
- Add a grid layout group for organized item display
- Consider adding item tooltips on hover

### Phone Panel:
- Use a dark theme with rounded corners
- Add app icons and labels
- Consider adding animations for opening/closing

## 🐛 8. Common Issues & Solutions

### Issue: Can't interact with objects
**Solution**: 
- Check that objects are on the "Interactable" layer
- Verify the Interact Layer mask is set correctly
- Ensure objects have colliders

### Issue: Inventory not updating
**Solution**:
- Check that InventoryManager exists in the scene
- Verify InventoryUI is properly connected
- Ensure slot prefab has an Image component

### Issue: Camera not moving
**Solution**:
- Check that camera is assigned to cameraTransform
- Verify CharacterController component is present
- Check for conflicting camera scripts

### Issue: UI not showing
**Solution**:
- Ensure Canvas has a CanvasScaler component
- Check that UI elements are active
- Verify EventSystem exists in the scene

## 📝 9. Usage Examples

### Adding Items to Inventory:
```csharp
// In an interactable object
public void Interact()
{
    Inventory.Instance.Add(myItem);
}
```

### Tracking Time:
```csharp
// Start tracking an action
TimeManager.Instance.StartAction("Reading");

// End tracking
TimeManager.Instance.EndAction();

// Get time spent
float timeSpent = TimeManager.Instance.GetTimeSpent("Reading");
```

### Creating Custom Interactables:
```csharp
public class Door : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        // Open door logic
        Debug.Log("Door opened!");
    }
}
```

## 🔄 10. Extending the Systems

### Adding Item Types:
Extend the Item ScriptableObject to include more properties:
```csharp
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public string description;
    public bool isStackable;
    public int maxStackSize = 1;
}
```

### Adding Inventory Categories:
Modify the Inventory system to support categories or tabs for different item types.

### Adding Save/Load:
Implement persistence for inventory items and time tracking using PlayerPrefs or JSON serialization.

---

## ✅ Final Checklist

- [ ] Player GameObject with CharacterController and FirstPersonController
- [ ] Camera as child of Player
- [ ] "Interactable" layer created
- [ ] InventoryManager GameObject with Inventory script
- [ ] Canvas with InventoryPanel and InventoryUI script
- [ ] Inventory slot prefab created
- [ ] PhonePanel with PhoneUIController script
- [ ] TimeManager GameObject with TimeManager script
- [ ] At least one interactable object for testing
- [ ] Item ScriptableObjects created
- [ ] All scripts properly assigned in Inspector

Once all items are checked, your core systems should be fully functional! 