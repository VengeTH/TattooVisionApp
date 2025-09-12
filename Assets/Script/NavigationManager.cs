using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    public GameObject cameraPanel;   // Canvas > Camera
    public GameObject[] otherPanels; // Assign Dashboard, Profile, etc.

    public void OpenCameraPanel()
    {
        cameraPanel.SetActive(true);

        foreach (GameObject panel in otherPanels)
            panel.SetActive(false);
    }
}
