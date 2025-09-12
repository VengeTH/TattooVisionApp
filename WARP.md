# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Project Overview

This is a Unity 6000.0.48f1 AR application for tattoo visualization using ARCore/ARKit and OpenCV. The project uses Firebase for backend services and targets Android (primary) and iOS (secondary) platforms.

## Common Development Commands

### Unity CLI Commands (if Unity Editor is installed with CLI support)

```bash
# Open project in Unity Editor
Unity.exe -projectPath "D:\Programming\Projects\Commission\TattooVisionApp2-main"

# Build Android APK
Unity.exe -projectPath "D:\Programming\Projects\Commission\TattooVisionApp2-main" -buildTarget Android -buildAndroidPlayer "build\TattooVision.apk" -batchmode -quit

# Build iOS Xcode project
Unity.exe -projectPath "D:\Programming\Projects\Commission\TattooVisionApp2-main" -buildTarget iOS -buildPath "build\ios" -batchmode -quit

# Run Unity tests
Unity.exe -projectPath "D:\Programming\Projects\Commission\TattooVisionApp2-main" -runTests -testResults results.xml -batchmode
```

### Android Development Commands

```bash
# Check connected Android devices
adb devices

# Install APK to device
adb install -r build\TattooVision.apk

# View Android logs for debugging
adb logcat -s Unity

# Clear app data
adb shell pm clear com.yourcompany.tattoovision

# Take screenshot from device
adb shell screencap /sdcard/screenshot.png
adb pull /sdcard/screenshot.png
```

### Git Commands for Unity Projects

```bash
# Clean Unity-generated files before commit
git clean -xfd -e Library -e Temp -e .vs

# Update submodules (if any)
git submodule update --init --recursive
```

## Architecture Overview

### Scene Architecture
The app uses multiple Unity scenes for different features:
- **AppScene.unity**: Main entry point and app initialization
- **Dashboard.unity**: Home screen with user collections and navigation
- **ARCamera.unity**: AR camera view for tattoo visualization
- **Gallery.unity** / **Gallery 1.unity**: Tattoo design galleries
- **Profile.unity** / **Profile 1.unity**: User profile management
- **Verification.unity**: User authentication/verification flow

### Core Components

#### Computer Vision & AR System
- **SkinScanner.cs**: Implements OpenCV-based skin detection and tattoo analysis
- **ARCameraUIManager.cs**: Manages ARFoundation camera, AR tracking, and overlay rendering
- Utilizes Unity XR packages (ARCore/ARKit) for device-specific AR functionality

#### Navigation & UI Flow
- **NavigationManager.cs**: Central navigation controller managing scene transitions
- **UIManager.cs**: Handles UI state management and user interactions
- **PanelController.cs**: Controls panel visibility and transitions within scenes

#### Content Management
- **Content Manager (script).cs**: Manages tattoo design collections and user content
- **GalleryLoader.cs**: Loads and displays tattoo designs from Firebase Storage
- **ImageUploader.cs**: Handles image upload functionality to cloud storage
- **MobileImageSlider.cs**: Implements touch-based image gallery navigation

#### Firebase Integration
- **FirebaseAuthManager.cs**: Handles user authentication flow
- Firebase SDK manages:
  - User authentication (Firebase Auth)
  - Design storage (Firebase Storage)  
  - User data sync (Firestore Database)
  - Real-time updates

#### Permission Management
- **PermissionManager.cs**: Centralized handling of Android/iOS permissions
- Manages camera, storage, and location permissions

### Key Dependencies
- **OpenCV for Unity**: Located in `Assets/OpenCV+Unity/` - provides computer vision capabilities
- **Firebase SDK**: Located in `Assets/Firebase/` - backend services
- **TextMeshPro**: UI text rendering (managed by Unity Package Manager)
- **Unity Input System**: Modern input handling for touch and AR interactions
- **XR Plugin Management**: ARCore (Android) and ARKit (iOS) support

### Data Flow
1. User launches app → **AppScene** initializes Firebase and checks authentication
2. Authenticated users land on **Dashboard** → Can navigate to AR Camera or Galleries
3. **AR Camera** flow:
   - PermissionManager requests camera access
   - ARCameraUIManager initializes AR session
   - SkinScanner processes camera feed for skin detection
   - User can overlay tattoo designs in real-time
4. Content is synced with Firebase for persistence across sessions

### Platform-Specific Considerations
- **Android**: Primary platform, requires API Level 24+, ARCore support
- **iOS**: Secondary platform, requires iOS 12.0+, ARKit support
- Platform-specific code should use Unity's platform defines: `#if UNITY_ANDROID` or `#if UNITY_IOS`

### Performance Considerations
- AR tracking is resource-intensive - limit simultaneous tracked objects
- Image processing with OpenCV should be optimized for mobile
- Use object pooling for frequently instantiated AR overlays
- Firebase operations should be batched when possible
