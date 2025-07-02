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
3. Name your item (e.g., "Phone", "Wallet", "Watch", "Key", "Note")
4. Assign an icon sprite to the `icon` field

### Step 3: Setup Hotbar Inventory UI
1. Create a **Canvas** in your scene
2. Create a **Panel** for the hotbar (position at bottom of screen)
3. Create **6 Image components** for the hotbar slots (arranged horizontally)
4. Add the `InventoryUI` script to the Canvas
5. Assign the 6 slot Images to the `hotbarSlots` array in order (left to right)
6. Create your fixed items (Phone, Wallet, Watch) as ScriptableObjects
7. Assign the fixed items to the `fixedItems` list in order (Phone, Wallet, Watch)

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

### Prerequisites
- Ensure your project uses the **Universal Render Pipeline (URP)**
- Post-Processing must be enabled in your URP Asset

### Step 1: Create Phone Panel
1. Create a **Panel** as a child of your Canvas for the phone interface
2. Design your phone interface (buttons, text, etc.) inside this panel
3. Set the panel to inactive by default

### Step 2: Setup Post-Processing Volume
1. Create an empty GameObject in the Hierarchy named **PostProcessVolume**
2. Add a **Volume** component to it
3. Check **Is Global** in the Volume component
4. Create a new **Volume Profile** and assign it to the Volume
5. In the Volume Profile, click **Add Override** ▶ **Unity** ▶ **DepthOfField**
6. Configure the DepthOfField settings:
   - **Focus Distance**: 0.1 (keeps phone in sharp focus)
   - **Aperture**: 32 (higher values = stronger blur)
   - **Focal Length**: 50
7. Set the Volume's **Weight** to **0** initially

### Step 3: Configure Phone Controller
1. Add the `PhoneUIController` script to the Canvas
2. Assign the phone panel to the `phonePanel` field
3. Assign the PostProcessVolume to the `postProcessVolume` field

### Step 4: Alternative UI Blur Shader (Optional)
If you prefer a UI-based blur instead of post-processing:
1. Create the **UIBlur.shader** in `Assets/Shaders/`
2. Create a material from the shader named **UIBlurMat**
3. Create a full-screen Image as **BlurOverlay** behind the phone panel
4. Assign the UIBlurMat to the BlurOverlay Image
5. Adjust the **Blur Size** property on the material

### Step 5: Customize Phone Interface
- Add buttons for different phone functions
- Add text displays for messages or information
- Style the panel to look like a phone interface
- Configure blur intensity through DepthOfField settings or shader properties

## ⏰ 4. Time Progression System

### Step 1: Create Time Manager
1. Create an empty GameObject in your scene
2. Rename it to "TimeManager"
3. Add the `TimeManager` script to it
4. Configure day schedule times (morning/evening windows)
5. Add event names and their durations in the inspector

### Step 2: Setup Day-Part Manager (Optional)
1. Create an empty GameObject named "DayPartManager"
2. Add the `DayPartManager` script to it
3. Create two Volume GameObjects for morning and evening post-processing
4. Assign the volumes and directional light to the script
5. Configure lighting intensities for different times of day

### Step 3: Create Event Objects
1. Create a new layer called "EventObject"
2. Create GameObjects for event-trigger objects in your world
3. Add the `EventObject` script to each
4. Set the `eventName` to match names defined in TimeManager
5. Assign objects to the "EventObject" layer

### Step 4: Setup Event Interaction
1. Add the `EventInteractionController` script to your Camera
2. Set the `eventLayer` to include "EventObject"
3. Configure `interactDistance` as needed

### Step 5: Debug Testing (Optional)
1. Create an empty GameObject named "EventTester"
2. Add the `EventTester` script to it
3. Assign event names to test via number keys

### Usage in Your Game
The time system now works with events:

```csharp
// Events are triggered by interacting with EventObject components
// Time advances automatically when events are triggered
// Day-part changes happen automatically based on current time
```

## 🔍 5. Examine System Setup

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

## 🎯 6. Interaction Hint UI Setup

### Step 1: Create Hint Canvas
1. In your UI Canvas (Screen Space - Overlay), create a **HintCanvas** GameObject
2. Create an **Image** as a child named "HintIcon"
3. Anchor the HintIcon to center (position 0.5, 0.5)
4. Assign a small white dot sprite as default
5. Set the Image component to **disabled** by default

