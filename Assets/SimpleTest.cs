using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ULTRA SIMPLE test - just shows text to verify basic app launch works
/// </summary>
public class SimpleTest : MonoBehaviour
{
    void Start()
    {
        // Create a simple canvas with text
        GameObject canvasGO = new GameObject("TestCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        // Create background
        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(canvasGO.transform);
        Image bg = bgGO.AddComponent<Image>();
        bg.color = Color.green;
        RectTransform bgRect = bgGO.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // Create text
        GameObject textGO = new GameObject("TestText");
        textGO.transform.SetParent(canvasGO.transform);
        TextMeshProUGUI text = textGO.AddComponent<TextMeshProUGUI>();
        text.text = "🎉 APP WORKS!\n\nDevice: " + SystemInfo.deviceModel +
                   "\nOS: " + SystemInfo.operatingSystem +
                   "\nUnity: " + Application.unityVersion +
                   "\n\n✅ Basic launch successful!";
        text.fontSize = 48;
        text.color = Color.black;
        text.alignment = TextAlignmentOptions.Center;

        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        Debug.Log("🚀 SIMPLE TEST: App launched successfully!");
    }
}
