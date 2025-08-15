# TV Flicker Light System Setup Guide

This guide explains how to set up and use the TV flicker light system to create realistic TV flickering effects in the dark.

## Overview

The TV flicker system consists of two main scripts:
- `TVFlickerLight.cs` - The main script that handles the flickering effect
- `TVFlickerSetupExample.cs` - An example script showing how to control the system

## Quick Setup

### 1. Basic Setup

1. **Add the TVFlickerLight script to your TV GameObject:**
   - Select your TV GameObject in the scene
   - Add Component → Scripts → TVFlickerLight

2. **Configure the light:**
   - The script will automatically create a Light component if none exists
   - Adjust the light settings in the inspector:
     - **Type**: Point (recommended for TV glow)
     - **Range**: 3-5 units (adjust based on your scene)
     - **Intensity**: 0 (will be controlled by the script)

3. **Set up flicker patterns:**
   - Right-click on the TVFlickerLight component in the inspector
   - Select "Create Default Patterns" from the context menu
   - This creates three preset patterns: Modern TV, Old CRT TV, and Faulty TV

### 2. Advanced Setup

#### TV Screen Material (Optional)
For enhanced realism, you can make the TV screen glow:

1. **Assign the TV screen material:**
   - Drag your TV screen material to the "TV Screen Material" field
   - Set the "Emission Property Name" (usually "_EmissionColor")

2. **Enable emission on the material:**
   - Select the TV screen material
   - Enable "Emission" in the material settings
   - The script will automatically control the emission color

#### Time Integration
The system integrates with your existing TimeManager:

1. **Enable time-based flickering:**
   - Check "Only Flicker At Night"
   - Set "Start Flickering Hour" (e.g., 18 for 6 PM)
   - Set "Stop Flickering Hour" (e.g., 6 for 6 AM)

## Flicker Patterns

### Default Patterns

1. **Modern TV**
   - Subtle, stable flickering
   - Low intensity variation
   - Cool color temperature (6500K)
   - Minimal power fluctuations

2. **Old CRT TV**
   - More pronounced flickering
   - Higher intensity variation
   - Warmer color temperature (5500K)
   - Occasional power fluctuations

3. **Faulty TV**
   - Aggressive flickering
   - High randomness
   - Warm color temperature (4500K)
   - Frequent power fluctuations

### Custom Patterns

You can create custom patterns by modifying the pattern settings:

- **Base Intensity**: Overall brightness of the light
- **Flicker Intensity**: How much the light varies
- **Flicker Speed**: How fast the flickering occurs
- **Flicker Randomness**: How unpredictable the flickering is
- **Power Fluctuation Chance**: How often power fluctuations occur
- **Color Temperature**: Warmth/coolness of the light color

## Usage Examples

### Basic Control

```csharp
// Get reference to the TV flicker component
TVFlickerLight tvFlicker = FindObjectOfType<TVFlickerLight>();

// Toggle flickering on/off
tvFlicker.ToggleFlicker();

// Change to a specific pattern
tvFlicker.SetFlickerPattern(1); // Old CRT TV

// Start/stop manually
tvFlicker.StartFlickering();
tvFlicker.StopFlickering();
```

### Integration with TimeManager

```csharp
// Subscribe to time changes
TimeManager.Instance.OnTimeChanged += OnTimeChanged;

void OnTimeChanged(int newTime)
{
    int hour = (newTime / 60) % 24;
    
    if (hour >= 22 || hour < 6)
    {
        // Late night - spooky effect
        tvFlicker.SetFlickerPattern(2);
    }
    else if (hour >= 18)
    {
        // Evening - nostalgic effect
        tvFlicker.SetFlickerPattern(1);
    }
    else
    {
        // Daytime - modern effect
        tvFlicker.SetFlickerPattern(0);
    }
}
```

### Player Proximity Effects

```csharp
void OnPlayerNearby(bool isNearby)
{
    if (isNearby)
    {
        // Increase flicker intensity when player is close
        var pattern = tvFlicker.flickerPatterns[tvFlicker.currentPatternIndex];
        pattern.flickerIntensity = Mathf.Min(pattern.flickerIntensity * 1.5f, 1f);
    }
    else
    {
        // Return to normal intensity
        var pattern = tvFlicker.flickerPatterns[tvFlicker.currentPatternIndex];
        pattern.flickerIntensity = Mathf.Max(pattern.flickerIntensity / 1.5f, 0.1f);
    }
}
```

## Keyboard Controls (Example Script)

When using the `TVFlickerSetupExample` script:

- **1, 2, 3**: Switch between flicker patterns
- **T**: Toggle flickering on/off
- **Auto Pattern Change**: Automatically cycles through patterns

## Performance Considerations

1. **Light Count**: Keep the number of flickering lights reasonable
2. **Update Frequency**: The script updates every frame, but this is lightweight
3. **Material Updates**: TV screen material updates are minimal
4. **Coroutines**: Uses coroutines for smooth transitions

## Troubleshooting

### Light Not Flickering
- Check if "Flicker Enabled" is checked
- Verify the time settings if "Only Flicker At Night" is enabled
- Ensure the Light component is assigned

### Screen Not Glowing
- Verify the TV screen material is assigned
- Check that the emission property name is correct
- Ensure the material has emission enabled

### Performance Issues
- Reduce the number of flickering lights
- Lower the flicker speed
- Disable smooth transitions if needed

### Integration Issues
- Ensure TimeManager exists in the scene
- Check that the time format matches (minutes since midnight)
- Verify event subscriptions are properly set up

## Advanced Features

### Custom Animation Curves
You can modify the flicker curve in the `SetupFlickerCurve()` method for different effects.

### Color Temperature
The system includes realistic color temperature conversion for authentic TV light colors.

### Power Fluctuations
Simulates old TV power issues with realistic timing and color variations.

### Smooth Transitions
Enables smooth transitions between patterns and when turning off the effect.

## Best Practices

1. **Test in Dark Scenes**: The effect is most visible in dark environments
2. **Adjust Light Range**: Match the light range to your scene scale
3. **Use Appropriate Patterns**: Choose patterns that match your game's atmosphere
4. **Consider Performance**: Don't use too many flickering lights simultaneously
5. **Test with Audio**: Consider adding TV static audio for enhanced realism

## Example Scene Setup

1. Create a dark room scene
2. Add a TV GameObject with the TVFlickerLight script
3. Position a camera to view the TV
4. Set the time to evening/night
5. Play the scene and observe the flickering effect

The TV flicker system provides a realistic and atmospheric lighting effect that can enhance the mood of your scenes, especially in horror or atmospheric games. 