### Step 2: Setup Hint Controller
1. Add the `InteractionHintController` script to your **Player** (with Camera)
2. Assign your main Camera to the `cam` field
3. Assign the HintIcon Image to the `hintImage` field
4. Set `hintDistance` to match your interaction range (e.g., 3f)
5. Set `interactableLayers` to include Door, Pickable, and Interactable layers

### Step 3: Configure Hint Sprites
1. Create or assign sprites for different interaction types:
   - **defaultDot**: Small white dot for general interaction
   - **doorIcon**: Door icon for door interactions
   - **handIcon**: Hand icon for pickup interactions
2. Assign these sprites to the corresponding fields in the script

### Step 4: Setup Object Tags and Layers
1. **Door objects**: Tag as "Door"
2. **Pickable objects**: Set layer to "Pickable" (or use existing "Interactable")
3. **Other interactive objects**: Use your existing "Interactable" layer

## 🧹 7. Garbage Cleanup Mini-Game Setup

### Step 1: TimeManager Configuration
1. In your **TimeManager** inspector, add a new event:
   - **Event Name**: "GarbageCleanup"
   - **Duration**: 30 (minutes)

### Step 2: Create Spawn Ranges
1. Create empty GameObjects with **BoxCollider** components (set as trigger) to define spawn volumes:
   - **StainRanges** parent object with children representing individual ranges
   - **FecesRanges** parent object with children representing individual ranges
   - **TrashRanges** parent object with children representing individual ranges
2. Ensure all BoxColliders include the floor bounds and have appropriate size and height
3. Set the **Floor** layer to any floors in your scene

### Step 3: Setup Garbage Controller
1. Create an empty GameObject named "GarbageCleanupController"
2. Add the `GarbageCleanupController` script to it
3. Assign the range parent transforms to the corresponding arrays:
   - `stainRanges`: Array of Transform components from StainRanges children
   - `fecesRanges`: Array of Transform components from FecesRanges children
   - `trashRanges`: Array of Transform components from TrashRanges children
4. Assign garbage prefabs:
   - `stainPrefab`: Prefab for stain objects
   - `fecesPrefab`: Prefab for elephant feces objects
   - `trashPrefab`: Prefab for trash objects
5. Configure spawn counts (default: 10 stains, 5 feces, 8 trash)
6. Set `floorLayer` to your floor layer
7. Adjust `verticalOffset` if needed (default: 0.01f)

### Step 4: Create Garbage Prefabs
1. Create prefabs for each garbage type (stains, feces, trash)
2. Add a **Collider** component to each prefab
3. Add the `GarbageItem` script to each prefab
4. Configure `interactDistance` as needed (default: 2f)

### Step 5: Create Event Trigger
1. Create an empty GameObject at the location where you want to trigger the garbage cleanup
2. Add the `EventObject` component to it
3. Set the `eventName` to "GarbageCleanup"
4. Assign it to the "EventObject" layer

### Step 6: Verify EventInteractionController
The `EventInteractionController` has been updated to handle the "GarbageCleanup" event automatically. When a player interacts with an EventObject named "GarbageCleanup", it will:
1. Call `GarbageCleanupController.Instance.StartMinigame()`
2. Generate garbage items in the defined ranges
3. Allow players to click on garbage items to clean them

### Usage
- Players interact with the garbage cleanup trigger object
- Garbage items are procedurally generated in the defined ranges
- Players approach each piece and click to clean it
- The event advances time by 30 minutes when triggered

## 🎯 8. Complete Scene Setup Checklist

### Player Setup
- [ ] Player GameObject with CharacterController
- [ ] Camera as child of Player
- [ ] FirstPersonController script attached
- [ ] Camera Transform assigned
- [ ] Interactable layer configured

### Inventory Setup
- [ ] InventoryManager GameObject with Inventory script
- [ ] Canvas with InventoryUI script
- [ ] Hotbar panel created at bottom of screen
- [ ] 6 slot Images created and assigned to hotbarSlots array
- [ ] Fixed items (Phone, Wallet, Watch) created as ScriptableObjects
- [ ] Fixed items assigned to fixedItems list in correct order
- [ ] At least one additional Item ScriptableObject created for testing

