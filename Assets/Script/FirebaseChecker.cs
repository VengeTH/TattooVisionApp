using UnityEngine;

/// <summary>
/// * Checks for Firebase availability and defines compilation symbols
/// * This script runs in the editor to set up conditional compilation
/// </summary>
#if UNITY_EDITOR
[UnityEditor.InitializeOnLoad]
public class FirebaseChecker
{
    static FirebaseChecker()
    {
        CheckFirebaseAvailability();
    }

    static void CheckFirebaseAvailability()
    {
        // * Check if Firebase assemblies are available
        var firebaseAppType = System.Type.GetType("Firebase.FirebaseApp, Firebase.App");
        var firebaseAuthType = System.Type.GetType("Firebase.FirebaseAuth, Firebase.Auth");

        if (firebaseAppType != null && firebaseAuthType != null)
        {
            Debug.Log("FirebaseChecker: Firebase assemblies found, defining FIREBASE_INSTALLED");
            DefineSymbol("FIREBASE_INSTALLED");
        }
        else
        {
            Debug.Log("FirebaseChecker: Firebase assemblies not found");
            UndefineSymbol("FIREBASE_INSTALLED");
        }
    }

    static void DefineSymbol(string symbol)
    {
        var buildTargetGroup = UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup;
        var defines = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

        if (!defines.Contains(symbol))
        {
            defines += ";" + symbol;
            UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
            Debug.Log($"FirebaseChecker: Defined symbol: {symbol}");
        }
    }

    static void UndefineSymbol(string symbol)
    {
        var buildTargetGroup = UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup;
        var defines = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

        if (defines.Contains(symbol))
        {
            defines = defines.Replace(";" + symbol, "").Replace(symbol, "");
            UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
            Debug.Log($"FirebaseChecker: Undefined symbol: {symbol}");
        }
    }
}
#endif



