# Elephant Wash Mini-Game Setup Guide

This guide walks you through setting up the complete elephant wash mini-game system in Unity.

## 🎮 Step 1: Elephant Setup

### 1.1 Prepare the Elephant
1. Select your elephant GameObject in the scene
2. Ensure it has these components:
   - **MeshFilter** (required)
   - **MeshCollider** (required)
   - **MeshRenderer** (for visuals)
3. Create a separate GameObject for the wash manager (e.g., "ElephantWashManager")
4. Add the **ElephantWashManager** script to this GameObject

### 1.2 Configure ElephantWashManager
1. Assign the **elephantObject** (the elephant GameObject to wash)
2. Assign the **stainPrefab** (create this in Step 2)
3. Set **stainCount** (default: 20)
4. Assign UI elements:
   - **progressBar**: Slider for progress
   - **stainCountText**: TextMeshPro - Text (UI) for stain count
   - **washCanvas**: Canvas for wash UI
5. Assign camera positions (optional - for fixed camera views):
   - **washCameraPosition**: Where camera moves during wash (optional)
   - **originalCameraPosition**: Where camera returns after wash (optional)
6. Assign **playerController**: Reference to FirstPersonController
7. Assign **waterGun**: Water gun GameObject
8. Assign **waterGunController**: WaterGunController script
9. Configure audio:
   - **washAudioSource**: AudioSource for wash sounds
   - **washStartSound**: AudioClip for wash start
   - **washCompleteSound**: AudioClip for wash completion

## 🎯 Step 2: Stain System

### 2.1 Create Stain Prefab
1. Create an empty GameObject → Name it "StainPrefab"
2. Add **MeshRenderer** and **MeshFilter** (use a simple sphere or cube)
3. Add **Collider** (SphereCollider or BoxCollider)
4. Add **Stain** script
5. Configure the Stain component:
   - **health**: Number of hits to remove (default: 3)
   - **damageEffect**: Optional VFX prefab for damage feedback
   - **cleanSound**: Optional AudioClip for clean sound
6. Set the layer to "Stain" (create this layer)
7. Create a prefab from this GameObject

### 2.2 Create Stain Layer
1. Go to **Edit → Project Settings → Tags and Layers**
2. Create a new layer: "Stain" (e.g., Layer 11)
3. Assign this layer to your stain prefab

## 💧 Step 3: Water Gun System

### 3.1 Create Water Gun
1. Create an empty GameObject → Name it "WaterGun"
2. Add **WaterGunController** script
3. Create child GameObjects for each particle system:
   - **MainSpray**: Main water spray particle system
   - **MistSpray**: Secondary mist/steam particle system
   - **SplashEffects**: Splash particle effects
4. Configure each particle system:
   - **Shape**: Cone or Circle
   - **Emission**: Rate over Time: 100-200
   - **Start Lifetime**: 1-2 seconds
   - **Start Speed**: 10-20
   - **Start Size**: Small particles (0.1-0.3)
   - **Collision**: Enable, set to "World"
   - **Collision Layers**: Include "Stain" layer
5. Assign the **waterGunParent** (the main WaterGun GameObject)
6. Assign **splashPrefab** (optional VFX)
7. Configure audio settings

### 3.2 Water Gun Particle System Settings
```
Main Module:
- Duration: 0 (continuous)
- Start Lifetime: 1-2 seconds
- Start Speed: 10-20
- Start Size: 0.1-0.3
- Start Color: Blue/white for water

Emission Module:
- Rate over Time: 100-200

Shape Module:
- Shape: Cone
- Angle: 15-30 degrees
- Radius: 0.1-0.3

Collision Module:
- Type: World
- Collides With: Stain layer
- Send Collision Messages: Enabled
- Quality: High
```

## 🎨 Step 4: UI Setup

### 4.1 Create Wash Canvas
1. Create **Canvas** → Set to "Screen Space - Overlay"
2. Name it "WashCanvas"
3. Set to **inactive** by default
4. Add UI elements:
   - **Panel** for background
   - **Slider** for progress bar
   - **Text** for stain count
   - **Button** for exit (optional)