### Phone Setup
- [ ] Phone panel created and designed
- [ ] Post-Processing Volume created and configured
- [ ] PhoneUIController script attached to Canvas
- [ ] Phone panel assigned in inspector
- [ ] Post-Processing Volume assigned in inspector
- [ ] Both UI elements set to inactive by default

### Time Tracking Setup
- [ ] TimeManager GameObject with TimeManager script
- [ ] Day schedule times configured (morning/evening windows)
- [ ] Event names and durations defined in inspector
- [ ] DayPartManager GameObject with DayPartManager script (optional)
- [ ] Morning and evening Volume GameObjects created (optional)
- [ ] EventObject layer created
- [ ] EventInteractionController attached to Camera
- [ ] EventTester GameObject for debugging (optional)

### Examine System Setup
- [ ] Examinable layer created
- [ ] ExamineController script attached to Camera
- [ ] HoldPoint created as child of Camera
- [ ] HoldPoint assigned in ExamineController
- [ ] Examinable layer assigned in ExamineController
- [ ] At least one examinable object created with proper components

### Interaction Hint UI Setup
- [ ] HintCanvas created in UI Canvas
- [ ] HintIcon Image created and centered
- [ ] InteractionHintController attached to Player
- [ ] Camera assigned to hint controller
- [ ] HintIcon assigned to hint controller
- [ ] Hint sprites (defaultDot, doorIcon, handIcon) assigned
- [ ] Interactable layers configured
- [ ] Object tags and layers set up (Door, Pickable, Interactable)

### Garbage Cleanup Setup
- [ ] "GarbageCleanup" event added to TimeManager with 30-minute duration
- [ ] StainRanges, FecesRanges, and TrashRanges parent objects created
- [ ] BoxCollider components added to range objects (set as triggers)
- [ ] GarbageCleanupController GameObject created with script attached
- [ ] Range transforms assigned to controller arrays
- [ ] Garbage prefabs created with Collider and GarbageItem components
- [ ] Prefabs assigned to controller
- [ ] Floor layer configured and assigned to controller
- [ ] Event trigger object created with EventObject component
- [ ] Event trigger set to "GarbageCleanup" event name
- [ ] Event trigger assigned to "EventObject" layer

## 🎮 Input Controls

- **WASD**: Move player (disabled while examining objects)
- **Mouse**: Look around (disabled while examining objects)
- **Left Click**: Interact with objects / Trigger events
- **Left Click + Hold**: Pick up and examine objects (while holding, move mouse to rotate)
- **Left Click Release**: Drop examined object and resume movement
- **1-6 Keys**: Select hotbar slots (TODO: implement item usage)
- **1-N Keys**: Test events (when EventTester is active)
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
   - Check that hotbarSlots array has 6 Image components assigned
   - Verify fixedItems list has exactly 3 items assigned

4. **Phone panel not toggling**
   - Verify PhoneUIController script is attached
   - Check if phonePanel is assigned in inspector
   - Ensure Post-Processing Volume is assigned in inspector
   - Verify both UI elements are properly configured

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

8. **Events not triggering**
   - Check that EventObject components have correct eventName
   - Verify event names match those defined in TimeManager
   - Ensure EventInteractionController is attached to Camera
   - Check that eventLayer includes "EventObject" layer
   - Verify interactDistance is appropriate

9. **Time not advancing**
   - Check that events are within time windows (morning/evening)
   - Verify event durations don't exceed window limits
   - Ensure TimeManager has event names and durations configured
   - Check console for warning messages about event failures

10. **Day-part changes not working**
    - Verify DayPartManager is properly configured
    - Check that morning/evening Volume GameObjects exist
    - Ensure directional light is assigned
    - Verify time windows are correctly set in TimeManager

11. **Interaction hints not showing**
    - Check that InteractionHintController is attached to Player
    - Verify Camera is assigned to the hint controller
    - Ensure HintIcon Image is assigned and properly positioned
    - Check that interactableLayers includes the correct layers
    - Verify objects have proper tags ("Door") or layers ("Pickable", "Interactable")
    - Ensure hintDistance matches your interaction range

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