# 🚀 TattooVision Quick Deploy Script
# Automates building and deploying Unity AR app to Android devices

param(
    [switch]$BuildOnly,
    [switch]$DeployOnly,
    [switch]$EmulatorMode,
    [string]$PackageName = "com.yourdomain.tattoovision"
)

function Write-ColorText($text, $color) {
    Write-Host $text -ForegroundColor $color
}

function Check-Prerequisites {
    Write-ColorText "🔍 Checking prerequisites..." "Cyan"
    
    # Try to append common Android platform-tools locations to PATH (user-specific)
    $candidatePaths = @(
        "C:\Users\U S E R - N I T R O\AppData\Local\Android\Sdk\platform-tools",
        "$env:LOCALAPPDATA\Android\Sdk\platform-tools",
        "$env:USERPROFILE\AppData\Local\Android\Sdk\platform-tools",
        "$env:ANDROID_HOME\platform-tools",
        "$env:ANDROID_SDK_ROOT\platform-tools"
    )
    foreach ($p in $candidatePaths) {
        if (-not [string]::IsNullOrWhiteSpace($p) -and (Test-Path $p) -and ($env:PATH -notlike "*${p}*")) {
            $env:PATH += ";$p"
        }
    }
    
    # Check ADB
    try {
        $adbVersion = adb version 2>$null
        if ($adbVersion) {
            Write-ColorText "✅ ADB found" "Green"
        } else {
            throw "ADB not found"
        }
    } catch {
        Write-ColorText "❌ ADB not found in PATH. Please install Android SDK Platform Tools." "Red"
        return $false
    }
    
    # Check Unity
    $unityPath = "D:\Tools\Unity\6000.0.48f1-x86_64\Editor\Unity.exe"
    if (Test-Path $unityPath) {
        Write-ColorText "✅ Unity found at $unityPath" "Green"
    } else {
        Write-ColorText "❌ Unity not found. Please update path in build.bat" "Red"
        return $false
    }
    
    # Check if APK exists (for deploy-only mode)
    if ($DeployOnly -and -not (Test-Path "Build\TattooVision.apk")) {
        Write-ColorText "❌ APK not found. Run build first." "Red"
        return $false
    }
    
    return $true
}

function Build-App {
    Write-ColorText "📦 Building APK..." "Yellow"
    
    # Kill existing Unity processes
    Write-ColorText "🔄 Stopping Unity processes..." "Cyan"
    Stop-Process -Name "Unity" -Force -ErrorAction SilentlyContinue
    Stop-Process -Name "Unity Hub" -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
    
    # Run build script
    & .\build.bat
    
    if ($LASTEXITCODE -eq 0) {
        Write-ColorText "✅ Build completed successfully!" "Green"
        
        # Check APK size
        if (Test-Path "Build\TattooVision.apk") {
            $apkSize = (Get-Item "Build\TattooVision.apk").Length / 1MB
            Write-ColorText "📊 APK Size: $([math]::Round($apkSize, 2)) MB" "Cyan"
        }
        return $true
    } else {
        Write-ColorText "❌ Build failed!" "Red"
        Write-ColorText "📋 Recent errors from build.log:" "Yellow"
        
        if (Test-Path "build.log") {
            Get-Content "build.log" | Select-String -Pattern "error|Error|ERROR" | Select-Object -Last 10 | ForEach-Object {
                Write-ColorText $_.Line "Red"
            }
        }
        return $false
    }
}

function Check-Device {
    Write-ColorText "📱 Checking connected devices..." "Cyan"
    
    $devices = adb devices | Select-String -Pattern "device$"
    
    if ($devices.Count -eq 0) {
        Write-ColorText "❌ No devices connected!" "Red"
        Write-ColorText "💡 Make sure USB Debugging is enabled and device is connected" "Yellow"
        return $false
    }
    
    $deviceCount = $devices.Count
    Write-ColorText "✅ Found $deviceCount device(s):" "Green"
    
    foreach ($device in $devices) {
        $deviceId = $device.ToString().Split()[0]
        Write-ColorText "   📱 $deviceId" "Cyan"
        
        # Get device info
        $deviceModel = adb -s $deviceId shell getprop ro.product.model 2>$null
        $androidVersion = adb -s $deviceId shell getprop ro.build.version.release 2>$null
        
        if ($deviceModel -and $androidVersion) {
            Write-ColorText "      Model: $deviceModel, Android: $androidVersion" "Gray"
        }
    }
    
    return $true
}

function Check-ARCore-Support {
    param($deviceId)
    
    Write-ColorText "🔍 Checking ARCore support on device..." "Cyan"
    
    # Check if ARCore is installed
    $arCorePackage = adb -s $deviceId shell pm list packages | Select-String "com.google.ar.core"
    
    if ($arCorePackage) {
        Write-ColorText "✅ ARCore is installed" "Green"
    } else {
        Write-ColorText "⚠️  ARCore not found. Install from Play Store or APK" "Yellow"
        Write-ColorText "   Download: https://play.google.com/store/apps/details?id=com.google.ar.core" "Cyan"
    }
    
    # Check OpenGL ES version
    $glVersion = adb -s $deviceId shell dumpsys | Select-String "OpenGL ES" | Select-Object -First 1
    if ($glVersion) {
        Write-ColorText "📊 $glVersion" "Cyan"
    }
}

