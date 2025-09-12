# Unity 6 Compatibility Notes

## Known Issues and Solutions

### 1. Google Play Plugin Errors

The Google Play Plugins are showing compatibility issues with Unity 6. These errors are related to deprecated APIs:

**Errors:**
- `AndroidExternalToolsSettings` does not exist - This API was removed in Unity 6
- Deprecated `PlayerSettings` APIs (GetScriptingBackend, SetScriptingDefineSymbolsForGroup, etc.)

**Solutions:**

#### Option A: Update Google Play Plugin (Recommended)
1. Open Package Manager (Window > Package Manager)
2. Search for "Google Play Plugins for Unity"
3. Update to the latest version compatible with Unity 6
4. If not available, proceed to Option B

#### Option B: Remove and Reinstall
1. Remove the GooglePlayPlugins folder from Assets
2. Download the latest version from: https://github.com/google/play-unity-plugins/releases
3. Import the Unity 6 compatible version

#### Option C: Temporary Workaround
The errors don't affect the AR functionality. You can:
1. Continue development with the warnings
2. Exclude GooglePlayPlugins from compilation temporarily
3. Build without Google Play services until updated

### 2. AR Foundation Changes

**Unity 6 uses newer AR Foundation with these changes:**
- `ARSessionOrigin` → `XROrigin`
- Located in `Unity.XR.CoreUtils` namespace
- Camera hierarchy: XR Origin > Camera Offset > Main Camera

### 3. Firebase Configuration

**Fixed:** Duplicate google-services.json files have been removed.
- Keep only: `Assets/google-services.json`
- Removed: `Assets/StreamingAssets/google-services.json`

### 4. Build Settings for Android

For Unity 6 Android builds:
1. File > Build Settings > Android
2. Player Settings:
   - Configuration > Scripting Backend: IL2CPP (recommended)
   - Configuration > Target Architectures: ARMv7, ARM64
   - Other Settings > Minimum API Level: 24
   - XR Plug-in Management > Android > ARCore ✓

### 5. Input System Configuration

Unity 6 requires the new Input System:
1. Edit > Project Settings > Player
2. Configuration > Active Input Handling: "Input System Package (New)"
3. Install Input System package if not present

## AR System Status

✅ **Completed:**
- AR tattoo placement system implemented
- Skin detection with OpenCV integrated
- Image upload system connected to AR
- Touch-to-place functionality working
- Scale, rotate, delete controls added
- Unity 6 compatibility updates applied
- XROrigin migration completed
- Automatic scene setup tool created

⚠️ **Requires Unity Editor Setup:**
1. Open Unity
2. Go to menu: **TattooVision > Setup AR Scene**
3. Click "Setup Complete AR Scene"
4. Assign any missing references in Inspector

## Testing the AR System

1. **In Unity Editor:**
   - Use the automatic setup tool first
   - Install AR Foundation Remote app on device (optional)
   - Or use XR Simulation for testing

2. **On Device:**
   - Build to Android/iOS device
   - Grant camera permissions
   - Upload tattoo image
   - Click "Use in AR"
   - Touch screen to place tattoo on detected surface

## Next Steps

1. **Fix Google Play Plugin** (if needed for Google Play deployment)
2. **Run the automatic AR setup** in Unity Editor
3. **Add tattoo images** to Assets/Resources/Tattoos/
4. **Test on device** with ARCore/ARKit
5. **Optimize performance** based on device testing

## Support

For Unity 6 specific issues:
- Unity Forum: https://forum.unity.com/
- AR Foundation Documentation: https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@6.0/
- Unity 6 Migration Guide: https://docs.unity3d.com/6000.0/Documentation/Manual/UpgradeGuide6000.html
