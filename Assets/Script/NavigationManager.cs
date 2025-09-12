using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationManager : MonoBehaviour
{
    public GameObject cameraPanel;   // Canvas > Camera
    public GameObject dashboardPanel;
    public GameObject profilePanel;
    public GameObject galleryPanel;
    public GameObject[] otherPanels; // Assign Dashboard, Profile, etc.
    
    // AR Components
    public ARTattooManager arTattooManager;
    public ARCameraUIManager arCameraUIManager;
    
    // Current selected tattoo for AR
    private Sprite selectedTattooForAR;

    public void OpenCameraPanel()
    {
        if (cameraPanel != null)
            cameraPanel.SetActive(true);

        foreach (GameObject panel in otherPanels)
        {
            if (panel != null)
                panel.SetActive(false);
        }
        
        // Initialize AR if available
        if (arCameraUIManager != null)
        {
            arCameraUIManager.enabled = true;
        }
        
        // If a tattoo was selected, set it in AR manager
        if (selectedTattooForAR != null && arTattooManager != null)
        {
            arTattooManager.SelectTattooDesign(selectedTattooForAR);
            selectedTattooForAR = null; // Clear after use
        }
    }
    
    public void OpenDashboard()
    {
        if (dashboardPanel != null)
            dashboardPanel.SetActive(true);
        
        HideOtherPanels(dashboardPanel);
    }
    
    public void OpenProfile()
    {
        if (profilePanel != null)
            profilePanel.SetActive(true);
        
        HideOtherPanels(profilePanel);
    }
    
    public void OpenGallery()
    {
        if (galleryPanel != null)
            galleryPanel.SetActive(true);
        
        HideOtherPanels(galleryPanel);
    }
    
    public void OpenARCameraWithTattoo(Sprite tattooSprite)
    {
        selectedTattooForAR = tattooSprite;
        OpenCameraPanel();
    }
    
    void HideOtherPanels(GameObject activePanel)
    {
        if (cameraPanel != null && cameraPanel != activePanel)
            cameraPanel.SetActive(false);
        
        if (dashboardPanel != null && dashboardPanel != activePanel)
            dashboardPanel.SetActive(false);
        
        if (profilePanel != null && profilePanel != activePanel)
            profilePanel.SetActive(false);
        
        if (galleryPanel != null && galleryPanel != activePanel)
            galleryPanel.SetActive(false);
        
        foreach (GameObject panel in otherPanels)
        {
            if (panel != null && panel != activePanel)
                panel.SetActive(false);
        }
    }
    
    // Scene-based navigation methods
    public void LoadARCameraScene()
    {
        SceneManager.LoadScene("ARCamera");
    }
    
    public void LoadDashboardScene()
    {
        SceneManager.LoadScene("Dashboard");
    }
    
    public void LoadGalleryScene()
    {
        SceneManager.LoadScene("Gallery");
    }
    
    public void LoadProfileScene()
    {
        SceneManager.LoadScene("Profile");
    }
}
