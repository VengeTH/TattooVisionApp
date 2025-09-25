using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using System.Linq;
#if UNITY_XR_MANAGEMENT
using UnityEngine.XR.Management;
using UnityEditor.XR.Management;
#endif

/// <summary>
/// ! Build automation script for TattooVision AR App
/// * Handles Android builds with all necessary AR and OpenCV configurations
/// TODO: Add iOS build support
/// </summary>
public class BuildPlayer : MonoBehaviour
{
    // * Build paths
    private static readonly string BUILD_PATH = Path.Combine(Application.dataPath, "..", "Build");
    private static readonly string APK_NAME = "TattooVision.apk";
    private static readonly string AAB_NAME = "TattooVision.aab";
    
    // * Package settings
    private static readonly string PACKAGE_NAME = "com.DefaultCompany.Firebase";
    private static readonly string PRODUCT_NAME = "TattooVision";
    
    [MenuItem("Build/Android APK (Development)")]
    public static void BuildAndroidDevelopment()
    {
        PerformAndroidBuild(true, false);
    }
    
    [MenuItem("Build/Android APK (Release)")]
    public static void BuildAndroidRelease()
    {
        PerformAndroidBuild(false, false);
    }
    
    [MenuItem("Build/Android App Bundle (Release)")]
    public static void BuildAndroidAppBundle()
    {
        PerformAndroidBuild(false, true);
    }
    
    /// <summary>
    /// * Main Android build method called by build.bat
    /// </summary>
    public static void PerformAndroidBuild()
    {
        PerformAndroidBuild(true, false);
    }
    
    /// <summary>
    /// * Performs Android build with comprehensive AR configuration
    /// </summary>
    /// <param name="isDevelopment">Build as development build</param>
    /// <param name="isAppBundle">Build as App Bundle (.aab) instead of APK</param>
    public static void PerformAndroidBuild(bool isDevelopment, bool isAppBundle)
    {
        Debug.Log("Starting TattooVision Android build...");
        
        // * Setup build directory
        if (!Directory.Exists(BUILD_PATH))
        {
            Directory.CreateDirectory(BUILD_PATH);
            Debug.Log($"Created build directory: {BUILD_PATH}");
        }
        
        // * Configure Android settings
        ConfigureAndroidSettings();
        
        // * Configure AR settings
        ConfigureARSettings();
        
        // * Configure build options
        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = GetScenePaths(),
            locationPathName = Path.Combine(BUILD_PATH, isAppBundle ? AAB_NAME : APK_NAME),
            target = BuildTarget.Android,
            options = GetBuildOptions(isDevelopment)
        };
        
        Debug.Log($"Building to: {buildOptions.locationPathName}");
        Debug.Log($"Development build: {isDevelopment}");
        Debug.Log($"App Bundle: {isAppBundle}");
        
        // * Set App Bundle option
        EditorUserBuildSettings.buildAppBundle = isAppBundle;
        
        // * Perform build
        BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
        BuildSummary summary = report.summary;
        
        // * Report results
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build succeeded!");
            Debug.Log($"Build size: {(summary.totalSize / (1024 * 1024)):F2} MB");
            Debug.Log($"Build time: {summary.totalTime}");
            
