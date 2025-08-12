# Camera Orbit Recorder with AVProMovieCapture

A Unity script that allows the camera to orbit around a specific game object with customizable speed, angle, and **professional video recording** using AVProMovieCapture.

## Features

- **Smooth Camera Orbiting**: Rotate the camera around any target object with customizable speed and smoothness
- **Professional Video Recording**: High-quality video capture using AVProMovieCapture with H.264 encoding
- **Customizable Parameters**: Distance, height offset, rotation speed, starting angle, and maximum angle
- **Multiple Video Quality Presets**: From 720p to 4K with various frame rates and quality settings
- **Screenshot Recording**: Automatic screenshot capture as backup or alternative
- **Multiple Orbit Presets**: Pre-configured settings for different recording styles
- **Runtime Controls**: Start/stop orbiting and recording with keyboard shortcuts
- **Debug UI**: On-screen information display for monitoring orbit and recording status
- **Easy Integration**: Simple setup with automatic target detection
- **Audio Support**: Optional audio recording with AAC encoding

## Quick Setup

### Method 1: Using CameraOrbitExample (Recommended)

1. **Add the script to your camera**:
   - Select your main camera in the scene
   - Add the `CameraOrbitExample` component
   - Configure the settings in the inspector

2. **Set up the target**:
   - Drag your target object to the "Target Object" field, or
   - Leave it empty to auto-detect common targets (Player, Elephant, etc.)

3. **Choose presets**:
   - Select orbit preset (Standard, Cinematic, Gameplay, Showcase, etc.)
   - Select video quality preset (Low, Medium, High, Ultra)

4. **Start recording**:
   - Press **R** to toggle orbiting
   - Press **F9** to take a screenshot
   - Press **F10** to toggle video recording
   - Press **F11** to toggle screenshot recording

### Method 2: Using CameraOrbitRecorder Directly

1. **Add the script to your camera**:
   - Select your main camera in the scene
   - Add the `CameraOrbitRecorder` component

2. **Configure the settings**:
   - Set the target object in "Target Settings"
   - Adjust orbit parameters (distance, height, speed, etc.)
   - Configure video recording settings

3. **Use the controls**:
   - **R**: Toggle orbiting
   - **F9**: Take screenshot
   - **F10**: Toggle video recording
   - **F11**: Toggle screenshot recording

## Configuration Options

### Orbit Settings

- **Target Object**: The GameObject to orbit around
- **Orbit Distance**: Distance from the target (in units)
- **Height Offset**: Vertical offset from the target (in units)
- **Rotation Speed**: Degrees per second (positive = clockwise, negative = counter-clockwise)
- **Starting Angle**: Initial angle in degrees (0 = right, 90 = forward, 180 = left, 270 = back)
- **Maximum Angle**: Total rotation angle (0 = continuous rotation)
- **Field of View**: Camera field of view
- **Smoothness**: Camera movement smoothness (lower = smoother)

### Video Recording Settings

- **Enable Video Recording**: Use AVProMovieCapture for professional video recording
- **Video Resolution**: Width and height (720p, 1080p, 4K, etc.)
- **Frame Rate**: Video frame rate (24, 30, 60 fps)
- **Video Quality**: H.264 encoding quality (0-100)
- **Include Audio**: Record audio with AAC encoding
- **Output Folder**: Directory for saved videos

### Screenshot Recording Settings

- **Enable Screenshot Recording**: Automatic screenshot capture as backup
- **Screenshot Interval**: Time between screenshots (in seconds)
- **Output Folder**: Directory for saved screenshots

### Controls

- **Toggle Orbit Key**: Start/stop orbiting (default: R)
- **Screenshot Key**: Take a single screenshot (default: F9)
- **Toggle Video Recording Key**: Start/stop video recording (default: F10)
- **Toggle Screenshot Recording Key**: Start/stop screenshot recording (default: F11)

## Orbit Preset Configurations

### Standard
- Distance: 5 units, Height: 2 units, Speed: 30°/s
- Good for general gameplay recording

### Close Up
- Distance: 2 units, Height: 1 unit, Speed: 20°/s
- Perfect for detailed object showcases

### Wide Shot
- Distance: 10 units, Height: 3 units, Speed: 15°/s
- Great for environment overviews

### Cinematic
- Distance: 7 units, Height: 3 units, Speed: 15°/s, Smoothness: 1.0
- Ideal for cinematic sequences with smooth movement

### Gameplay
- Distance: 4 units, Height: 1.5 units, Speed: 45°/s
- Good for dynamic gameplay footage

### Showcase
- Distance: 6 units, Height: 2 units, Speed: 20°/s, Smoothness: 0.8
- Perfect for product showcases with ultra-smooth movement

### Slow Motion
- Distance: 6 units, Height: 2.5 units, Speed: 10°/s
- Ideal for slow, dramatic sequences

### Fast Motion
- Distance: 4 units, Height: 1.5 units, Speed: 60°/s
- Good for dynamic action shots

### Top Down
- Distance: 8 units, Height: 8 units, Speed: 25°/s
- Perfect for overview shots

### Low Angle
- Distance: 3 units, Height: -1 unit, Speed: 35°/s
- Creates dramatic low-angle shots

## Video Quality Presets

### Low
- Resolution: 1280x720 (720p)
- Frame Rate: 30 fps
- Quality: 60%
- Good for quick previews and testing

### Medium
- Resolution: 1920x1080 (1080p)
- Frame Rate: 30 fps
- Quality: 75%
- Perfect for most recording needs

### High
- Resolution: 1920x1080 (1080p)
- Frame Rate: 60 fps
- Quality: 85%
- Excellent for smooth gameplay footage

