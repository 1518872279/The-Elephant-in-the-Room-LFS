# Elephant Proximity Animator Setup Guide

## Overview
The `ElephantProximityAnimator` script makes the elephant play one animation when players stay within proximity for a fixed time range. It triggers exactly once per proximity session, making it predictable and controlled.

## Setup Instructions

### 1. Add the Script to Your Elephant
1. Select your Elephant GameObject in the hierarchy
2. Add the `ElephantProximityAnimator` component to it
3. The script will automatically require an Animator component

### 2. Configure Animation Triggers
In the Inspector, add animation trigger names to the `Random Animation Triggers` list:

**Common Elephant Animation Triggers:**
- `isWalking` - Walking animation
- `isRunning` - Running animation  
- `isDrinking` - Drinking animation
- `isAttacking` - Attack animation
- `BreakfastReaction` - Reaction to breakfast event
- `GarbageCleanupReaction` - Reaction to cleanup event
- `ElephantWashReaction` - Reaction to wash event

**Custom Animation Triggers:**
Add any custom animation triggers you've created for your elephant.

### 3. Configure Proximity Settings
- **Proximity Distance**: How close the player needs to be (default: 5 meters)
- **Animation Time Range**: Fixed time the player must stay in range to trigger animation (default: 5 seconds)

### 4. Configure Animation Timing
- **Min Time Between Animations**: Minimum wait time before next animation can play (default: 2 seconds)

### 5. Player Detection Settings
- **Player Tag**: Tag of your player GameObject (default: "Player")
- **Player Layer**: Layer mask for player detection (default: Everything)

### 6. Debug Options
- **Show Debug**: Enable console logging for debugging

## How It Works

1. **Proximity Detection**: Continuously checks distance to player
2. **Time Tracking**: When player enters range, starts counting time
3. **Single Trigger**: After the fixed time range, plays one random animation
4. **Reset on Exit**: When player leaves range, resets the trigger state
5. **Cooldown**: Respects minimum time between animations

## Example Configuration

```
Proximity Distance: 5
Animation Time Range: 5
Min Time Between Animations: 2

Random Animation Triggers:
- isWalking
- isDrinking
- BreakfastReaction
- GarbageCleanupReaction
```

## Behavior Flow

1. **Player enters range** → Timer starts
2. **Player stays in range for X seconds** → Animation triggers (once)
3. **Player leaves range** → Timer resets
4. **Player re-enters range** → New timer starts, can trigger again

## Integration with Existing Systems

This script works alongside your existing elephant systems:
- **ElephantStateController**: For event-based reactions
- **ElephantBehaviorController**: For event-triggered animations
- **ElephantTeaserAnimationManager**: For teaser interactions

## Troubleshooting

**No animations playing:**
- Check that animation triggers are correctly named
- Verify the Animator component has the triggers defined
- Ensure player has the correct tag
- Make sure player stays in range for the full time duration

**Animations playing too frequently:**
- Increase `Min Time Between Animations`
- Increase `Animation Time Range`

**Animations not stopping:**
- Check that animations have proper exit conditions in Animator
- Verify animation lengths match timing settings

## Public Methods

You can also trigger animations manually:
```csharp
// Trigger random animation
elephantProximityAnimator.TriggerRandomAnimation();

// Trigger specific animation
elephantProximityAnimator.TriggerAnimation("isWalking");

// Reset trigger state (useful for testing)
elephantProximityAnimator.ResetTriggerState();
```

## Key Changes from Previous Version

- **Removed random chance system** - Now uses fixed time range
- **Single trigger per proximity session** - More predictable behavior
- **Simplified timing** - Only one time setting to configure
- **Reset on range exit** - Allows re-triggering when player returns 