using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject homePanel;
    public GameObject profilePanel;
    public GameObject galleryPanel;
    public GameObject cameraPanel;

    // Add these for AR activation
    public GameObject arSession;     // Drag your AR Session GameObject here
    public GameObject arOrigin;      // Drag your XR Origin GameObject here
    public GameObject cameraUIPanel; // Optional: background UI panel inside cameraPanel

    public void OpenHome()
    {
        homePanel.SetActive(true);
        profilePanel.SetActive(false);
        galleryPanel.SetActive(false);
        cameraPanel.SetActive(false);
        DisableAR();
    }

    public void OpenProfile()
    {
        homePanel.SetActive(false);
        profilePanel.SetActive(true);
        galleryPanel.SetActive(false);
        cameraPanel.SetActive(false);
        DisableAR();
    }

    public void OpenGallery()
    {
        homePanel.SetActive(false);
        profilePanel.SetActive(false);
        galleryPanel.SetActive(true);
        cameraPanel.SetActive(false);
        DisableAR();
    }

    public void OpenCamera()
    {
        homePanel.SetActive(false);
        profilePanel.SetActive(false);
        galleryPanel.SetActive(false);
        cameraPanel.SetActive(true);
        EnableAR();
    }

    private void EnableAR()
    {
        arSession.SetActive(true);
        arOrigin.SetActive(true);

        if (cameraUIPanel != null)
            cameraUIPanel.SetActive(false); // Hide UI panel if desired
    }

    private void DisableAR()
    {
        arSession.SetActive(false);
        arOrigin.SetActive(false);

        if (cameraUIPanel != null)
            cameraUIPanel.SetActive(true); // Restore UI panel
    }
}
