# Water Gun Particle Collision Setup Guide

This guide walks you through setting up particle collision detection for the water gun to hit stains.

## 🎯 Step 1: Create Water Gun Structure

### 1.1 Create Water Gun Parent
1. Create an empty GameObject → Name it "WaterGun"
2. Add **WaterGunController** script to this GameObject
3. This will be your `waterGunParent` reference

### 1.2 Create Particle System Children
1. Create child GameObject → Name it "MainSpray"
2. Add **Particle System** component
3. Create another child → Name it "MistSpray" (optional)
4. Add **Particle System** component to this too

## 🔧 Step 2: Configure Particle System Collision

### 2.1 Main Particle System Settings
Select the **MainSpray** GameObject and configure the Particle System:

**Main Module:**
- Duration: 0 (continuous)
- Start Lifetime: 1-2 seconds
- Start Speed: 10-20
- Start Size: 0.1-0.3
- Start Color: Blue/cyan for water
- Simulation Space: World

**Emission Module:**
- Rate over Time: 100-200
- Bursts: None

**Shape Module:**
- Shape: Cone
- Angle: 15-30 degrees
- Radius: 0.1-0.3

**Collision Module (CRITICAL):**
- ✅ **Collision Enabled**: Check this
- **Type**: World
- **Collision Quality**: High
- **Collision Mode**: 3D
- ✅ **Send Collision Messages**: Check this
- **Collision Layers**: Set to include your stain layer
- **Collision Force**: 0 (we don't need physics force)
- **Collision Dampen**: 0.5
- **Collision Bounce**: 0.1
- **Collision Lifetime Loss**: 0.1

### 2.2 Collision Layer Setup
1. Go to **Edit → Project Settings → Tags and Layers**
2. Create a new layer: "Stain" (e.g., Layer 11)
3. Assign this layer to your stain prefab
4. In the particle system collision settings, set **Collision Layers** to include the Stain layer

## 🎮 Step 3: Assign References

### 3.1 WaterGunController Setup
1. Select the **WaterGun** GameObject
2. In **WaterGunController** component:
   - **Water Gun Parent**: Assign the WaterGun GameObject
   - **Water Spray**: Assign the MainSpray particle system
   - **Stain Layer Mask**: Set to include the Stain layer
   - **Enable Collision Debug**: Check this for testing

### 3.2 ElephantWashManager Setup
1. Select your **ElephantWashManager** GameObject
2. Assign the **Water Gun** reference to the WaterGun GameObject
3. Assign the **Water Gun Controller** reference

## 🧪 Step 4: Test the Setup

### 4.1 Test Particle Systems
1. **Select WaterGunController** in inspector
2. **Right-click** → "Print Debug Info"
3. **Check Console** for:
   - ✅ Particle systems found
   - ✅ Collision enabled
   - ✅ Correct collision layers

### 4.2 Test Collision Detection
1. **Start the wash mini-game** (Key 5)
2. **Spray water at stains** (Left mouse button)
3. **Watch Console** for collision messages
4. **Check gizmo visualization** in Scene view

## 🔍 Step 5: Troubleshooting

### Common Issues and Solutions:

#### **Issue: No collision detected**
**Solutions:**
1. **Check collision enabled** on particle system
2. **Verify stain layer** matches collision layers
3. **Ensure stain has collider** component
4. **Check "Send Collision Messages"** is enabled

#### **Issue: Particles not playing**
**Solutions:**
1. **Check water gun parent** is active
2. **Verify particle systems** are children of parent
3. **Ensure emission rate** is not zero
4. **Check particle lifetime** is reasonable

#### **Issue: Collision detected but no damage**
**Solutions:**
1. **Verify stain has Stain component**
2. **Check stain health** is greater than 0
3. **Ensure stain is active** in hierarchy
4. **Test manual stain cleaning**

#### **Issue: Gizmo not showing**
**Solutions:**
1. **Enable collision debug** in WaterGunController
2. **Check Scene view** is selected
3. **Verify gizmos are enabled** in Scene view
4. **Ensure camera is moving** (gizmo updates with movement)

## 📋 Step 6: Advanced Settings

### 6.1 Multiple Particle Systems
If you have multiple particle systems (main spray + mist):
1. **Configure each separately** with collision settings
2. **Use same collision layers** for all systems
3. **Test each system** individually

### 6.2 Performance Optimization
1. **Limit particle count** (100-200 per system)
2. **Use appropriate collision quality** (High for testing, Medium for release)
3. **Disable collision debug** in release builds

### 6.3 Visual Effects
1. **Add splash effects** on collision
2. **Use sub-emitters** for impact particles
3. **Add audio effects** for spray sounds

## ✅ Step 7: Verification Checklist

Before testing, ensure:
- [ ] Particle system collision is enabled
- [ ] Collision layers include stain layer
- [ ] "Send Collision Messages" is checked
- [ ] Stain prefab has Stain component
- [ ] Stain prefab has Collider component
- [ ] Stain prefab is on correct layer
- [ ] Water gun parent is assigned
- [ ] Collision debug is enabled
- [ ] Particle systems are playing

## 🎯 Testing Commands

Use these context menu commands to test:
- **"Print Debug Info"** - Check system status
- **"Test Manual Collision"** - Test raycast detection
- **"Test Start Spray"** - Test particle systems
- **"Test Raycast Detection"** - Test alternative method

## 🔧 Quick Fix Commands

If something isn't working:
1. **Right-click WaterGunController** → "Print Debug Info"
2. **Check Console** for error messages
3. **Verify all references** are assigned
4. **Test with manual collision** first
5. **Enable debug logging** for detailed feedback

---

## 🎉 Setup Complete!

Your water gun should now properly detect collisions with stains. The particle systems will send collision messages to the WaterGunController, which will then damage the stains and trigger the cleaning process. 