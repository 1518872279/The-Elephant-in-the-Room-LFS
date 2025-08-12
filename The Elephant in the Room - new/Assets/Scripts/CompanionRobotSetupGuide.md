# Companion Robot Setup Guide

## Overview
The companion robot system allows players to purchase a robot that increases elephant stability but decreases happiness. After purchase, players can press key 6 to spawn the robot.

## Setup Instructions

### 1. Create CompanionRobotManager GameObject
1. Create an empty GameObject in your scene
2. Name it "CompanionRobotManager"
3. Add the `CompanionRobotManager` component to it

### 2. Configure CompanionRobotManager
In the Inspector, set up the following:

**Robot Settings:**
- **Companion Robot Prefab**: Drag the "Companion Robot" prefab from `Assets/Custom Models/Robot/`
- **Spawn Position**: Create an empty GameObject as a child and assign it as the spawn position

**Elephant Effects:**
- **Stability Increase**: Set to 15 (or your preferred value)
- **Happiness Decrease**: Set to 10 (or your preferred value)

**Animation:**
- **Elephant Animator**: Assign the elephant's Animator component

### 3. Configure FirstPersonController
1. Select your player GameObject with FirstPersonController
2. In the Inspector, find the "Companion Robot Control" section
3. Assign the CompanionRobotManager to the "Companion Robot Manager" field

### 4. Set Up Elephant Animation
1. In your elephant's Animator Controller, create a new trigger parameter called "isSad"
2. Create an animation state for the sad animation
3. Set up a transition from any state to the sad state, triggered by "isSad"

### 5. Configure Goods System (Optional)
The companion robot is automatically added to the goods system. If you want to customize:

1. In GoodsManager, you can modify the companion robot goods data
2. Set the delivery prefab to the companion robot prefab if you want it delivered via the goods system

## How It Works

1. **Purchase**: Player buys "Companion Robot" from the goods system
2. **Spawn**: Player presses key 6 to spawn the robot
3. **Effects**: 
   - Elephant stability increases by 15 points
   - Elephant happiness decreases by 10 points
   - Elephant plays "isSad" animation
4. **One-time Use**: Robot can only be spawned once per purchase

## Testing

### Test Robot Spawning
1. In Play mode, press key 6
2. Check console for spawn messages
3. Verify robot appears at spawn position
4. Check elephant stats and animation

### Test Goods Purchase
1. Use the goods system to purchase "Companion Robot"
2. Verify the robot becomes available for spawning
3. Test the spawn functionality

## Troubleshooting

**Robot doesn't spawn:**
- Check if CompanionRobotManager is assigned to FirstPersonController
- Verify spawn position is set
- Check console for error messages

**Elephant effects don't work:**
- Ensure ElephantStateController is in the scene
- Check if elephant animator is assigned
- Verify "isSad" trigger exists in animator

**Goods not available:**
- Check if GoodsManager is properly initialized
- Verify companion robot is in the goods list
- Check console for goods-related messages

## Customization

### Modify Effects
- Change `stabilityIncrease` and `happinessDecrease` values in CompanionRobotManager
- Adjust the spawn position by moving the spawn transform

### Add More Features
- Add sound effects when robot spawns
- Create particle effects for the spawn
- Add UI notifications
- Implement robot removal functionality

### Animation Integration
- Add more animation triggers for different robot states
- Create robot-specific animations
- Integrate with existing elephant behavior system 