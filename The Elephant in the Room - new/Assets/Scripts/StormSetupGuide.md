# Storm Visual Effects Setup Guide

This guide provides step-by-step instructions for implementing the complete storm visual effects system in your Unity project.

## 1. Rain Particle System Setup

### 1.1 Create Rain System
1. Create an empty GameObject named **RainSystem**
2. Add a **Particle System** component
3. Configure the following settings:

**Shape:**
- Shape: Box
- Size: X=50, Y=1, Z=50 (adjust to cover your play area)

**Emission:**
- Rate over Time: 2000
- Bursts: None

**Start Lifetime:**
- Start Lifetime: 2-3 seconds

**Start Speed:**
- Start Speed: 20 (downward direction)

**Start Size:**
- Start Size: X=0.02, Y=0.5 (elongated raindrop shape)

**Velocity over Lifetime:**
- Add X and Z velocity with small noise for drift effect

**Collision:**
- Enable Collision
- Collides With: Terrain/Floor layer
- Type: World
- Send Collision Messages: Enabled

### 1.2 Rain Splash Sub-Emitter
1. In the Collision module, add a Sub-Emitter
2. Create a small particle system for splashes
3. Configure splash particles to be short-lived and small

## 2. Screen-Space Rain Overlay

### 2.1 UI Setup
1. In your main Canvas (Screen Space - Overlay), add a full-screen **RawImage** named **RainOverlay**
2. Assign a raindrop-streak texture to the RawImage
3. Set the RawImage to cover the entire screen
4. Add the **RainOverlayController** script to the RainOverlay GameObject
5. Assign the RawImage to the `overlay` field in the script

### 2.2 Raindrop Texture
- Create or download a raindrop streak texture
- The texture should show elongated raindrops
- Set texture import settings to Repeat

## 3. Wind Zone & Foliage

### 3.1 Wind Zone Setup
1. In the Hierarchy, create **WindZone** (GameObject > 3D Object > Wind Zone)
2. Configure settings:
   - Mode: Directional
   - Main: 1.5
   - Turbulence: 0.8
   - Pulse Magnitude: 1
   - Pulse Frequency: 0.3

### 3.2 Foliage Setup
1. For each foliage object (trees, grass, etc.):
   - Add the **WindAffectedObject** script
   - Assign materials that use the **WindShader**
   - Configure wind sensitivity and max bend angle

### 3.3 Wind Shader Material
1. Create a new material using the **Custom/WindShader**
2. Assign this material to foliage objects
3. Adjust wind parameters in the material inspector

## 4. Lightning System

### 4.1 Lightning Controller Setup
1. Create an empty GameObject named **LightningController**
2. Add the **LightningController** script
3. Assign references:
   - Main Light: Your directional light
   - Flash Image: A full-screen white Image in your UI

### 4.2 Flash Image Setup
1. In your UI Canvas, create a full-screen **Image** named **FlashImage**
2. Set color to white with alpha = 0
3. Position it above other UI elements
4. Assign to the LightningController script

## 5. Lightning System

### 5.1 Lightning Controller Setup
1. Create an empty GameObject named **LightningController**
2. Add the **LightningController** script
3. Assign references:
   - Main Light: Your directional light
   - Flash Image: A full-screen white Image in your UI

### 5.2 Flash Image Setup
1. In your UI Canvas, create a full-screen **Image** named **FlashImage**
2. Set color to white with alpha = 0
3. Position it above other UI elements
4. Assign to the LightningController script

## 6. Audio System

### 6.1 Audio Sources Setup
1. Create an empty GameObject named **StormAudio**
2. Add three **AudioSource** components:
   - Rain Audio: Loop rain sound effects
   - Wind Audio: Loop wind sound effects  
   - Thunder Audio: Thunder sound effects (not looped)

### 6.2 Audio Configuration
- Set Spatial Blend = 0 for stereo audio
- Configure appropriate volume levels
- Ensure rain and wind audio are set to loop

## 7. Wet Lens Post-Processing

### 7.1 Wet Lens Shader Setup
1. Create a material using the **Custom/WetLensShader**
2. Assign a raindrop normal map texture
3. Configure distortion strength and wetness parameters

