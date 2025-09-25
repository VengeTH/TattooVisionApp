@echo off
echo Starting Unity Build Process...

REM Kill any existing Unity processes more aggressively
echo Killing any existing Unity processes...
taskkill /F /IM Unity.exe /T >nul 2>&1
taskkill /F /IM "Unity Hub.exe" /T >nul 2>&1
taskkill /F /FI "WINDOWTITLE eq Unity*" /T >nul 2>&1

REM Wait a moment for processes to fully terminate
timeout /t 3 /nobreak >nul

REM Check if Unity executable exists
if not exist "D:\Tools\Unity\6000.0.48f1-x86_64\Editor\Unity.exe" (
    echo ERROR: Unity executable not found at D:\Tools\Unity\6000.0.48f1-x86_64\Editor\Unity.exe
    echo Please update the path in this script to match your Unity installation.
    pause
    exit /b 1
)

echo Building Android APK...
"D:\Tools\Unity\6000.0.48f1-x86_64\Editor\Unity.exe" ^
-batchmode ^
-nographics ^
-projectPath "%~dp0" ^
-executeMethod BuildPlayer.PerformAndroidBuild ^
-quit ^
-logFile build.log

if %ERRORLEVEL% EQU 0 (
    echo Build completed successfully!
    echo Check Build\ directory for output files.
) else (
    echo Build failed! Check build.log for details.
)

pause
