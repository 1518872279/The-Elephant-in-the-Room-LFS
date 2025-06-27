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

## 🧪 Step 7: Testing

### 7.1 Create Event Tester
1. Create empty GameObject → Name it "EventTester"
2. Add **EventTester** script
3. Assign event names to `testEvents` array

### 7.2 Test Everything
1. **Movement**: WASD to move, mouse to look
2. **Pickup Items**: Click on items with "Interactable" layer
3. **Phone**: Press P to toggle
4. **Examine**: Hold left click on "Examinable" objects
5. **Events**: Click on "EventObject" items or press T+number keys
6. **Inventory**: Check hotbar at bottom of screen
7. **Interaction Hints**: Look at different objects to see context hints

## 🎯 What Each System Does

- **FirstPersonController**: Handles movement and basic interaction
- **Inventory**: Stores items and manages hotbar display
- **Phone**: Toggleable UI with blur background effect
- **TimeManager**: Controls game time progression through events
- **EventObjects**: World objects that trigger time events
- **ExamineController**: Allows detailed object inspection
- **DayPartManager**: Changes lighting based on time of day
- **InteractionHintController**: Shows context-sensitive interaction hints

## 🔧 Common Issues

- **Can't move**: Check CharacterController is attached
- **Can't interact**: Verify objects are on correct layers
- **Phone not working**: Check Post-Processing Volume is assigned
- **Events not triggering**: Ensure event names match in TimeManager
- **Inventory not showing**: Verify hotbar slots are assigned

This should get you started with all the basic systems working together! 