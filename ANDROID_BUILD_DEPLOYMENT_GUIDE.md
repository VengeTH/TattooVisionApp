# 📱 Android Build & Deployment Guide for TattooVision AR App

## 🚨 Common Issues & Solutions for Physical Device Testing

### Issue: "Works in Unity Player but not on physical device"

This typically happens due to:

1. **Missing ARCore support** on device
2. **Camera/Storage permissions** not granted
3. **Google Services configuration** missing
4. **API Level compatibility** issues
5. **AR tracking requirements** not met

---

## 🛠️ Prerequisites & Setup

### 1. Required Software

```bash
# Check if these are installed:
# - Unity 6000.0.48f1 (as per your build.bat)
# - Android SDK (API 24+)
# - Java JDK 8 or 11
# - Android Build Tools
# - ADB (Android Debug Bridge)
```

### 2. Environment Variables Setup

```bash
# Add these to your system PATH:
# Windows PowerShell:
$env:ANDROID_HOME = "C:\Users\YourName\AppData\Local\Android\Sdk"
$env:JAVA_HOME = "C:\Program Files\Java\jdk-11.0.x"
$env:PATH += ";$env:ANDROID_HOME\platform-tools"
$env:PATH += ";$env:ANDROID_HOME\build-tools\34.0.0"

# Linux/Mac:
export ANDROID_HOME=$HOME/Android/Sdk
export JAVA_HOME=/usr/lib/jvm/java-11-openjdk
export PATH=$PATH:$ANDROID_HOME/platform-tools:$ANDROID_HOME/build-tools/34.0.0
```

### 3. Verify Installation

```bash
# Test ADB connection
adb version

# List connected devices
adb devices

# Check available emulators
emulator -list-avds
```

---

## 🔧 Build Commands

### Method 1: Using Your Existing Build Script (Recommended)

```bash
# Navigate to project directory
cd "D:\Programming\Projects\Commission\TattooVisionApp2-main"

# Run the build script
.\build.bat

# Check build log for errors
type build.log | findstr /i "error"
```

### Method 2: Direct Unity Command Line Build

```bash
# Create custom build script
# Build for Android (Development)
"D:\Tools\Unity\6000.0.48f1-x86_64\Editor\Unity.exe" ^
-batchmode ^
-quit ^
-projectPath "D:\Programming\Projects\Commission\TattooVisionApp2-main" ^
-buildTarget Android ^
-executeMethod BuildPlayer.PerformAndroidDevelopmentBuild ^
-logFile "build.log"

# Build for Android (Release)
"D:\Tools\Unity\6000.0.48f1-x86_64\Editor\Unity.exe" ^
-batchmode ^
-quit ^
-projectPath "D:\Programming\Projects\Commission\TattooVisionApp2-main" ^
-buildTarget Android ^
-executeMethod BuildPlayer.PerformAndroidReleaseBuild ^
-logFile "build.log"
```

### Method 3: Advanced Build with Custom Settings

```bash
# Build with specific settings
"D:\Tools\Unity\6000.0.48f1-x86_64\Editor\Unity.exe" ^
-batchmode ^
-quit ^
-projectPath "%cd%" ^
-buildTarget Android ^
-executeMethod BuildPlayer.BuildAndroid ^
-buildPath "Build/TattooVision.apk" ^
-developmentBuild ^
-enableDeepProfilingSupport ^
-scriptDebugging ^
-logFile "build_debug.log"
```

---

## 📱 Deployment to Physical Device

### 1. Enable Developer Options

```bash
# ! First, enable Developer Options on your Android device:
# Settings > About Phone > Tap "Build Number" 7 times
# Then go to Settings > Developer Options > Enable USB Debugging
```

### 2. Install APK via ADB

```bash
# Check if device is connected
adb devices

# Install the APK
adb install -r "Build/TattooVision.apk"

# Or force install (if app exists)
adb install -r -d "Build/TattooVision.apk"

# Install with all permissions granted (Android 6+)
adb install -r -g "Build/TattooVision.apk"
```

