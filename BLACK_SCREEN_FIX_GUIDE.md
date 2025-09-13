# Black Screen Fix Guide

## Problem Identified

Your TattooVision app shows only the logo and then a black screen because:

1. **Missing First Scene**: The `AppScene.unity` referenced in `ProfileMenuManager.cs` doesn't exist
2. **Wrong Scene Order**: Dashboard is set as the first scene but expects authentication
3. **Firebase Authentication**: App tries to load Dashboard without proper auth flow

## Solutions Implemented

### 1. Fixed Build Settings

- ✅ Removed non-existent `AppScene.unity` from build settings
- ✅ Added proper scene order: Login → Dashboard → ARCamera → Profile → Gallery
- ✅ Created new `Login.unity` scene as the first scene

### 2. Created Authentication Flow

- ✅ **AppInitializer.cs**: Handles initial app startup and auth checking
- ✅ **LoginSceneManager.cs**: Manages login/registration UI
- ✅ **DashboardInitializer.cs**: Handles Dashboard scene initialization
- ✅ **BlackScreenFixer.cs**: Emergency fallback to prevent black screens

### 3. Updated Profile Menu

- ✅ Fixed logout functionality to return to Dashboard instead of non-existent AppScene
- ✅ Added guest mode support for unauthenticated users

## How to Apply the Fix

### Option 1: Quick Fix (Recommended)

1. **Open Unity Editor**
2. **Go to File > Build Settings**
3. **Make sure scenes are in this order:**

   - Login (index 0) - **MUST BE FIRST**
   - Dashboard (index 1)
   - ARCamera (index 2)
   - Profile (index 3)
   - Gallery (index 4)

4. **Build and test the APK**

### Option 2: Complete Setup

1. **Add the new scripts to your scenes:**

   - Add `BlackScreenFixer` to your first scene (Login or Dashboard)
   - Add `DashboardInitializer` to Dashboard scene
   - Add `LoginSceneManager` to Login scene

2. **Configure UI references in Inspector:**

   - Assign UI panels and buttons to the script components
   - Set up proper scene names in the configuration fields

3. **Test in Unity Editor first:**
   - Play the scene to ensure everything works
   - Check Console for any errors

## Testing the Fix

### In Unity Editor:

1. Open the Login scene
2. Press Play
3. Verify the scene loads properly
4. Test navigation between scenes

### On Device:

1. Build the APK with the new scene order
2. Install on your phone
3. The app should now:
   - Show the TattooVision logo
   - Load the Login scene (or Dashboard with guest mode)
   - Allow navigation to other scenes

## Troubleshooting

### If you still get black screen:

1. **Check Console logs** in Unity Editor for errors
2. **Verify Firebase configuration** - ensure `google-services.json` is in Assets folder
3. **Test without Firebase** - temporarily disable Firebase to isolate the issue
4. **Use the BlackScreenFixer** - it will automatically load Dashboard after 10 seconds

### If scenes don't load:

1. **Check build settings** - ensure all scenes are enabled
2. **Verify scene names** - they must match exactly in scripts
3. **Check for missing references** - assign all UI elements in Inspector

## Firebase Configuration

Make sure you have:

- ✅ `google-services.json` in `Assets/` folder
- ✅ Firebase project properly configured
- ✅ Authentication enabled in Firebase Console

## Alternative: Guest Mode

If you want to skip authentication entirely:

1. Set `allowGuestMode = true` in `DashboardInitializer`
2. Users can use the app without logging in
3. Some features may be limited without authentication

## Next Steps

1. **Test the fix** with the new scene order
2. **Add proper UI** to the Login scene if needed
3. **Configure Firebase** properly for authentication
4. **Test on multiple devices** to ensure compatibility

The black screen issue should now be resolved. The app will either show the Login scene or Dashboard with guest mode, preventing the black screen completely.
