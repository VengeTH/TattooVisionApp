using UnityEngine;

/// <summary>
/// SIMPLE diagnostic script - add this to ANY scene to test if the app launches
/// </summary>
public class SimpleDiagnostic : MonoBehaviour
{
    void Start()
    {
        Debug.Log("🚀 APP LAUNCHED SUCCESSFULLY!");
        Debug.Log($"📱 Device: {SystemInfo.deviceModel}");
        Debug.Log($"🎮 Unity: {Application.unityVersion}");
        Debug.Log($"📷 Camera Permission: {Application.HasUserAuthorization(UserAuthorization.WebCam)}");

        // Create a visible UI element to prove the app is working
        CreateTestUI();
    }

    void CreateTestUI()
    {
        // Create a simple canvas with text
        GameObject canvasGO = new GameObject("Diagnostic Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        GameObject textGO = new GameObject("Diagnostic Text");
        textGO.transform.SetParent(canvasGO.transform);
        var text = textGO.AddComponent<UnityEngine.UI.Text>();
        text.text = "✅ APP WORKING!\n🚀 LAUNCHED OK\n📱 " + SystemInfo.deviceModel;
        text.fontSize = 40;
        text.color = Color.green;
        text.alignment = TextAnchor.MiddleCenter;

        var rectTransform = textGO.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;

        Debug.Log("🎯 DIAGNOSTIC UI CREATED - If you see green text, app is working!");
    }

    void Update()
    {
        if (Time.frameCount % 300 == 0) // Every 5 seconds
        {
            Debug.Log($"⏰ App still running - {Time.time:F1}s");
        }
    }
}