### 3. Grant Required Permissions

```bash
# Grant camera permission (CRITICAL for AR)
adb shell pm grant com.yourdomain.tattoovision android.permission.CAMERA

# Grant storage permissions
adb shell pm grant com.yourdomain.tattoovision android.permission.READ_EXTERNAL_STORAGE
adb shell pm grant com.yourdomain.tattoovision android.permission.WRITE_EXTERNAL_STORAGE

# Grant location permission (for AR tracking)
adb shell pm grant com.yourdomain.tattoovision android.permission.ACCESS_FINE_LOCATION

# List all permissions for the app
adb shell dumpsys package com.yourdomain.tattoovision | findstr permission
```

### 4. Launch and Monitor

```bash
# Launch the app
adb shell monkey -p com.yourdomain.tattoovision -c android.intent.category.LAUNCHER 1

# Monitor logs in real-time
adb logcat | findstr "TattooVision"

# Clear logs and monitor from fresh start
adb logcat -c
adb logcat | findstr -i "unity\|tattoo\|ar"
```

---

## 🖥️ Android Studio Emulator Deployment

### 1. List Available Emulators

```bash
# List all AVDs
emulator -list-avds

# Example output:
# Pixel_4_API_30
# Pixel_6_API_33
```

### 2. Start Emulator with ARCore Support

```bash
# Start emulator with Google Play services (required for ARCore)
emulator -avd Pixel_4_API_30 -gpu host -camera-back webcam0

# Start with specific camera settings for AR testing
emulator -avd Pixel_6_API_33 -gpu auto -camera-back virtualscene -camera-front webcam0
```

### 3. Install ARCore on Emulator

```bash
# Wait for emulator to fully boot
adb wait-for-device

# Install Google Play Services for AR
# Download from: https://github.com/google-ar/arcore-android-sdk/releases
adb install "arcore-android_v1.xx.x.apk"

# Verify ARCore installation
adb shell pm list packages | findstr "google.ar.core"
```

### 4. Deploy to Emulator

```bash
# Install your app
adb install -r "Build/TattooVision.apk"

# Grant permissions immediately
adb shell pm grant com.yourdomain.tattoovision android.permission.CAMERA
adb shell pm grant com.yourdomain.tattoovision android.permission.READ_EXTERNAL_STORAGE
adb shell pm grant com.yourdomain.tattoovision android.permission.WRITE_EXTERNAL_STORAGE
```

---

## 🐛 Debugging Physical Device Issues

### 1. Check AR Compatibility

```bash
# Check if ARCore is supported
adb shell getprop ro.config.low_ram

# Check OpenGL ES version
adb shell dumpsys | findstr "OpenGL ES"

# Check camera capabilities
adb shell dumpsys camera
```

### 2. Monitor Unity Logs

```bash
# Real-time Unity logs
adb logcat -s Unity

# Filter for errors only
adb logcat | findstr /i "error\|exception\|crash"

# Specific AR-related logs
adb logcat | findstr /i "ar\|camera\|opencv"
```

### 3. Firebase Configuration Check

```bash
# Check if google-services.json exists in StreamingAssets
adb shell ls /sdcard/Android/data/com.yourdomain.tattoovision/files/

# Test Firebase connection
adb logcat | findstr /i "firebase\|google"
```

### 4. Performance Monitoring

```bash
# Monitor CPU usage
adb shell top | findstr tattoovision

# Monitor memory usage
adb shell dumpsys meminfo com.yourdomain.tattoovision

# Monitor battery usage
adb shell dumpsys battery
```

---

## 🔄 Quick Build & Deploy Script

Create this PowerShell script for rapid development:

```powershell
# quick-deploy.ps1
Write-Host "🚀 TattooVision Quick Deploy Script" -ForegroundColor Green

# Step 1: Build
Write-Host "📦 Building APK..." -ForegroundColor Yellow
& .\build.bat

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Build successful!" -ForegroundColor Green

    # Step 2: Install
    Write-Host "📱 Installing on device..." -ForegroundColor Yellow
    adb install -r -g "Build\TattooVision.apk"

    # Step 3: Grant permissions
    Write-Host "🔐 Granting permissions..." -ForegroundColor Yellow
    adb shell pm grant com.yourdomain.tattoovision android.permission.CAMERA
    adb shell pm grant com.yourdomain.tattoovision android.permission.READ_EXTERNAL_STORAGE
    adb shell pm grant com.yourdomain.tattoovision android.permission.WRITE_EXTERNAL_STORAGE

    # Step 4: Launch and monitor
    Write-Host "🎯 Launching app..." -ForegroundColor Yellow
    adb shell monkey -p com.yourdomain.tattoovision -c android.intent.category.LAUNCHER 1

    Write-Host "📋 Monitoring logs... (Press Ctrl+C to stop)" -ForegroundColor Cyan
    adb logcat -s Unity
} else {
    Write-Host "❌ Build failed! Check build.log" -ForegroundColor Red
    type build.log | Select-String -Pattern "error" -CaseSensitive:$false
}
```

Run it with:

```bash
powershell -ExecutionPolicy Bypass -File quick-deploy.ps1
```

---

## 🛡️ Device-Specific AR Fixes

### Fix 1: ARCore Installation Issues

```bash
# Uninstall old ARCore
adb shell pm uninstall com.google.ar.core

# Install latest ARCore from APK Mirror or Google Play
# Then reinstall your app
adb install -r "Build/TattooVision.apk"
```

### Fix 2: Camera Permission Issues

```bash
# Reset app permissions
adb shell pm reset-permissions com.yourdomain.tattoovision

# Grant permissions manually
adb shell pm grant com.yourdomain.tattoovision android.permission.CAMERA
```

### Fix 3: OpenCV Issues

```bash
# Check OpenCV native library loading
adb logcat | findstr /i "opencv\|native"

# Clear app data if OpenCV fails to initialize
adb shell pm clear com.yourdomain.tattoovision
```

---

## 📊 Testing Checklist

### ✅ Pre-Deployment

- [ ] Build completed without errors
- [ ] APK size reasonable (<100MB)
- [ ] All required permissions in AndroidManifest.xml
- [ ] Google Services JSON configured
- [ ] ARCore dependency declared

### ✅ Post-Deployment

- [ ] App launches without crashes
- [ ] Camera permission granted
- [ ] AR session initializes
- [ ] OpenCV skin detection works
- [ ] Tattoo placement functional
- [ ] UI responsive on device

### ✅ Performance Tests

- [ ] Frame rate >30fps during AR
- [ ] Memory usage <500MB
- [ ] Battery drain acceptable
- [ ] No thermal throttling

---

## 🚨 Emergency Debugging Commands

```bash
# If app won't start:
adb shell am start -n com.yourdomain.tattoovision/.MainActivity

# If app crashes immediately:
adb logcat -b crash

# If AR won't initialize:
adb shell setprop debug.ar.enabled 1

# Clear all app data:
adb shell pm clear com.yourdomain.tattoovision

# Restart ADB if connection issues:
adb kill-server && adb start-server
```

---

## 💡 Pro Tips

1. **Always test on multiple devices** - AR compatibility varies significantly
2. **Use development builds** for better debugging
3. **Monitor memory usage** - AR apps are resource-intensive
4. **Test in different lighting conditions** - affects AR tracking
5. **Keep ARCore updated** on test devices
6. **Use Unity Profiler** for performance optimization

---

Remember: The key difference between Unity Player and device testing is that devices have real hardware constraints, require proper permissions, and need ARCore support. Your AR tracking and computer vision features depend heavily on device capabilities!