### Ultra
- Resolution: 3840x2160 (4K)
- Frame Rate: 60 fps
- Quality: 95%
- Professional quality for showcases

## Usage Examples

### Basic Orbiting with Video Recording
```csharp
// Get the orbit recorder component
CameraOrbitRecorder recorder = camera.GetComponent<CameraOrbitRecorder>();

// Set target and start orbiting with video recording
recorder.SetTarget(targetObject);
recorder.StartOrbit();
recorder.StartVideoRecording();
```

### Custom Video Configuration
```csharp
// Configure video settings
recorder.SetVideoResolution(1920, 1080);
recorder.SetVideoFrameRate(60);
recorder.SetVideoQuality(85);

// Configure orbit parameters
recorder.ConfigureOrbit(
    distance: 5f,      // Distance from target
    height: 2f,        // Height offset
    speed: 30f,        // Rotation speed
    startAngle: 0f,    // Starting angle
    maxAngle: 360f     // Maximum rotation angle
);
```

### Using Presets
```csharp
// Get the example component
CameraOrbitExample example = camera.GetComponent<CameraOrbitExample>();

// Apply cinematic preset with high quality video
example.SetPreset(CameraOrbitExample.OrbitPreset.Cinematic);
example.SetVideoQualityPreset(CameraOrbitExample.VideoQualityPreset.High);

// Start recording
example.RecordCinematic();
```

### Runtime Control
```csharp
// Change orbit speed during runtime
recorder.SetOrbitSpeed(45f);

// Adjust distance
recorder.SetOrbitDistance(8f);

// Change height
recorder.SetHeightOffset(3f);

// Change video quality
recorder.SetVideoQuality(90);
```

## AVProMovieCapture Integration

The script automatically integrates with AVProMovieCapture when available:

### Automatic Setup
- Automatically adds `CaptureFromCamera` component
- Configures H.264 video encoding with AAC audio
- Sets up proper output paths and file naming
- Handles video quality and resolution settings

### Video Codec Settings
- **Video Codec**: H.264 Software (compatible with most players)
- **Audio Codec**: AAC (high quality, widely supported)
- **Audio Sample Rate**: 48kHz
- **Audio Channels**: 2 (stereo)

### Output Files
- Videos are saved as MP4 files with timestamps
- File naming: `OrbitRecording_YYYY-MM-DD_HH-mm-ss.mp4`
- Location: `Recordings/` folder in your project

## Tips and Best Practices

### For Best Video Results
1. **Target Selection**: Choose a target that's roughly centered in your scene
2. **Distance**: Start with 5-8 units distance for most objects
3. **Speed**: Use 20-40°/s for smooth, cinematic movement
4. **Video Quality**: Use "High" preset for most recordings, "Ultra" for showcases
5. **Lighting**: Ensure good lighting on your target object
6. **Audio**: Enable audio recording for more professional results

### Performance Considerations
- Video recording is more resource-intensive than screenshots
- Higher resolutions and frame rates require more processing power
- Consider using "Medium" quality for longer recordings
- 4K recording requires significant hardware resources

### Troubleshooting
- **Camera not moving**: Check if target object is assigned
- **Jumpy movement**: Increase smoothness value
- **Poor video quality**: Ensure good lighting and increase video quality setting
- **Missing videos**: Check if AVProMovieCapture is properly installed
- **Audio issues**: Ensure AudioListener is present in the scene

## Integration with Existing Systems

### With FirstPersonController
The orbit recorder can work alongside your existing player controller:

```csharp
// Temporarily disable player input during recording
FirstPersonController playerController = FindObjectOfType<FirstPersonController>();
if (playerController != null)
{
    playerController.enabled = false;
}

// Start orbiting with video recording
CameraOrbitRecorder recorder = Camera.main.GetComponent<CameraOrbitRecorder>();
recorder.StartOrbit();
recorder.StartVideoRecording();
```

### With UI Systems
The script creates its own UI elements but can be integrated with existing UI:

```csharp
// Disable the built-in debug UI
recorder.showDebugInfo = false;

// Use your own UI to control the recorder
yourUIButton.onClick.AddListener(() => recorder.StartOrbit());
yourVideoButton.onClick.AddListener(() => recorder.StartVideoRecording());
```

## File Structure

```
Assets/Scripts/
├── CameraOrbitRecorder.cs          # Main orbit recording script with AVProMovieCapture
├── CameraOrbitExample.cs           # Example implementation with presets
└── CameraOrbitRecorder_README.md   # This documentation

Recordings/                          # Generated video folder
├── OrbitRecording_2024-01-01_12-00-00.mp4
├── OrbitRecording_2024-01-01_12-05-30.mp4
└── ...

Screenshots/                         # Generated screenshot folder (if used)
├── OrbitScreenshot_2024-01-01_12-00-00_0001.png
├── OrbitScreenshot_2024-01-01_12-00-00_0002.png
└── ...
```

## Requirements

- Unity 2019.4 or later
- **AVProMovieCapture** plugin (for video recording)
- Works in both Editor and Build (video recording works in Editor only)
- No additional packages required for basic functionality

## AVProMovieCapture Setup

If you don't have AVProMovieCapture installed:

1. **Purchase from Asset Store**: Search for "AVPro Movie Capture"
2. **Import the package**: Follow the installation instructions
3. **Enable the plugin**: The script will automatically detect and use it
4. **Alternative**: Use screenshot recording if AVProMovieCapture is not available

## License

This script is provided as-is for educational and development purposes. Feel free to modify and use in your projects.

**Note**: AVProMovieCapture is a commercial plugin. Please ensure you have a valid license for production use. 