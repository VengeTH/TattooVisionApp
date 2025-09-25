@echo off
echo TattooVision Quick Deploy Launcher
echo.

REM Check if PowerShell is available
powershell -Command "Write-Host 'PowerShell available' -ForegroundColor Green" 2>nul
if errorlevel 1 (
    echo PowerShell not available. Please install PowerShell.
    pause
    exit /b 1
)

echo Choose deployment option:
echo [1] Full Build + Deploy (Recommended)
echo [2] Build Only
echo [3] Deploy Only (existing APK)
echo [4] Emergency Device Check
echo.

set /p choice="Enter choice (1-4): "

if "%choice%"=="1" (
    echo Running full build and deploy...
    powershell -ExecutionPolicy Bypass -File "quick-deploy-ascii.ps1"
) else if "%choice%"=="2" (
    echo Building only...
    powershell -ExecutionPolicy Bypass -File "quick-deploy-ascii.ps1" -BuildOnly
) else if "%choice%"=="3" (
    echo Deploying existing APK...
    powershell -ExecutionPolicy Bypass -File "quick-deploy-ascii.ps1" -DeployOnly
) else if "%choice%"=="4" (
    echo Checking device and ADB connection...
    echo.
    echo Connected devices:
    adb devices
    echo.
    echo Device properties:
    for /f "tokens=1" %%i in ('adb devices ^| findstr device') do (
        echo Checking device: %%i
        adb -s %%i shell getprop ro.product.model
        adb -s %%i shell getprop ro.build.version.release
        adb -s %%i shell pm list packages | findstr "google.ar.core"
    )
    pause
) else (
    echo Invalid choice. Exiting.
    pause
)
