using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using TMPro;
using System.Collections;
using System.IO;

public class ProfileMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject hamburgerMenuPanel;
    public Button hamburgerButton;
    public Button changeProfileButton;
    public Button logoutButton;
    public Button backButton; // ← Back button inside the hamburger menu

    [Header("User Info")]
    public TextMeshProUGUI usernameText;
    public Image profileImage;

    private FirebaseAuth auth;

    void Start()
    {
        hamburgerMenuPanel.SetActive(false);

        hamburgerButton.onClick.AddListener(ToggleMenu);
        changeProfileButton.onClick.AddListener(OpenProfilePicturePicker);
        logoutButton.onClick.AddListener(Logout);
        backButton.onClick.AddListener(CloseMenu); // ← Assign back button logic

        auth = FirebaseAuth.DefaultInstance;
        if (auth.CurrentUser != null && !string.IsNullOrEmpty(auth.CurrentUser.DisplayName))
        {
            usernameText.text = auth.CurrentUser.DisplayName;
        }
        else
        {
            usernameText.text = "Welcome!";
        }
    }

    void ToggleMenu()
    {
        hamburgerMenuPanel.SetActive(!hamburgerMenuPanel.activeSelf);
    }

    void CloseMenu()
    {
        hamburgerMenuPanel.SetActive(false);
    }

    void OpenProfilePicturePicker()
    {
        hamburgerMenuPanel.SetActive(false);

        NativeFilePicker.PickFile((path) =>
        {
            if (path != null)
            {
                Debug.Log("Picked image: " + path);
                StartCoroutine(LoadImage(path));
            }
        }, new string[] { "image/*" });
    }

    IEnumerator LoadImage(string path)
    {
        byte[] imageBytes = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);

        profileImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        profileImage.preserveAspect = true;

        // Optional: Save path or upload to Firebase here
        yield return null;
    }

    void Logout()
    {
        hamburgerMenuPanel.SetActive(false);

        Debug.Log("Logging out...");
        auth.SignOut();
        SceneManager.LoadScene("AppScene"); // Replace with your login scene name
    }
}
