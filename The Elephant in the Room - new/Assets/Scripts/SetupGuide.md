# Simple Setup Guide

This guide walks you through setting up all the systems step by step.

## 🎮 Step 1: Basic Player Setup

### 1.1 Create Player
1. Create an empty GameObject → Name it "Player"
2. Add **CharacterController** component
3. Add **FirstPersonController** script
4. Create a **Camera** as child → Name it "Main Camera"
5. Assign the Camera to `cameraTransform` in FirstPersonController

### 1.2 Create Layers
1. Go to **Edit → Project Settings → Tags and Layers**
2. Create these layers:
   - Layer 8: "Interactable" (for items you can pick up)
   - Layer 9: "Examinable" (for objects you can examine)
   - Layer 10: "EventObject" (for objects that trigger events)

## 🎒 Step 2: Inventory System

### 2.1 Create Inventory Manager
1. Create empty GameObject → Name it "InventoryManager"
2. Add **Inventory** script

### 2.2 Create Items
1. Right-click in Project → **Create → Inventory → Item**
2. Create these items:
   - "Phone" (assign an icon)
   - "Wallet" (assign an icon) 
   - "Watch" (assign an icon)
   - "Key" (assign an icon)

### 2.3 Setup Hotbar UI
1. Create **Canvas** → Set to "Screen Space - Overlay"
2. Create **Panel** as child → Position at bottom of screen
3. Create **6 Image components** inside panel → Arrange horizontally
4. Add **InventoryUI** script to Canvas
5. Assign the 6 Images to `hotbarSlots` array
6. Assign Phone, Wallet, Watch to `fixedItems` list

### 2.4 Create Pickup Items
1. Create a cube → Name it "Key"
2. Add **Box Collider**
3. Set layer to "Interactable"
4. Add **PickupItem** script
5. Assign the "Key" Item to `itemData`

## 📱 Step 3: Phone System

### 3.1 Create Phone UI
1. In Canvas, create **Panel** → Name it "PhonePanel"
2. Design phone interface inside (buttons, text, etc.)
3. Set PhonePanel to **inactive**

### 3.2 Setup Post-Processing (URP)
1. Create empty GameObject → Name it "PostProcessVolume"
2. Add **Volume** component → Check "Is Global"
3. Create new **Volume Profile** → Assign to Volume
4. In Profile → **Add Override → Unity → DepthOfField**
5. Set Focus Distance: 0.1, Aperture: 32, Focal Length: 50
6. Set Volume Weight to **0**

### 3.3 Configure Phone Controller
1. Add **PhoneUIController** script to Canvas
2. Assign PhonePanel to `phonePanel`
3. Assign PostProcessVolume to `postProcessVolume`

## ⏰ Step 4: Time System

### 4.1 Create Time Manager
1. Create empty GameObject → Name it "TimeManager"
2. Add **TimeManager** script
3. In inspector, add event names:
   - "Breakfast" (duration: 30)
   - "Work" (duration: 240)
   - "Dinner" (duration: 60)
   - "Sleep" (duration: 480)

### 4.2 Create Event Objects
1. Create a cube → Name it "BreakfastTable"
2. Add **Box Collider**
3. Set layer to "EventObject"
4. Add **EventObject** script
5. Set `eventName` to "Breakfast"

### 4.3 Setup Event Interaction
1. Add **EventInteractionController** script to Main Camera
2. Set `eventLayer` to include "EventObject"
3. Set `interactDistance` to 3

### 4.4 Optional: Day-Part Manager
1. Create empty GameObject → Name it "DayPartManager"
2. Add **DayPartManager** script
3. Create two Volume GameObjects for morning/evening lighting
4. Assign volumes and directional light to script

## 🔍 Step 5: Examine System

### 5.1 Setup Examine Controller
1. Add **ExamineController** script to Main Camera
2. Set `examinableLayer` to "Examinable"
3. Create empty GameObject as child of Camera → Name it "HoldPoint"
4. Position HoldPoint in front of camera (Z = 2)
5. Assign HoldPoint to `holdParent`

### 5.2 Create Examinable Objects
1. Create a sphere → Name it "MysteryObject"
2. Add **Sphere Collider**
3. Add **Rigidbody** (set isKinematic to false)
4. Set layer to "Examinable"
5. Add **ExaminableObject** script

## 🎯 Step 6: Interaction Hint UI

### 6.1 Create Hint Canvas
1. In Canvas, create **HintCanvas** GameObject
2. Create **Image** as child → Name it "HintIcon"
3. Anchor HintIcon to center (position 0.5, 0.5)
4. Assign small white dot sprite as default
5. Set Image component to **disabled**

### 6.2 Setup Hint Controller
1. Add **InteractionHintController** script to Player
2. Assign Main Camera to `cam` field
3. Assign HintIcon to `hintImage` field
4. Set `hintDistance` to 3
5. Set `interactableLayers` to include Door, Pickable, Interactable