function Install-App {
    param($deviceId)
    
    Write-ColorText "📲 Installing APK on device..." "Yellow"
    
    # Uninstall existing app first (to avoid conflicts)
    Write-ColorText "🗑️  Removing existing installation..." "Cyan"
    adb -s $deviceId uninstall $PackageName 2>$null | Out-Null
    
    # Install with all permissions granted
    $installResult = adb -s $deviceId install -r -g "Build\TattooVision.apk" 2>&1
    
    if ($installResult -match "Success") {
        Write-ColorText "✅ Installation successful!" "Green"
        return $true
    } else {
        Write-ColorText "❌ Installation failed!" "Red"
        Write-ColorText "$installResult" "Red"
        return $false
    }
}

function Grant-Permissions {
    param($deviceId)
    
    Write-ColorText "🔐 Granting critical permissions..." "Yellow"
    
    $permissions = @(
        "android.permission.CAMERA",
        "android.permission.READ_EXTERNAL_STORAGE", 
        "android.permission.WRITE_EXTERNAL_STORAGE",
        "android.permission.ACCESS_FINE_LOCATION",
        "android.permission.ACCESS_COARSE_LOCATION",
        "android.permission.RECORD_AUDIO"
    )
    
    foreach ($permission in $permissions) {
        Write-ColorText "   Granting: $permission" "Gray"
        adb -s $deviceId shell pm grant $PackageName $permission 2>$null | Out-Null
    }
    
    Write-ColorText "✅ Permissions granted" "Green"
}

function Launch-App {
    param($deviceId)
    
    Write-ColorText "🎯 Launching TattooVision..." "Yellow"
    
    # Launch app
    $launchResult = adb -s $deviceId shell monkey -p $PackageName -c android.intent.category.LAUNCHER 1 2>&1
    
    if ($launchResult -match "Events injected: 1") {
        Write-ColorText "✅ App launched successfully!" "Green"
        return $true
    } else {
        Write-ColorText "⚠️  Launch may have failed. Check device." "Yellow"
        return $false
    }
}

function Monitor-Logs {
    param($deviceId)
    
    Write-ColorText "📋 Monitoring Unity logs... (Press Ctrl+C to stop)" "Cyan"
    Write-ColorText "🔍 Looking for Unity, AR, and TattooVision logs..." "Gray"
    
    # Clear existing logs
    adb -s $deviceId logcat -c
    
    # Start monitoring with filters
    try {
        adb -s $deviceId logcat | Select-String -Pattern "Unity|TattooVision|AR|OpenCV|Camera" | ForEach-Object {
            $line = $_.Line
            
            # Color code based on log level
            if ($line -match "E/") {
                Write-ColorText $line "Red"
            } elseif ($line -match "W/") {
                Write-ColorText $line "Yellow" 
            } elseif ($line -match "I/") {
                Write-ColorText $line "Cyan"
            } else {
                Write-ColorText $line "White"
            }
        }
    } catch {
        Write-ColorText "📋 Log monitoring stopped" "Cyan"
    }
}

function Deploy-To-Device {
    $devices = adb devices | Select-String -Pattern "device$"
    
    foreach ($device in $devices) {
        $deviceId = $device.ToString().Split()[0]
        Write-ColorText "🎯 Deploying to device: $deviceId" "Magenta"
        
        Check-ARCore-Support -deviceId $deviceId
        
        if (Install-App -deviceId $deviceId) {
            Grant-Permissions -deviceId $deviceId
            
            if (Launch-App -deviceId $deviceId) {
                Write-ColorText "🎉 Deployment complete for $deviceId!" "Green"
                Write-ColorText "💡 App should now be running on device" "Cyan"
                
                # Ask if user wants to monitor logs
                $monitorLogs = Read-Host "Monitor logs for this device? (y/N)"
                if ($monitorLogs -eq "y" -or $monitorLogs -eq "Y") {
                    Monitor-Logs -deviceId $deviceId
                }
            }
        }
        
        Write-Host ""
    }
}

# Main execution flow
Write-ColorText "🚀 TattooVision Quick Deploy Script" "Green"
Write-ColorText "===================================" "Green"

if (-not (Check-Prerequisites)) {
    exit 1
}

# Build phase
if (-not $DeployOnly) {
    if (-not (Build-App)) {
        Write-ColorText "❌ Stopping due to build failure" "Red"
        Read-Host "Press Enter to exit"
        exit 1
    }
}

# Exit if build-only mode
if ($BuildOnly) {
    Write-ColorText "✅ Build completed. Exiting (build-only mode)" "Green"
    Read-Host "Press Enter to exit"
    exit 0
}

# Deploy phase
if (-not (Check-Device)) {
    Write-ColorText "❌ No devices available for deployment" "Red"
    Read-Host "Press Enter to exit"
    exit 1
}

Deploy-To-Device

Write-ColorText "🎉 All done!" "Green"
Write-ColorText "💡 Check your device for the TattooVision app" "Cyan"
Read-Host "Press Enter to exit"
