# Elephant Teaser Animation Manager Setup Guide

## Overview
The `ElephantTeaserAnimationManager` script provides a complete animation system for the Elephant Teaser object. It handles input detection (pressing '4' and left mouse button) and manages four animation states: Idle, Take Out, Using, and Take Back.

## Setup Instructions

### 1. Add the Script to Your Elephant Teaser
1. Select your Elephant Teaser GameObject in the hierarchy
2. Add the `ElephantTeaserAnimationManager` component to it
3. The script will automatically require an Animator component
4. **Important**: The teaser GameObject will be deactivated by default

### 2. Configure the Animator
1. Make sure your Elephant Teaser has an Animator component
2. Create an Animator Controller for the teaser
3. Set up three animation states:
   - **TakeOut**: Animation for taking out the teaser
   - **Using**: Animation for using the teaser (click to trigger)
   - **TakeBack**: Animation for putting the teaser back

### 3. Configure Animation Parameters
In your Animator Controller, create these Bool parameters:
- `TakeOut` (Bool)
- `Using` (Bool)
- `TakeBack` (Bool)

### 4. Set Up Animation Transitions
Create transitions between states:
- **Any State → TakeOut** (when `TakeOut` is true)
- **Any State → Using** (when `Using` is true)
- **Any State → TakeBack** (when `TakeBack` is true)

### 5. Configure the Script
In the Inspector, you can customize:
- **Animation Parameters**: Update parameter names if they differ from defaults
- **State Names**: Update if your animation state names are different
- **Start Deactivated**: Whether the teaser should start disabled (default: true)
- **Deactivate After Take Back**: Whether to deactivate the teaser after take back animation (default: true)
- **Using Animation Duration**: How long the using animation plays (default: 2 seconds)

### 6. Connect to FirstPersonController
1. Select your player GameObject (the one with FirstPersonController)
2. In the FirstPersonController component, assign the ElephantTeaserAnimationManager to the "Teaser Manager" field
3. The input handling is now managed by the FirstPersonController (always active)

## How It Works

### Input Handling
- **Press 4**: Activates the teaser and starts "Take Out" animation
- **Press 4 again** (when taken out): Starts "Take Back" animation
- **Click Left Mouse Button** (when taken out): Plays "Using" animation for a set duration
- **Using animation** automatically stops after the configured duration

### Animation Flow
```
Deactivated → Press 4 → Take Out → Default State (Taken Out)
                ↓
            Click Left Mouse → Using (Duration) → Default State (Taken Out)
                ↓
            Press 4 → Take Back → Default State → Deactivated
```

### State Management
The script tracks five states:
- `isActivated`: Whether the teaser GameObject is active/enabled
- `isTakingOut`: Teaser is playing take out animation
- `isUsing`: Teaser is playing using animation
- `isTakingBack`: Teaser is playing take back animation
- `isTakenOut`: Whether the teaser has been taken out

## Advanced Features

### External Control
You can control the teaser from other scripts:
```csharp
ElephantTeaserAnimationManager teaserManager = GetComponent<ElephantTeaserAnimationManager>();

// Force specific states (will activate if needed)
teaserManager.ForceTakeOut();
teaserManager.ForceUsing();
teaserManager.ForceTakeBack();
teaserManager.ForceIdle();

// Activation control
teaserManager.ActivateTeaser();
teaserManager.DeactivateTeaser();

// Check current state
if (teaserManager.IsActivated) { /* ... */ }
if (teaserManager.IsTakenOut) { /* ... */ }
if (teaserManager.IsUsing) { /* ... */ }
if (teaserManager.IsTakingOut) { /* ... */ }
```

### Animation Events
You can add animation events to your clips that call:
- `OnTakeOutAnimationComplete()`: Called when take out animation finishes
- `OnTakeBackAnimationComplete()`: Called when take back animation finishes

## Troubleshooting

### Common Issues:
1. **No Animator Found**: Make sure your teaser has an Animator component
2. **Animations Not Playing**: Check that your Animator Controller has the correct parameters and transitions
3. **Wrong Key**: The script uses key 4 by default (handled by FirstPersonController)
4. **Parameter Names**: Ensure the parameter names in your Animator Controller match the ones in the script
5. **Mouse Input**: Make sure the teaser is taken out before left mouse click will work
6. **Using Duration**: Adjust the "Using Animation Duration" if the animation is too short or long

### Debug Information
The script outputs debug messages to the console:
- "Elephant Teaser: Activated"
- "Elephant Teaser: Starting take out animation"
- "Elephant Teaser: Starting using animation"
- "Elephant Teaser: Starting take back animation"
- "Elephant Teaser: Deactivated"

## Example Animator Controller Setup

```
Parameters:
- TakeOut (Bool)
- Using (Bool)
- TakeBack (Bool)

States:
- TakeOut
- Using
- TakeBack

Transitions:
- Any State → TakeOut (Condition: TakeOut = true)
- Any State → Using (Condition: Using = true)
- Any State → TakeBack (Condition: TakeBack = true)
```

This setup will give you a fully functional elephant teaser animation system that responds to player input and manages animation states automatically. 