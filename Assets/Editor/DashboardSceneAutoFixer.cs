using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class DashboardSceneAutoFixer
{
    [MenuItem("Tools/Auto-Fix Dashboard Scene UI")]
    public static void AutoFixDashboardSceneUI()
    {
        string scenePath = "Assets/Scenes/Dashboard.unity";
        var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
        if (sceneAsset == null)
        {
            Debug.LogError($"Dashboard scene not found at {scenePath}");
            return;
        }
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);
        var canvases = Object.FindObjectsOfType<Canvas>();
        int fixedCount = 0;
        foreach (var canvas in canvases)
        {
            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                fixedCount++;
            }
            if (!canvas.gameObject.activeInHierarchy)
            {
                canvas.gameObject.SetActive(true);
                fixedCount++;
            }
            var scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1080, 1920);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 1f;
                fixedCount++;
            }
        }
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log($"DashboardSceneAutoFixer: Fixed {fixedCount} Canvas/UI settings in Dashboard scene.");
    }
}