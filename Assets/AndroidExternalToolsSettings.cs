// Unity 6 Compatibility Fix for AndroidExternalToolsSettings
// This file MUST be in the root Assets folder to compile before packages

#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace UnityEditor.Android
{
    /// <summary>
    /// Compatibility shim for Unity 6 where AndroidExternalToolsSettings was removed.
    /// This provides a working implementation that gets the actual Android SDK paths.
    /// </summary>
    public static class AndroidExternalToolsSettings
    {
        private static string GetAndroidSDKPath()
        {
            // Try multiple methods to get Android SDK path
            
            // Method 1: From Unity preferences
            string sdkPath = EditorPrefs.GetString("AndroidSdkRoot");
            if (!string.IsNullOrEmpty(sdkPath) && Directory.Exists(sdkPath))
                return sdkPath;
            
            // Method 2: From environment variable
            sdkPath = Environment.GetEnvironmentVariable("ANDROID_HOME");
            if (!string.IsNullOrEmpty(sdkPath) && Directory.Exists(sdkPath))
                return sdkPath;
            
            // Method 3: Common default locations
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                sdkPath = Path.Combine(localAppData, "Android", "Sdk");
                if (Directory.Exists(sdkPath))
                    return sdkPath;
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                string home = Environment.GetEnvironmentVariable("HOME");
                sdkPath = Path.Combine(home, "Library", "Android", "sdk");
                if (Directory.Exists(sdkPath))
                    return sdkPath;
            }
            
            return string.Empty;
        }
        
        private static string GetJDKPath()
        {
            // Try to get JDK path
            string jdkPath = EditorPrefs.GetString("JdkPath");
            if (!string.IsNullOrEmpty(jdkPath))
                return jdkPath;
            
            // Try JAVA_HOME
            jdkPath = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (!string.IsNullOrEmpty(jdkPath))
                return jdkPath;
            
            return string.Empty;
        }
        
        public static string sdkRootPath 
        { 
            get { return GetAndroidSDKPath(); }
            set { EditorPrefs.SetString("AndroidSdkRoot", value); }
        }
        
        public static string jdkRootPath 
        { 
            get { return GetJDKPath(); }
            set { EditorPrefs.SetString("JdkPath", value); }
        }
        
        public static string ndkRootPath 
        { 
            get 
            { 
                string ndkPath = EditorPrefs.GetString("AndroidNdkRoot");
                if (string.IsNullOrEmpty(ndkPath))
                {
                    // Try to find NDK in SDK folder
                    string sdkPath = GetAndroidSDKPath();
                    if (!string.IsNullOrEmpty(sdkPath))
                    {
                        string ndkFolder = Path.Combine(sdkPath, "ndk");
                        if (Directory.Exists(ndkFolder))
                        {
                            // Get the latest NDK version
                            var ndkVersions = Directory.GetDirectories(ndkFolder);
                            if (ndkVersions.Length > 0)
                                return ndkVersions[ndkVersions.Length - 1];
                        }
                    }
                }
                return ndkPath;
            }
            set { EditorPrefs.SetString("AndroidNdkRoot", value); }
        }
        
        public static string gradlePath 
        { 
            get 
            { 
                // Try to find Gradle in Unity's installation
                string gradlePath = EditorPrefs.GetString("GradlePath");
                if (!string.IsNullOrEmpty(gradlePath))
                    return gradlePath;
                
                // Unity usually bundles Gradle
                string unityPath = EditorApplication.applicationPath;
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    string unityFolder = Path.GetDirectoryName(unityPath);
                    string gradleFolder = Path.Combine(unityFolder, "Data", "PlaybackEngines", "AndroidPlayer", "Tools", "gradle");
                    if (Directory.Exists(gradleFolder))
                        return gradleFolder;
                }
                
                return string.Empty;
            }
            set { EditorPrefs.SetString("GradlePath", value); }
        }
    }
}
#endif
