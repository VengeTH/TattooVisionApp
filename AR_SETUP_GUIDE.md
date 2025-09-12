# AR Tattoo System Setup Guide for Unity 6

## Overview
This guide will help you set up the AR tattoo visualization system in Unity 6. The system allows users to:
- Upload tattoo images
- Detect skin using computer vision
- Place tattoos on skin in AR
- Scale, rotate, and position tattoos in real-time

## Prerequisites
- Unity 6000.0.48f1 (Unity 6) or later
- AR Foundation 6.0.6+
- ARCore (Android) / ARKit (iOS)
- OpenCV for Unity (already included)

## IMPORTANT: Unity 6 Changes
- **XR Origin**: Unity 6 uses `XROrigin` instead of `ARSessionOrigin`
- **New Input System**: Uses Input System package for XR tracking
- **API Updates**: Some PlayerSettings APIs have changed

## Step 1: Automatic Scene Setup (Recommended)

### Use the Unity Editor Tool:
1. Open Unity
2. Go to menu: **TattooVision > Setup AR Scene**
3. Click **"Setup Complete AR Scene"** button
4. The tool will automatically create all necessary GameObjects and components

## Step 2: Manual Scene Setup (if needed)

### 2.1 ARCamera Scene Configuration

1. Open `Assets/Scenes/ARCamera.unity`
2. Create the following GameObject hierarchy:

```
ARCamera Scene
├── AR Session (with ARSession component)
├── XR Origin (Unity 6 - replaces AR Session Origin)
│   ├── Camera Offset
│   │   └── Main Camera
│   │       ├── ARCameraManager (component)
│   │       ├── ARCameraBackground (component)
│   │       └── TrackedPoseDriver (component)
│   ├── ARPlaneManager (component)
│   └── ARRaycastManager (component)
├── Canvas (UI)
│   ├── AR Controls Panel
│   │   ├── Apply Tattoo Button
│   │   ├── Scale Slider
│   │   ├── Rotate Button
│   │   └── Delete Button
│   ├── Tattoo Selection Panel
│   │   └── Grid Layout Group
│   ├── Status Text
│   └── Loading Panel
├── EventSystem (with InputSystemUIInputModule)
└── Managers
    ├── AR Tattoo Manager
    ├── AR Camera UI Manager
    └── Skin Scanner
```

### 2.2 Component Configuration

#### XR Origin (Unity 6)
- Add Components:
  - `XROrigin` (from Unity.XR.CoreUtils)
  - `ARPlaneManager`
  - `ARRaycastManager`

#### Main Camera
- Path: XR Origin > Camera Offset > Main Camera
- Add Components:
  - `Camera`
  - `ARCameraManager`
  - `ARCameraBackground`
  - `TrackedPoseDriver` (Input System)
  - Set Tag: "MainCamera"
  - Set Position: (0, 1.36, 0) in Camera Offset

## Step 3: Manager Script Configuration

### 3.1 ARTattooManager Setup

1. Create empty GameObject named "ARTattooManager"
2. Add `ARTattooManager.cs` script
3. In Inspector, assign:
   - **AR Raycast Manager**: Reference from AR Session Origin
   - **AR Plane Manager**: Reference from AR Session Origin
   - **AR Camera**: Main Camera reference
   - **UI Elements**: Link all buttons and sliders
   - **Tattoo Settings**:
     - Default Scale: 0.1
     - Min Scale: 0.05
     - Max Scale: 0.5

### 3.2 ARCameraUIManager Setup

1. Create empty GameObject named "ARCameraUIManager"
2. Add `ARCameraUIManager.cs` script
3. In Inspector, assign:
   - **AR Session**: AR Session GameObject
   - **XR Origin**: XR Origin GameObject (Unity 6)
   - **AR Camera Manager**: From AR Camera
   - **AR Raycast Manager**: From AR Session Origin
   - **AR Plane Manager**: From AR Session Origin
   - **Tattoo Manager**: ARTattooManager reference
   - **Skin Scanner**: SkinScanner reference
   - **UI Components**: All UI references

### 3.3 SkinScanner Setup

1. Create empty GameObject named "SkinScanner"
2. Add `SkinScanner.cs` script
3. In Inspector, assign:
   - **Camera Manager**: ARCameraManager from AR Camera
   - **Tattoo Manager**: ARTattooManager reference
   - **AR Raycast Manager**: From AR Session Origin
   - **Skin Detection Settings**:
     - Enable Skin Detection: ✓
     - Skin Detection Threshold: 0.7

## Step 4: UI Setup

### 4.1 Create AR Control UI