### 6.3 Configure Hint Sprites
1. Create/assign sprites for different interactions:
   - **defaultDot**: Small white dot
   - **doorIcon**: Door icon
   - **handIcon**: Hand icon
2. Assign sprites to corresponding fields

### 6.4 Setup Object Tags
1. **Door objects**: Tag as "Door"
2. **Pickable objects**: Set layer to "Pickable"
3. **Other interactive**: Use "Interactable" layer

## 🧹 Step 7: Garbage Cleanup Mini-Game

### 7.1 Add Event to TimeManager
1. In **TimeManager** inspector, add new event:
   - Event Name: "GarbageCleanup"
   - Duration: 30

### 7.2 Create Spawn Ranges
1. Create empty GameObject → Name it "StainRanges"
2. Create 3 child objects → Name them "StainRange1", "StainRange2", "StainRange3"
3. Add **Box Collider** to each child → Check "Is Trigger"
4. Repeat for "TrashRanges" (3 children)
5. Position ranges where you want garbage to spawn (floor areas only)
6. **Note**: Items will be distributed evenly across all ranges (e.g., 10 stains across 3 ranges = 3, 3, 4)

### 7.3 Create Garbage Controller
1. Create empty GameObject → Name it "GarbageCleanupController"
2. Add **GarbageCleanupController** script
3. Assign StainRanges children to `stainRanges` array
4. Assign TrashRanges children to `trashRanges` array
5. Set `stainCount` to 10 and `trashCount` to 8

### 7.4 Create Garbage Prefabs
1. Create multiple stain variations:
   - Create a cube → Name it "StainPrefab1"
   - Add **Box Collider**
   - Create prefab from this object
   - Repeat for 2-3 more stain variations
2. Create multiple trash variations:
   - Create a cube → Name it "TrashPrefab1"
   - Add **Box Collider**
   - Create prefab from this object
   - Repeat for 2-3 more trash variations
3. Assign all prefab arrays to GarbageCleanupController

### 7.5 Setup UI Elements
1. Create **Text** component for debug display:
   - In Canvas, create **Text** → Name it "DebugText"
   - Position it where you want progress to show
   - Assign to `debugText` in GarbageCleanupController
2. Create **Fade Image** for transitions:
   - In Canvas, create **Image** → Name it "FadeImage"
   - Set color to black, alpha to 0
   - Cover full screen
   - Assign to `fadeImage` in GarbageCleanupController
   - **Note**: If using same canvas as CookingMinigameController, the controller will automatically re-enable it when needed

### 7.6 Create Event Trigger
1. Create empty GameObject → Name it "GarbageTrigger"
2. Add **EventObject** script
3. Set `eventName` to "GarbageCleanup"
4. Set layer to "EventObject"

### 7.7 Configure Floor Layer
1. Create layer called "Floor"
2. Assign floor objects to this layer
3. Set `floorLayer` in GarbageCleanupController to "Floor"

### 7.8 How It Works
- Player interacts with event object to start mini-game
- System spawns random variations of stains and trash on floor
- Items are distributed evenly across all spawn ranges
- Debug text shows progress (e.g., "Cleaned: 5 / 18")
- Click to clean each item
- When all items are cleaned, fade transition plays
- Event automatically advances time by 30 minutes

## 🧪 Step 8: Testing

### 7.1 Create Event Tester
1. Create empty GameObject → Name it "EventTester"
2. Add **EventTester** script
3. Assign event names to `testEvents` array

### 8.1 Create Event Tester
1. Create empty GameObject → Name it "EventTester"
2. Add **EventTester** script
3. Assign event names to `testEvents` array

### 8.2 Test Everything
1. **Movement**: WASD to move, mouse to look
2. **Pickup Items**: Click on items with "Interactable" layer
3. **Phone**: Press P to toggle
4. **Examine**: Hold left click on "Examinable" objects
5. **Events**: Click on "EventObject" items or press T+number keys
6. **Inventory**: Check hotbar at bottom of screen
7. **Interaction Hints**: Look at different objects to see context hints
8. **Garbage Cleanup**: Click on GarbageTrigger to spawn garbage, then click on garbage items to clean them

## 🎯 What Each System Does

- **FirstPersonController**: Handles movement and basic interaction
- **Inventory**: Stores items and manages hotbar display
- **Phone**: Toggleable UI with blur background effect
- **TimeManager**: Controls game time progression through events
- **EventObjects**: World objects that trigger time events
- **ExamineController**: Allows detailed object inspection
- **DayPartManager**: Changes lighting based on time of day
- **InteractionHintController**: Shows context-sensitive interaction hints
- **GarbageCleanupController**: Procedurally generates garbage items for cleanup mini-game
- **GarbageItem**: Handles individual garbage item interaction and cleanup

## 🔧 Common Issues

- **Can't move**: Check CharacterController is attached
- **Can't interact**: Verify objects are on correct layers
- **Phone not working**: Check Post-Processing Volume is assigned
- **Events not triggering**: Ensure event names match in TimeManager
- **Inventory not showing**: Verify hotbar slots are assigned

This should get you started with all the basic systems working together! 