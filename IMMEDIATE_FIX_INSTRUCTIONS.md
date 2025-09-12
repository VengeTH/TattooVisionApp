# IMMEDIATE FIX FOR AR SETUP

## The Problem
The ARCore package version 5.2.bb7d408c3 is incompatible with Unity 6 because Unity 6 removed `AndroidExternalToolsSettings`. This is causing compilation errors that prevent any scripts from running.

## Solution Options

### Option 1: Remove ARCore Package Temporarily (RECOMMENDED)
This will allow you to set up the AR scene and test in editor. You can add ARCore back when ready to build for Android.

1. In Unity, go to **Window > Package Manager**
2. In the dropdown at top-left, select **"In Project"**
3. Find **"ARCore XR Plugin"**
4. Click **"Remove"**
5. Unity will recompile without errors
6. You can now run the AR setup

### Option 2: Downgrade to Compatible Version
1. **Window > Package Manager**
2. Find **"ARCore XR Plugin"**
3. Click the version dropdown
4. Select version **5.1.0** or earlier
5. Click **"Update"**

### Option 3: Manual Setup Without Scripts
Since scripts can't compile, manually create the AR structure:

#### In Unity Hierarchy:
1. **Right-click** in Hierarchy > **Create Empty**
2. Name it **"AR Session"**

3. **Right-click** > **Create Empty**
4. Name it **"XR Origin"**

5. Right-click **XR Origin** > **Create Empty**
6. Name it **"Camera Offset"**

7. **Drag your Main Camera** into Camera Offset (as child)
8. Set Main Camera position to (0, 1.36, 0)

9. Create empty GameObjects:
   - **"Managers"**
   - Under Managers: **"AR Tattoo Manager"**
   - Under Managers: **"AR Camera UI Manager"**  
   - Under Managers: **"Skin Scanner"**

## After Fixing Package Issue

Once the package errors are resolved:

1. Add components to **AR Session**:
   - Add Component > Search "ARSession" > Add it

2. Add components to **XR Origin**:
   - Add Component > "XROrigin" (from Unity.XR.CoreUtils)
   - Add Component > "ARRaycastManager"
   - Add Component > "ARPlaneManager"

3. Add to **Main Camera**:
   - Add Component > "ARCameraManager"
   - Add Component > "ARCameraBackground"

4. Add scripts to managers:
   - **AR Tattoo Manager** → ARTattooManager
   - **AR Camera UI Manager** → ARCameraUIManager
   - **Skin Scanner** → SkinScanner

## For Testing Without ARCore

You can test AR features in editor using:
1. **XR Simulation** (Window > XR > AR Foundation > XR Environment)
2. **AR Foundation Remote** app
3. **Mock AR data** in Play Mode

## Building for Android Later

When ready to build for Android:
1. Re-add ARCore package (version 6.0+ for Unity 6)
2. Configure XR Plug-in Management
3. Set up Android build settings

The AR tattoo system will work with these components set up, even without ARCore during development.
