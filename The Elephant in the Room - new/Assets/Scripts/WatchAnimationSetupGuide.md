# Watch Animation Manager Setup Guide

## Overview
The `WatchAnimationManager` script provides a complete animation system for your watch object. It handles input detection (pressing '3') and manages three animation states: Idle, Check Time, and Put Down.

## Setup Instructions

### 1. Add the Script to Your Watch
1. Select your watch GameObject in the hierarchy
2. Add the `WatchAnimationManager` component to it
3. The script will automatically require an Animator component
4. **Important**: The watch GameObject will be deactivated by default

### 2. Configure the Animator
1. Make sure your watch has an Animator component
2. Create an Animator Controller for the watch
3. Set up three animation states:
   - **Idle**: The default idle animation
   - **CheckTime**: Animation for checking the time
   - **PutDown**: Animation for putting the watch down

### 3. Configure Animation Parameters
In your Animator Controller, create these Bool parameters:
- `Idle` (Bool)
- `CheckTime` (Bool) 
- `PutDown` (Bool)

### 4. Set Up Animation Transitions
Create transitions between states:
- **Any State → Idle** (when `Idle` is true)
- **Any State → CheckTime** (when `CheckTime` is true)
- **Any State → PutDown** (when `PutDown` is true)

### 5. Configure the Script
In the Inspector, you can customize:
- **Animation Parameters**: Update parameter names if they differ from defaults
- **State Names**: Update if your animation state names are different
- **Start Deactivated**: Whether the watch should start disabled (default: true)
- **Deactivate After Put Down**: Whether to deactivate the watch after put down animation (default: true)

### 6. Connect to FirstPersonController
1. Select your player GameObject (the one with FirstPersonController)
2. In the FirstPersonController component, assign the WatchAnimationManager to the "Watch Manager" field
3. The input handling is now managed by the FirstPersonController (always active)

### 7. Set Up Time Synchronization (Optional)
1. **Assign Watch Hands**: Drag the hour hand, minute hand, and second hand (optional) transforms to the respective fields
2. **Auto Sync**: Enable "Auto Sync Time" to automatically update hands with game time
3. **Rotation Direction**: Set "Clockwise Rotation" to true for standard clock rotation
4. **Manual Sync**: Use `SyncWatchTime()` or `SyncWatchTime(hour, minute, second)` from other scripts
5. **Game Time**: The watch automatically syncs with TimeManager.Instance.GetCurrentTime()

## How It Works

### Activation Behavior
- The watch starts **deactivated** by default (invisible/disabled)
- Press **3** to **activate** the watch and start "Check Time" animation
- Second press: Start "Put Down" animation  
- Third press: Return to "Idle" state and **deactivate** the watch
- The cycle repeats: Deactivated → Activated (Check Time) → Put Down → Deactivated
- **Note**: Input is handled by FirstPersonController since the watch can be deactivated

### Animation Flow
```
Idle → CheckTime → PutDown → Idle (cycle repeats)
```

### State Management
The script tracks four states:
- `isActivated`: Whether the watch GameObject is active/enabled
- `isInIdle`: Watch is in idle state
- `isCheckingTime`: Watch is playing check time animation
- `isPuttingDown`: Watch is playing put down animation

## Advanced Features

### External Control
You can control the watch from other scripts:
```csharp
WatchAnimationManager watchManager = GetComponent<WatchAnimationManager>();

// Force specific states (will activate if needed)
watchManager.ForceCheckTime();
watchManager.ForcePutDown();
watchManager.ForceIdle();

// Activation control
watchManager.ActivateWatch();
watchManager.DeactivateWatch();

// Time synchronization
watchManager.SyncWatchTime(); // Sync to current game time
watchManager.SyncWatchTime(14, 30, 45); // Sync to 2:30:45 PM
watchManager.SyncWatchTime(870); // Sync to 14:30 (870 minutes since midnight)

// Check current state
if (watchManager.IsActivated) { /* ... */ }
if (watchManager.IsCheckingTime) { /* ... */ }
```

### Animation Events
You can add animation events to your clips that call:
- `OnCheckTimeAnimationComplete()`: Called when check time animation finishes
- `OnPutDownAnimationComplete()`: Called when put down animation finishes

## Troubleshooting

### Common Issues:
1. **No Animator Found**: Make sure your watch has an Animator component
2. **Animations Not Playing**: Check that your Animator Controller has the correct parameters and transitions
3. **Wrong Key**: Change the `triggerKey` in the inspector if you want a different key
4. **Parameter Names**: Ensure the parameter names in your Animator Controller match the ones in the script

### Debug Information
The script outputs debug messages to the console:
- "Watch: Starting check time animation"
- "Watch: Starting put down animation" 
- "Watch: Returning to idle state"

## Example Animator Controller Setup

```
Parameters:
- Idle (Bool)
- CheckTime (Bool)
- PutDown (Bool)

States:
- Idle (Default State)
- CheckTime
- PutDown

Transitions:
- Any State → Idle (Condition: Idle = true)
- Any State → CheckTime (Condition: CheckTime = true)
- Any State → PutDown (Condition: PutDown = true)
```

This setup will give you a fully functional watch animation system that responds to player input and manages animation states automatically. 