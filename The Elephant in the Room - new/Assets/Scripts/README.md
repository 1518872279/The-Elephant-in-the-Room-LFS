# Core Systems Setup Guide

This guide will help you set up all the foundational systems for your Unity project.

## 📋 Prerequisites

- Unity 2022.3 LTS or newer
- Universal Render Pipeline (URP) or Built-in Render Pipeline
- Basic knowledge of Unity's Inspector and Scene view

## 🎮 1. First-Person Controller Setup

### Step 1: Create Player GameObject
1. Create an empty GameObject in your scene
2. Rename it to "Player"
3. Add a **CharacterController** component

### Step 2: Add Camera
1. Create a **Camera** as a child of the Player GameObject
2. Position it at the player's head level (e.g., Y = 1.8)
3. Make sure it's not a child of any other object

### Step 3: Configure FirstPersonController
1. Add the `FirstPersonController` script to the Player GameObject
2. In the Inspector, assign the camera Transform to the `cameraTransform` field
3. Set the `interactLayer` to a layer you'll use for interactable objects
4. Adjust `walkSpeed` and `lookSpeed` as needed (default: 5f and 2f)

### Step 4: Create Interactable Layer
1. Go to **Edit > Project Settings > Tags and Layers**
2. Create a new layer called "Interactable" (e.g., layer 8)
3. Assign this layer to objects you want to be clickable

## 🎒 2. Inventory System Setup

### Step 1: Create Inventory Manager
1. Create an empty GameObject in your scene
2. Rename it to "InventoryManager"
3. Add the `Inventory` script to it

### Step 2: Create Item ScriptableObjects
1. Right-click in the Project window
2. Select **Create > Inventory > Item**
3. Name your item (e.g., "Key", "Phone", "Note")
4. Assign an icon sprite to the `icon` field

### Step 3: Setup Inventory UI
1. Create a **Canvas** in your scene
2. Create a **Panel** as a child of the Canvas (this will be your inventory panel)
3. Add a **ScrollView** or **Grid Layout Group** inside the panel for item slots
4. Create an empty GameObject as a child of the ScrollView/Grid (this will be `itemsParent`)
5. Create a **Button** prefab for inventory slots (this will be your `slotPrefab`)
   - Add an **Image** component to display item icons
   - Make it the size you want for inventory slots
6. Add the `InventoryUI` script to the Canvas
7. Assign the panel, itemsParent, and slotPrefab in the Inspector

### Step 4: Create Interactable Items
1. Create a GameObject for your item
2. Add a **Collider** component (Box Collider, Sphere Collider, etc.)
3. Set the layer to "Interactable"
4. Add a script that implements `IInteractable`:

```csharp
public class PickupItem : MonoBehaviour, IInteractable
{
    public Item itemData;
    
    public void Interact()
    {
        Inventory.Instance.Add(itemData);
        Destroy(gameObject);
    }
}
```

## 📱 3. Phone UI Setup

### Step 1: Create Phone Panel
1. Create a **Panel** as a child of your Canvas
2. Design your phone interface (buttons, text, etc.)
3. Add the `PhoneUIController` script to the Canvas
4. Assign the phone panel to the `phonePanel` field

### Step 2: Customize Phone Interface
- Add buttons for different phone functions
- Add text displays for messages or information
- Style the panel to look like a phone interface

## ⏰ 4. Time Manager Setup

### Step 1: Create Time Manager
1. Create an empty GameObject in your scene
2. Rename it to "TimeManager"
3. Add the `TimeManager` script to it

### Step 2: Usage in Your Game
Use the TimeManager to track player actions:

```csharp
// Start tracking an action
TimeManager.Instance.StartAction("Reading");

// End tracking when action is complete
TimeManager.Instance.EndAction();

// Get total time spent on an action
float timeSpent = TimeManager.Instance.GetTimeSpent("Reading");
```

## 🔍 5. Pickup & Examine System Setup

### Step 1: Create Examinable Layer
1. Go to **Edit > Project Settings > Tags and Layers**
2. Create a new layer called "Examinable" (e.g., layer 9)
3. This layer will be used for objects that can be picked up and examined

### Step 2: Setup Camera Examine Controller
1. Add the `ExamineController` script to your **Camera** (child of Player)
2. In the Inspector, assign the `examinableLayer` to the "Examinable" layer
3. Set `examineDistance` to 3f (or your preferred distance)
4. Set `rotationSpeed` to 5f (or your preferred rotation speed)
5. **Important**: The script will automatically find and control the FirstPersonController component