1. Create Canvas if not present
2. Add UI elements:

```csharp
// Apply Tattoo Button
- Position: Bottom Center
- Size: 150x50
- Text: "Apply Tattoo"
- OnClick: ARTattooManager.StartTattooPlacement()

// Scale Slider
- Position: Bottom Left
- Size: 200x30
- Min Value: 0.05
- Max Value: 0.5
- OnValueChanged: ARTattooManager.OnScaleChanged()

// Rotate Button
- Position: Bottom Right
- Size: 50x50
- Icon: Rotation icon
- OnClick: ARTattooManager.ToggleRotation()

// Delete Button
- Position: Top Right of tattoo controls
- Size: 40x40
- Icon: Trash icon
- OnClick: ARTattooManager.DeleteCurrentTattoo()
```

### 4.2 Tattoo Selection Panel

1. Create Panel GameObject
2. Add Grid Layout Group component:
   - Cell Size: 100x100
   - Spacing: 10x10
   - Padding: 10 all sides
3. This will be populated dynamically with uploaded tattoos

## Step 5: Image Upload Integration

### 5.1 Update ImageUploader

1. Open `Assets/Script/Dashboard/ImageUploader.cs`
2. In Inspector, assign:
   - **AR Tattoo Manager**: Reference to ARTattooManager
   - This allows uploaded images to be available in AR

### 5.2 Connect Navigation

1. Open NavigationManager GameObject
2. Assign references:
   - **AR Tattoo Manager**: ARTattooManager
   - **AR Camera UI Manager**: ARCameraUIManager

## Step 6: Tattoo Resources Setup

### 6.1 Create Resources Folder

1. Create folder: `Assets/Resources/Tattoos/`
2. Add sample tattoo images (PNG with transparency recommended)
3. For each image:
   - Set Texture Type: "Sprite (2D and UI)"
   - Set Sprite Mode: "Single"
   - Apply settings

## Step 7: Build Settings

### 7.1 Android Configuration

1. File > Build Settings > Android
2. Player Settings:
   - Minimum API Level: 24
   - Target API Level: Latest
   - Graphics APIs: OpenGLES3, Vulkan
3. XR Plug-in Management:
   - Enable ARCore
   - Required: ✓

### 7.2 iOS Configuration

1. File > Build Settings > iOS
2. Player Settings:
   - Minimum iOS Version: 12.0
   - Camera Usage Description: "This app uses the camera for AR tattoo visualization"
3. XR Plug-in Management:
   - Enable ARKit
   - Required: ✓

## Step 8: Testing

### 8.1 Unity Editor Testing

1. Install AR Foundation Remote (optional)
2. Or use Unity's XR Simulation:
   - Window > XR > AR Foundation > XR Environment
   - Select simulation environment

### 8.2 Device Testing

1. Build to device
2. Test workflow:
   - Launch app
   - Navigate to Gallery/Dashboard
   - Upload tattoo image
   - Click "Use in AR"
   - Point camera at skin/flat surface
   - Touch to place tattoo
   - Use controls to adjust

## Usage Flow

1. **Upload Tattoo**: User uploads image from gallery
2. **Select for AR**: Click "Use in AR" button on uploaded image
3. **AR Camera Opens**: System switches to AR camera view
4. **Skin Detection**: OpenCV detects skin areas (optional)
5. **Touch to Place**: User touches screen where tattoo should appear
6. **Adjust**: Use slider for size, button for rotation
7. **Confirm**: Tattoo stays in AR space, tracked by ARFoundation

## Troubleshooting

### AR Not Working
- Check ARCore/ARKit is installed on device
- Verify camera permissions are granted
- Ensure good lighting conditions

### Tattoos Not Appearing
- Check tattoo images are in Resources/Tattoos folder
- Verify images are set as Sprites
- Check ARTattooManager references are assigned

### Skin Detection Issues
- Adjust skin detection threshold in SkinScanner
- Ensure good lighting
- Check OpenCV is properly imported

### Performance Issues
- Reduce tattoo image resolution
- Limit number of simultaneous tattoos
- Disable skin detection if not needed

## Advanced Features

### Custom Shaders
- Create custom shaders for tattoo rendering
- Add effects like glow, fade, or color adjustments

### Multiple Tattoos
- System supports placing multiple tattoos
- Each can be individually controlled

### Save/Load Sessions
- Implement saving tattoo positions
- Reload previous AR sessions

## Next Steps

1. Test on target devices
2. Optimize performance
3. Add more visual effects
4. Implement tattoo library management
5. Add social sharing features