### 7.2 Post-Processing Integration
1. Add a URP Volume to your scene
2. Create a custom post-processing effect using the wet lens shader
3. Or apply the material to a full-screen quad in front of the camera

## 8. Storm Controller Integration

### 8.1 Main Controller Setup
1. Create an empty GameObject named **StormController**
2. Add the **StormController** script
3. Assign all references:
   - Rain System: Your RainSystem GameObject (with ParticleSystem component)
   - Rain Overlay: Your RainOverlayController script
   - Wind Zone: Your WindZone component
   - Lightning Controller: Your LightningController script
   - Storm Audio: Your AudioSource for rain/wind ambience

### 8.2 Storm Activation
1. **Code Activation**: Call `StormController.Instance.ActivateStorm()` to begin the storm
2. **Code Deactivation**: Call `StormController.Instance.DeactivateStorm()` to end the storm
3. **Inspector Buttons**: Use the "Activate Storm" and "Deactivate Storm" buttons in the inspector for manual testing
4. The controller will manage all subsystems automatically:
   - Rain particles: Activates GameObject and starts ParticleSystem
   - UI overlay: Enables both GameObject and RawImage component
   - Wind and lightning: Activates respective GameObjects and components
   - Audio: Starts/stops AudioSource playback

## 9. Manual Testing

### 9.1 Inspector Buttons
1. Select the **StormController** GameObject in the hierarchy
2. In the inspector, you'll see a "Manual Control" section with two buttons:
   - **Activate Storm**: Starts the storm system
   - **Deactivate Storm**: Stops the storm system
3. These buttons work in both Play mode and Edit mode for testing

### 9.2 Testing Workflow
1. Enter Play mode
2. Select the StormController GameObject
3. Click "Activate Storm" to test the complete storm system
4. Verify all subsystems are working:
   - Rain particles are visible and falling
   - Rain overlay is scrolling
   - Wind is affecting foliage
   - Lightning flashes are occurring
   - Audio is playing
5. Click "Deactivate Storm" to stop all effects

## 10. Performance Optimization

### 10.1 Rain System Optimization
- Limit rain particles to 50m around the camera
- Use LOD system for particle quality at distance
- Pool splash sub-emitters

### 10.2 Wind Effects Optimization
- Only apply wind effects to visible foliage
- Use instancing for similar objects
- Limit wind calculations to nearby objects

### 10.3 Audio Optimization
- Use audio pooling for thunder effects
- Implement distance-based audio culling
- Use audio compression for better performance

## 11. Testing and Tuning

### 11.1 Visual Testing
1. Test rain density and visibility
2. Verify wind effects on foliage
3. Check lightning flash timing and intensity
4. Ensure rain overlay scrolling works smoothly

### 11.2 Audio Testing
1. Balance rain, wind, and thunder volumes
2. Test audio looping and transitions
3. Verify spatial audio positioning

### 11.3 Performance Testing
1. Monitor frame rate during storm
2. Check memory usage
3. Test on target platforms

## 11. Integration with Time System

### 11.1 Storm Events
1. Add storm events to your TimeManager
2. Create EventObjects for storm triggers
3. Integrate with your existing event system

### 11.2 Weather Transitions
1. Implement gradual weather changes
2. Add transition effects between weather states
3. Coordinate with day/night cycle

## 13. Troubleshooting

### Common Issues:
1. **Rain not visible**: Check particle system settings and camera culling
2. **Wind not affecting objects**: Verify WindAffectedObject script and material assignments
3. **Lightning too bright**: Adjust flash intensity and duration
4. **Audio not playing**: Check AudioSource settings and volume levels
5. **Rain overlay not scrolling**: Verify RainOverlayController is enabled and overlay reference is assigned
6. **Rain particles not starting**: Ensure RainSystem GameObject has ParticleSystem component and is assigned in StormController
7. **Performance issues**: Reduce particle counts and optimize shaders

### Debug Tools:
- Use the StormController inspector to manually control storm intensity
- Enable debug logging in scripts for troubleshooting
- Use Unity Profiler to identify performance bottlenecks

This setup provides a comprehensive storm system with rain, wind, lightning, and audio effects that can be easily integrated into your existing Unity project. 