            // * Log build artifacts
            foreach (var step in report.steps)
            {
                if (step.name.Contains("Compile"))
                {
                    Debug.Log($"{step.name}: {step.duration}");
                }
            }
        }
        else
        {
            Debug.LogError($"Build failed: {summary.result}");
            
            // * Log build errors
            foreach (var step in report.steps)
            {
                if (step.messages.Any(m => m.type == LogType.Error))
                {
                    Debug.LogError($"Errors in {step.name}:");
                    foreach (var message in step.messages.Where(m => m.type == LogType.Error))
                    {
                        Debug.LogError($"   {message.content}");
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// * Configure Android-specific settings for AR app
    /// </summary>
    private static void ConfigureAndroidSettings()
    {
        Debug.Log("Configuring Android settings...");
        
        // * Basic Android settings
        PlayerSettings.applicationIdentifier = PACKAGE_NAME;
        PlayerSettings.productName = PRODUCT_NAME;
        PlayerSettings.companyName = "TattooVision Studios";
        
        // * Android API levels
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24; // API 24 for ARCore
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel34; // Target Android 14 for best store compatibility and features
        
        // * Graphics and rendering
        PlayerSettings.Android.renderOutsideSafeArea = true;
        PlayerSettings.use32BitDisplayBuffer = true;
        
        // * Set graphics APIs (OpenGL ES 3.0+ required for ARCore)
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new UnityEngine.Rendering.GraphicsDeviceType[] 
        { 
            UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3,
            UnityEngine.Rendering.GraphicsDeviceType.Vulkan 
        });
        
        // * Scripting settings
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7 | AndroidArchitecture.X86_64;
        
        // * Permissions for AR and camera
        PlayerSettings.Android.forceInternetPermission = true;
        PlayerSettings.Android.forceSDCardPermission = true;
        
        // * Orientation
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.allowedAutorotateToPortrait = true;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        
        Debug.Log("Android settings configured");
    }
    
    /// <summary>
    /// * Configure AR-specific settings
    /// </summary>
    private static void ConfigureARSettings()
    {
        Debug.Log("Configuring AR settings...");
        
        try
        {
#if UNITY_XR_MANAGEMENT
            // * Try to configure XR Management
            var buildTargetGroup = BuildTargetGroup.Android;
            
            // * Enable ARCore if available
            var xrSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(buildTargetGroup);
            if (xrSettings != null && xrSettings.Manager != null)
            {
                Debug.Log("XR Management found, configuring ARCore...");
                
                // * This would require XR Management package to be properly configured
                var loaders = xrSettings.Manager.activeLoaders;
                Debug.Log($"Active XR Loaders: {loaders.Count}");
                
                foreach (var loader in loaders)
                {
                    Debug.Log($"   - {loader.name}");
                }
            }
            else
            {
                Debug.LogWarning("XR Management not found. Make sure AR Foundation is properly set up.");
            }
#else
            Debug.LogWarning("XR Management package not installed. AR features may not work properly.");
#endif
            
            Debug.Log("AR settings configured");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Could not configure AR settings: {e.Message}");
            Debug.LogWarning("Make sure AR Foundation and XR Plug-in Management packages are installed");
        }
    }
    
    /// <summary>
    /// * Get all scenes to include in build
    /// </summary>
    private static string[] GetScenePaths()
    {
        var scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
        
        Debug.Log($"Including {scenes.Length} scenes:");
        foreach (var scene in scenes)
        {
            Debug.Log($"   - {Path.GetFileNameWithoutExtension(scene)}");
        }
        
        return scenes;
    }
    
    /// <summary>
    /// * Configure build options based on build type
    /// </summary>
    private static BuildOptions GetBuildOptions(bool isDevelopment)
    {
        BuildOptions options = BuildOptions.None;
        
        if (isDevelopment)
        {
            options |= BuildOptions.Development;
            options |= BuildOptions.AllowDebugging;
            options |= BuildOptions.ConnectWithProfiler;
            
            Debug.Log("Development build options enabled");
        }
        else
        {
            // * Release optimizations
            Debug.Log("Release build options enabled");
        }
        
        return options;
    }
    
    /// <summary>
    /// * Development build method for quick iteration
    /// </summary>
    public static void PerformAndroidDevelopmentBuild()
    {
        Debug.Log("Starting development build...");
        PerformAndroidBuild(true, false);
    }
    
    /// <summary>
    /// * Release build method for distribution
    /// </summary>
    public static void PerformAndroidReleaseBuild()
    {
        Debug.Log("Starting release build...");
        PerformAndroidBuild(false, false);
    }
    
    /// <summary>
    /// ? Custom build with specific package name (for testing different variants)
    /// </summary>
    public static void BuildWithPackageName(string packageName)
    {
        var originalPackageName = PlayerSettings.applicationIdentifier;
        
        try
        {
            PlayerSettings.applicationIdentifier = packageName;
            PerformAndroidBuild(true, false);
        }
        finally
        {
            PlayerSettings.applicationIdentifier = originalPackageName;
        }
    }
    
    /// <summary>
    /// * Clean build artifacts
    /// </summary>
    [MenuItem("Build/Clean Build Directory")]
    public static void CleanBuildDirectory()
    {
        if (Directory.Exists(BUILD_PATH))
        {
            Directory.Delete(BUILD_PATH, true);
            Debug.Log("Build directory cleaned");
        }
        
        Directory.CreateDirectory(BUILD_PATH);
        Debug.Log("Fresh build directory created");
    }
    
    /// <summary>
    /// * Show current build configuration
    /// </summary>
    [MenuItem("Build/Show Build Info")]
    public static void ShowBuildInfo()
    {
        Debug.Log("Current Build Configuration:");
        Debug.Log($"   Package Name: {PlayerSettings.applicationIdentifier}");
        Debug.Log($"   Product Name: {PlayerSettings.productName}");
        Debug.Log($"   Version: {PlayerSettings.bundleVersion}");
        Debug.Log($"   Build Number: {PlayerSettings.Android.bundleVersionCode}");
        Debug.Log($"   Min SDK: {PlayerSettings.Android.minSdkVersion}");
        Debug.Log($"   Target SDK: {PlayerSettings.Android.targetSdkVersion}");
        Debug.Log($"   Scripting Backend: {PlayerSettings.GetScriptingBackend(NamedBuildTarget.Android)}");
        Debug.Log($"   Target Architectures: {PlayerSettings.Android.targetArchitectures}");
        
        var graphicsAPIs = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
        Debug.Log($"   Graphics APIs: {string.Join(", ", graphicsAPIs)}");
    }
}