### Step 3: Create Hold Point
1. Create an empty GameObject as a child of your **Camera**
2. Rename it to "HoldPoint"
3. Position it in front of the camera (e.g., Z = 2)
4. Assign this Transform to the `holdParent` field in ExamineController

### Step 4: Setup Examinable Objects
1. Create GameObjects for objects you want to examine
2. Add a **Collider** component (Box Collider, Sphere Collider, etc.)
3. Add a **Rigidbody** component
4. Set the layer to "Examinable"
5. Add the `ExaminableObject` script (this will auto-configure the object)
6. Ensure the Rigidbody's `isKinematic` is set to **false** by default

### Step 5: Configure Object Properties
- **Collider**: Required for raycast detection
- **Rigidbody**: Required for physics interaction (set to non-kinematic by default)
- **Layer**: Must be set to "Examinable"
- **Size**: Ensure the object is appropriately sized for examination

## 🎯 6. Complete Scene Setup Checklist

### Player Setup
- [ ] Player GameObject with CharacterController
- [ ] Camera as child of Player
- [ ] FirstPersonController script attached
- [ ] Camera Transform assigned
- [ ] Interactable layer configured

### Inventory Setup
- [ ] InventoryManager GameObject with Inventory script
- [ ] Canvas with InventoryUI script
- [ ] Inventory panel created and assigned
- [ ] Items parent Transform assigned
- [ ] Slot prefab created and assigned
- [ ] At least one Item ScriptableObject created

### Phone Setup
- [ ] Phone panel created and designed
- [ ] PhoneUIController script attached to Canvas
- [ ] Phone panel assigned in inspector

### Time Tracking Setup
- [ ] TimeManager GameObject with TimeManager script

### Examine System Setup
- [ ] Examinable layer created
- [ ] ExamineController script attached to Camera
- [ ] HoldPoint created as child of Camera
- [ ] HoldPoint assigned in ExamineController
- [ ] Examinable layer assigned in ExamineController
- [ ] At least one examinable object created with proper components

### Interactable Objects
- [ ] Objects assigned to "Interactable" layer
- [ ] Colliders added to interactable objects
- [ ] IInteractable scripts implemented

## 🎮 Input Controls

- **WASD**: Move player (disabled while examining objects)
- **Mouse**: Look around (disabled while examining objects)
- **Left Click**: Interact with objects
- **Left Click + Hold**: Pick up and examine objects (while holding, move mouse to rotate)
- **Left Click Release**: Drop examined object and resume movement
- **I**: Toggle inventory
- **P**: Toggle phone
- **ESC**: Unlock cursor (you may want to add this functionality)

## 🔧 Troubleshooting

### Common Issues:

1. **Player can't move**
   - Check if CharacterController is attached
   - Verify Input Manager settings

2. **Can't interact with objects**
   - Ensure objects are on the "Interactable" layer
   - Check if objects have colliders
   - Verify interactDistance in FirstPersonController

3. **Inventory not showing**
   - Check if InventoryManager exists in scene
   - Verify InventoryUI script assignments
   - Ensure Canvas is set to Screen Space - Overlay

4. **Phone panel not toggling**
   - Verify PhoneUIController script is attached
   - Check if phonePanel is assigned in inspector

5. **Can't pick up examinable objects**
   - Ensure objects are on the "Examinable" layer
   - Check if objects have Collider and Rigidbody components
   - Verify examineDistance in ExamineController
   - Ensure HoldPoint is properly assigned

6. **Examined objects don't rotate**
   - Check if rotationSpeed is set in ExamineController
   - Verify the object is properly parented to HoldPoint
   - Ensure the object has a Rigidbody component

7. **Player can still move while examining**
   - Ensure ExamineController is attached to the Camera (child of Player)
   - Check that FirstPersonController is on the same GameObject as the Camera
   - Verify the script can find the FirstPersonController component

## 📝 Next Steps

After completing this setup:

1. Create your game's specific interactable objects
2. Design your inventory items and their icons
3. Implement your phone's specific functionality
4. Add time tracking to your game's key actions
5. Create examinable objects for your game world
6. Test all systems work together properly

## 🎨 Customization Tips

- Adjust movement and look speeds in FirstPersonController
- Modify inventory slot size and layout
- Design custom phone interface elements
- Add sound effects for interactions
- Implement visual feedback for interactions
- Customize examine rotation speed and distance
- Add particle effects or UI prompts for examinable objects

---

**Need Help?** Check the console for error messages and ensure all required components are properly assigned in the Inspector. 