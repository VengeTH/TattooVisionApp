# 🚀 Quick Start Terminal Commands for TattooVision AR

## 🎯 Essential Commands (Copy & Paste Ready)

### 1. Quick Setup Check

```bash
# Check if everything is ready
adb version
adb devices
emulator -list-avds
```

### 2. Build & Deploy (One Command)

```bash
# Easy way - Use the batch launcher
quick-deploy.bat

# Or PowerShell directly
powershell -ExecutionPolicy Bypass -File quick-deploy.ps1
```

### 3. Manual Build Commands

```bash
# Build APK using existing script
.\build.bat

# Check for errors
type build.log | findstr /i "error"
```

### 4. Deploy to Device

```bash
# Install APK with permissions
adb install -r -g "Build\TattooVision.apk"

# Grant critical permissions
adb shell pm grant com.yourdomain.tattoovision android.permission.CAMERA
adb shell pm grant com.yourdomain.tattoovision android.permission.READ_EXTERNAL_STORAGE
adb shell pm grant com.yourdomain.tattoovision android.permission.WRITE_EXTERNAL_STORAGE
```

### 5. Launch & Monitor

```bash
# Launch app
adb shell monkey -p com.yourdomain.tattoovision -c android.intent.category.LAUNCHER 1

# Monitor Unity logs (CRITICAL for debugging AR issues)
adb logcat -s Unity

# Monitor specific AR/Camera issues
adb logcat | findstr /i "ar\|camera\|opencv\|tattoo"
```

---

## 🔥 Troubleshooting Commands

### If App Crashes on Launch

```bash
# Check crash logs
adb logcat -b crash

# Clear app data and retry
adb shell pm clear com.yourdomain.tattoovision
adb install -r -g "Build\TattooVision.apk"
```

### If AR Not Working

```bash
# Check ARCore installation
adb shell pm list packages | findstr "google.ar.core"

# Check device AR capability
adb shell getprop ro.config.low_ram
adb shell dumpsys | findstr "OpenGL ES"
```

### If Camera Permission Issues

```bash
# Reset permissions
adb shell pm reset-permissions com.yourdomain.tattoovision

# Grant permissions again
adb shell pm grant com.yourdomain.tattoovision android.permission.CAMERA
```

---

## 📱 Android Studio Emulator Commands

### Start Emulator with AR Support

```bash
# List available emulators
emulator -list-avds

# Start with camera support (replace Pixel_4_API_30 with your AVD name)
emulator -avd Pixel_4_API_30 -gpu host -camera-back webcam0
```

### Install ARCore on Emulator

```bash
# Wait for emulator to boot
adb wait-for-device

# Download ARCore APK from: https://github.com/google-ar/arcore-android-sdk/releases
# Then install (replace with actual APK name)
adb install "arcore-android_v1.xx.x.apk"
```

---

## 💡 Pro Developer Workflow

### Complete Build-Test Cycle

```bash
# 1. Clean build
.\build.bat

# 2. Deploy with full setup
adb install -r -g "Build\TattooVision.apk"
adb shell pm grant com.yourdomain.tattoovision android.permission.CAMERA
adb shell pm grant com.yourdomain.tattoovision android.permission.READ_EXTERNAL_STORAGE

# 3. Launch and monitor
adb shell monkey -p com.yourdomain.tattoovision -c android.intent.category.LAUNCHER 1
adb logcat -c && adb logcat -s Unity
```

### Quick Redeploy (After Code Changes)

```bash
# Build only
.\build.bat

# Quick install (app already exists)
adb install -r "Build\TattooVision.apk"

# Launch
adb shell monkey -p com.yourdomain.tattoovision -c android.intent.category.LAUNCHER 1
```

### Performance Monitoring

```bash
# Monitor memory usage
adb shell dumpsys meminfo com.yourdomain.tattoovision

# Monitor CPU usage
adb shell top | findstr tattoovision

# Monitor frame rate (requires development build)
adb shell dumpsys gfxinfo com.yourdomain.tattoovision framestats
```

---

## 🚨 Emergency Fixes for Common AR Issues

### Issue: "ARCore not supported"

```bash
# Check device compatibility
adb shell pm list packages | grep google.ar
adb shell getprop ro.product.model

# Install ARCore manually if missing
# Download from APK Mirror or Google Play Store
```

### Issue: "Camera initialization failed"

```bash
# Check camera permissions
adb shell dumpsys package com.yourdomain.tattoovision | findstr permission

# Check camera hardware
adb shell dumpsys camera

# Reset camera app if needed
adb shell pm clear com.android.camera
```

### Issue: "OpenCV native library not loaded"

```bash
# Check native library loading
adb logcat | findstr /i "opencv\|native"

# Clear app data to reset OpenCV
adb shell pm clear com.yourdomain.tattoovision

# Reinstall with clean state
adb install -r -g "Build\TattooVision.apk"
```

---

## 🎯 Testing on Physical Device vs Emulator

### Physical Device (Recommended for AR)

**Pros:**

- ✅ Real AR tracking
- ✅ Actual camera hardware
- ✅ True performance testing
- ✅ Real-world lighting conditions

**Setup:**

1. Enable Developer Options (tap Build Number 7 times)
2. Enable USB Debugging
3. Connect via USB
4. Allow computer debugging when prompted

### Emulator (Good for UI testing)

**Pros:**

- ✅ Quick iteration
- ✅ No device needed
- ✅ Consistent testing environment

**Cons:**

- ❌ Limited AR functionality
- ❌ Simulated camera only
- ❌ May not reflect real performance

---

## 🔧 Package Name Customization

If you need to change the package name for testing:

```bash
# Update in the build script by editing:
# Assets\Script\Editor\BuildPlayer.cs
# Change: private static readonly string PACKAGE_NAME = "com.yourdomain.tattoovision";

# Then rebuild
.\build.bat
```

---

## 📊 Build Output Analysis

After each build, check these files:

- `build.log` - Build process logs
- `Build\TattooVision.apk` - Your app (should be 50-100MB)
- Unity Console - Real-time build status

**Good build indicators:**

- ✅ APK size: 50-100MB (reasonable for AR app)
- ✅ No errors in build.log
- ✅ All scenes included
- ✅ ARCore dependencies resolved

---

## 🎉 Success Checklist

After deployment, verify:

- [ ] App launches without crashing
- [ ] Camera permission granted automatically
- [ ] AR session initializes (check logs for "AR Session Started")
- [ ] OpenCV loads successfully (check logs for "OpenCV initialized")
- [ ] UI responds to touch
- [ ] Can navigate between scenes
- [ ] Can upload and view tattoo images
- [ ] AR placement works (point at flat surface)

---

**💡 Remember:** Always test on physical device for final verification, as AR apps behave very differently on real hardware vs Unity Editor!




