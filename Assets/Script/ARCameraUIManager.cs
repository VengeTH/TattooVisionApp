using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARCameraUIManager : MonoBehaviour
{
    public GameObject arSession;
    public GameObject arOrigin;
    public GameObject cameraUIPanel; // Optional: the white panel to show/hide

    void OnEnable()
    {
        // Start AR system
        arSession.SetActive(true);
        arOrigin.SetActive(true);

        // Optional: Hide panel background to show camera feed
        if (cameraUIPanel != null)
            cameraUIPanel.SetActive(false);
    }

    void OnDisable()
    {
        // Stop AR system
        arSession.SetActive(false);
        arOrigin.SetActive(false);

        if (cameraUIPanel != null)
            cameraUIPanel.SetActive(true);
    }
}
