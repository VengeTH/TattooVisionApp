using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Editor script to automatically add SimpleDiagnostic to the Dashboard scene
/// </summary>
public class AddDiagnosticToScene
{
    [MenuItem("Tools/Add Diagnostic to Dashboard Scene")]
    static void AddDiagnostic()
    {
        // Open Dashboard scene
        string scenePath = "Assets/Scenes/Dashboard.unity";
        SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

        if (scene == null)
        {
            Debug.LogError("Dashboard scene not found at: " + scenePath);
            return;
        }

        // Open the scene
        EditorSceneManager.OpenScene(scenePath);

        // Find or create a GameObject to attach the diagnostic
        GameObject diagnosticObject = GameObject.Find("DiagnosticObject");

        if (diagnosticObject == null)
        {
            diagnosticObject = new GameObject("DiagnosticObject");
        }

        // Add the SimpleDiagnostic component if not already present
        SimpleDiagnostic diagnostic = diagnosticObject.GetComponent<SimpleDiagnostic>();
        if (diagnostic == null)
        {
            diagnostic = diagnosticObject.AddComponent<SimpleDiagnostic>();
            Debug.Log("✅ Added SimpleDiagnostic component to scene");
        }
        else
        {
            Debug.Log("ℹ️ SimpleDiagnostic component already exists");
        }

        // Mark scene as dirty and save
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

        Debug.Log("🎯 Dashboard scene updated with diagnostic component. Ready to build!");
    }
}
