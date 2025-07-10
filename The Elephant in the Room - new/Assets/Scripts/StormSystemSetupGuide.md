# Storm Visual Effects System Setup Guide

This guide explains how to set up and use the storm visual effects system in your Unity project.

## 🌩️ System Overview

The storm system consists of several components that work together to create immersive weather effects:

- **StormController**: Main controller that manages all storm subsystems
- **RainOverlayController**: Handles UI rain overlay effects
- **LightningController**: Manages lightning flashes and light effects
- **WindAffectedObject**: Makes objects respond to wind
- **WindShader**: Shader for wind-affected materials
- **WetLensShader**: Shader for camera lens wetness effects

## 🎯 Quick Start

### Step 1: Create Storm Controller
1. Create an empty GameObject → Name it "StormController"
2. Add the **StormController** script
3. This will be your main control point for the entire storm system

### Step 2: Setup Rain System
1. Create a **ParticleSystem** GameObject → Name it "RainSystem"
2. Configure the ParticleSystem for rain (use existing Storm Effects assets)
3. Assign the RainSystem GameObject to `rainSystem` in StormController

### Step 3: Setup Rain Overlay
1. Create a **Canvas** → Set to "Screen Space - Overlay"
2. Create a **RawImage** as child → Name it "RainOverlay"
3. Assign a rain texture to the RawImage
4. Add **RainOverlayController** script to the RawImage
5. Assign the RawImage to `overlay` in RainOverlayController
6. Assign the RainOverlayController to `rainOverlay` in StormController

### Step 4: Setup Wind Zone
1. Create a **WindZone** GameObject → Name it "WindZone"
2. Configure wind settings (Main, Turbulence, Pulse Magnitude, Pulse Frequency)
3. Assign the WindZone to `windZone` in StormController

### Step 5: Setup Lightning
1. Create a **Light** → Name it "StormLight"
2. Create a **UI Image** → Name it "FlashImage" (full screen, white)
3. Add **LightningController** script to StormLight
4. Assign the Light to `mainLight` in LightningController
5. Assign the FlashImage to `flashImage` in LightningController
6. Assign the LightningController to `lightningController` in StormController

### Step 6: Setup Audio
1. Create an **AudioSource** → Name it "StormAudio"
2. Assign rain/wind audio clip
3. Assign the AudioSource to `stormAudio` in StormController

## 🌬️ Wind-Affected Objects

### Step 1: Setup Wind Shader
1. Use the existing **WindShader.shader** in Assets/Shaders/
2. Create materials using this shader for objects that should respond to wind
3. Configure shader properties:
   - `_WindDirection`: Wind direction vector
   - `_WindStrength`: Wind intensity
   - `_WindFrequency`: Wind oscillation frequency
   - `_WindAmplitude`: Wind oscillation amplitude

### Step 2: Add WindAffectedObject Script
1. Select objects that should respond to wind (trees, grass, flags, etc.)
2. Add the **WindAffectedObject** script
3. Configure settings:
   - **Wind Sensitivity**: How much the object responds to wind (0-1)
   - **Max Bend Angle**: Maximum rotation angle in degrees
   - **Wind Frequency**: Local wind oscillation frequency
   - **Material Properties**: Names of shader properties (usually default)

### Step 3: Apply Wind Materials
1. Create materials using the WindShader
2. Assign these materials to objects with WindAffectedObject script
3. The script will automatically update shader properties based on WindZone

## 🎮 Manual Testing

### Using Inspector Buttons
1. Select the StormController in the hierarchy
2. Right-click in the Inspector
3. Choose **"Activate Storm"** or **"Deactivate Storm"**
4. This lets you test the storm system manually

### Using Code
```csharp
// Activate storm
StormController.Instance.ActivateStorm();

// Deactivate storm
StormController.Instance.DeactivateStorm();
```

## 🔧 Configuration Tips

### Rain Particle System Settings
- **Start Lifetime**: 2-3 seconds
- **Start Speed**: 10-15 m/s
- **Start Size**: 0.1-0.3
- **Shape**: Box or Cone
- **Emission Rate**: 1000-5000 particles/second
- **Gravity Modifier**: 1-2

### Wind Zone Settings
- **Main**: 0.5-2.0 (wind strength)
- **Turbulence**: 0.1-0.5 (random wind variation)
- **Pulse Magnitude**: 0.1-0.3 (wind oscillation strength)
- **Pulse Frequency**: 0.5-2.0 (wind oscillation speed)

### Lightning Settings
- **Min Interval**: 5-10 seconds between flashes
- **Max Interval**: 15-30 seconds between flashes
- **Flash Intensity**: 3-8 (light multiplier)
- **Flash Duration**: 0.1-0.3 seconds

## 🎨 Visual Enhancement

### Wet Lens Effect
1. Create a material using **WetLensShader**
2. Apply to a full-screen quad or camera overlay
3. Configure properties:
   - **Distortion Strength**: 0.01-0.05
   - **Raindrop Scale**: 1-5
   - **Raindrop Speed**: 0.5-2.0
   - **Wetness**: 0.3-0.8

### Rain Overlay Settings
1. Use a seamless rain texture
2. Set **Scroll Speed** in RainOverlayController:
   - X: 0 (no horizontal scroll)
   - Y: -0.3 to -1.0 (downward scroll speed)

## 🐛 Troubleshooting

### Storm Not Activating
- Check all references in StormController Inspector
- Ensure ParticleSystem is assigned to `rainSystem`
- Verify RainOverlayController is assigned
- Check that WindZone and LightningController are assigned

### Wind Not Affecting Objects
- Ensure objects have WindAffectedObject script
- Check that materials use WindShader
- Verify WindZone is active and configured
- Check shader property names match in WindAffectedObject

### Performance Issues
- Reduce particle count in rain system
- Limit number of wind-affected objects
- Use LOD (Level of Detail) for distant objects
- Consider disabling storm effects on low-end devices

## 📋 Complete Example Setup

```csharp
// Example: Trigger storm from another script
public class WeatherTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StormController.Instance.ActivateStorm();
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StormController.Instance.DeactivateStorm();
        }
    }
}
```

## 🎯 Integration with Other Systems

### With Time System
```csharp
// In TimeManager or similar
public void TriggerStormEvent()
{
    StormController.Instance.ActivateStorm();
    // Advance time or trigger other events
}
```

### With Event System
```csharp
// In EventObject
public void OnInteract()
{
    if (eventName == "Storm")
    {
        StormController.Instance.ActivateStorm();
    }
}
```

This storm system provides a complete weather simulation that can be easily integrated into your game's event system and time progression mechanics. 