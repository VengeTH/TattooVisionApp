# TattooVision Quick Deploy (ASCII-safe)
# Builds APK, installs to connected device(s), launches app, optional logcat

param(
    [switch]$BuildOnly,
    [switch]$DeployOnly,
    [string]$PackageName = "com.yourdomain.tattoovision"
)

function Info($msg) { Write-Host $msg -ForegroundColor Cyan }
function Ok($msg) { Write-Host $msg -ForegroundColor Green }
function Warn($msg) { Write-Host $msg -ForegroundColor Yellow }
function Err($msg) { Write-Host $msg -ForegroundColor Red }

function Ensure-Adb {
    Info "Checking ADB..."
    $candidates = @(
        'C:\Users\U S E R - N I T R O\AppData\Local\Android\Sdk\platform-tools',
        "$env:LOCALAPPDATA\Android\Sdk\platform-tools",
        "$env:USERPROFILE\AppData\Local\Android\Sdk\platform-tools",
        "$env:ANDROID_HOME\platform-tools",
        "$env:ANDROID_SDK_ROOT\platform-tools"
    )
    foreach ($p in $candidates) {
        if ([string]::IsNullOrWhiteSpace($p)) { continue }
        if (Test-Path $p) {
            $parts = $env:PATH -split ';'
            if ($parts -notcontains $p) { $env:PATH = "$env:PATH;$p" }
        }
    }
    try {
        $v = adb version 2>$null
        if (-not $v) { throw 'adb missing' }
        Ok "ADB available"
        return $true
    } catch {
        Err "ADB not found. Install Android SDK Platform-Tools and/or set PATH."
        return $false
    }
}

function Build-App {
    if ($DeployOnly) { return $true }
    Info "Building APK via build.bat..."
    & .\build.bat
    if ($LASTEXITCODE -ne 0) { Err "Build failed. See build.log"; return $false }
    if (-not (Test-Path "Build\TattooVision.apk")) { Err "APK not found after build"; return $false }
    Ok "Build ok"
    return $true
}

function Get-Devices {
    $list = @()
    $lines = adb devices | Select-String -Pattern "\tdevice$"
    foreach ($ln in $lines) { $list += $ln.ToString().Split()[0] }
    return $list
}

function Install-App($deviceId) {
    Info "Installing on ${deviceId} ..."
    adb -s $deviceId uninstall $PackageName 2>$null | Out-Null
    $res = adb -s $deviceId install -r -g "Build\TattooVision.apk" 2>&1
    if ($res -match 'Success') { Ok "Install ok on $deviceId"; return $true }
    Err "Install failed on ${deviceId}: $res"; return $false
}

function Launch-App($deviceId) {
    Info "Launching on ${deviceId} ..."
    $res = adb -s $deviceId shell monkey -p $PackageName -c android.intent.category.LAUNCHER 1 2>&1
    if ($res -match 'Events injected: 1') { Ok "Launched on $deviceId"; return $true }
    Warn "Launch may have failed on $deviceId"; return $false
}

function Tail-Logs($deviceId) {
    Info "Starting logcat for Unity (Ctrl+C to stop)"
    adb -s $deviceId logcat -c
    adb -s $deviceId logcat -s Unity
}

# Main
Write-Host "TattooVision Quick Deploy (ASCII)" -ForegroundColor Green
if (-not (Ensure-Adb)) { exit 1 }
if (-not (Build-App)) { exit 1 }

$devices = Get-Devices
if (-not $devices -or $devices.Count -eq 0) { Err "No devices connected"; exit 1 }
Ok ("Devices: " + ($devices -join ', '))

foreach ($d in $devices) {
    if (-not (Install-App $d)) { continue }
    Launch-App $d | Out-Null
    $ans = Read-Host "Tail logs for ${d}? (y/N)"
    if ($ans -eq 'y' -or $ans -eq 'Y') { Tail-Logs $d }
}

Ok "Done"
