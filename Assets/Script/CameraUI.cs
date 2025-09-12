using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARCameraUIManager : MonoBehaviour
{
    public GameObject arSession;
    public GameObject arOrigin;
    public GameObject cameraUIPanel;

    void OnEnable()
    {
        // Enable AR components
        arSession.SetActive(true);
        arOrigin.SetActive(true);

        // Hide UI panel background if needed
        cameraUIPanel.SetActive(false); // or keep true if you want overlay
    }

    void OnDisable()
    {
        // Optional: stop AR when leaving the camera panel
        arSession.SetActive(false);
        arOrigin.SetActive(false);

        cameraUIPanel.SetActive(true);
    }
}