### 4.2 Configure Progress Bar
1. Select the Slider
2. Set **Min Value**: 0, **Max Value**: 1
3. Configure the **Fill Area** and **Fill** image
4. Set initial **Value**: 0

### 4.3 Configure Stain Count Text
1. Select the TextMeshPro - Text (UI) component
2. Set initial text: "Stains Left: 20"
3. Style the text as needed (TextMeshPro offers better text rendering)

## 🎯 Step 5: Trigger System

### 5.1 Create Wash Trigger
1. Create an empty GameObject → Name it "WashTrigger"
2. Add **Box Collider** or **Sphere Collider**
3. Set **Is Trigger**: true
4. Add **ElephantWashTrigger** script
5. Assign the **washManager** reference
6. Set **eventName**: "ElephantWash"
7. Set **minTimeRequired**: 30 (minutes)

### 5.2 Configure Trigger Collider
1. Size the collider to cover the wash area
2. Position it near the elephant or jacuzzi
3. Set layer to "Interactable" or "EventObject"

## ⏰ Step 6: TimeManager Integration

### 6.1 Add Wash Event
1. Select the **TimeManager** GameObject
2. In the **eventNames** list, add: "ElephantWash"
3. In the **eventDurations** list, add: 5 (minutes)
4. This allows the wash to consume 5 minutes of game time

## 🎮 Step 7: Player Integration

### 7.1 Update FirstPersonController
1. Select the **Player** GameObject
2. In **FirstPersonController**, assign:
   - **washManager**: Reference to ElephantWashManager
3. Now pressing **Key 5** will start the wash mini-game

## 🔊 Step 8: Audio Setup

### 8.1 Create Audio Sources
1. Add **AudioSource** to the elephant for wash sounds
2. Add **AudioSource** to the water gun for spray sounds
3. Add **AudioSource** to the wash trigger for interaction sounds

### 8.2 Audio Clips Needed
- **washStartSound**: Sound when wash begins
- **washCompleteSound**: Sound when wash completes
- **spraySound**: Sound when water gun sprays
- **cleanSound**: Sound when stain is cleaned
- **interactionSound**: Sound when trigger is activated

## 🧪 Step 9: Testing

### 9.1 Test Setup
1. Press **Play** in Unity
2. Press **Key 5** to start wash mini-game
3. **Move freely** around the elephant while washing
4. Hold **Left Mouse Button** to spray water
5. Clean all stains to complete the mini-game

### 9.2 Debug Features
- Right-click on **ElephantWashManager** → "Test Start Wash"
- Right-click on **ElephantWashManager** → "Force End Wash"
- Right-click on **WaterGunController** → "Test Start Spray"

## 🔧 Step 10: Troubleshooting

### Common Issues:
1. **Stains not spawning**: Check elephant has MeshFilter and MeshCollider
2. **Water not hitting stains**: Check particle collision settings and layer masks
3. **Camera not moving**: Check washCameraPosition and originalCameraPosition assignments
4. **UI not showing**: Check washCanvas is assigned and enabled
5. **Audio not playing**: Check AudioSource components and AudioClip assignments

### Performance Tips:
1. Limit **stainCount** to 20-30 for good performance
2. Use simple particle effects for water spray
3. Optimize stain prefab with simple meshes
4. Use object pooling for frequent spawn/destroy operations

## 🎯 Integration with Other Systems

### Elephant State Controller
The wash mini-game automatically calls:
```csharp
ElephantStateController.Instance.OnEventCompleted(true);
```
This increases the elephant's happiness and stability.

### Time Management
The wash consumes time from the TimeManager, ensuring it fits within the game's time system.

### Inventory System
You can extend this to require specific items (soap, sponge) to start the wash.

---

## 🎉 Setup Complete!

Your elephant wash mini-game is now ready to use. Players can interact with the wash trigger or press Key 5 to start cleaning the elephant with the water gun! 