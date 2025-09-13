using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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

    [Header("Configuration")]
    [SerializeField] private bool useFirebase = false; // * Set to true if Firebase is installed
    [SerializeField] private string guestUsername = "Guest User";

    void Start()
    {
        hamburgerMenuPanel.SetActive(false);

        hamburgerButton.onClick.AddListener(ToggleMenu);
        changeProfileButton.onClick.AddListener(OpenProfilePicturePicker);
        logoutButton.onClick.AddListener(Logout);
        backButton.onClick.AddListener(CloseMenu); // ← Assign back button logic

        // * Initialize user display
        InitializeUserDisplay();
    }

    void InitializeUserDisplay()
    {
        if (useFirebase && CheckFirebaseUser())
        {
            // * Try to get user info from Firebase
            try
            {
#if FIREBASE_INSTALLED
                var auth = Firebase.FirebaseAuth.DefaultInstance;
                var user = auth.CurrentUser;

                if (user != null)
                {
                    if (!string.IsNullOrEmpty(user.DisplayName))
                    {
                        usernameText.text = user.DisplayName;
                    }
                    else if (!string.IsNullOrEmpty(user.Email))
                    {
                        usernameText.text = user.Email;
                    }
                    else
                    {
                        usernameText.text = "Welcome!";
                    }
                    Debug.Log("ProfileMenuManager: User authenticated");
                    return;
                }
#endif
            }
            catch (System.Exception e)
            {
                Debug.LogError($"ProfileMenuManager: Firebase error: {e.Message}");
            }
        }

        // * Fallback to guest mode
        usernameText.text = guestUsername;
        Debug.Log("ProfileMenuManager: Running in guest mode");
    }

    bool CheckFirebaseUser()
    {
#if FIREBASE_INSTALLED
        try
        {
            var auth = Firebase.FirebaseAuth.DefaultInstance;
            return auth != null && auth.CurrentUser != null;
        }
        catch
        {
            return false;
        }
#else
        return false;
#endif
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

        // * Try Firebase logout if available
        if (useFirebase)
        {
            try
            {
#if FIREBASE_INSTALLED
                var auth = Firebase.FirebaseAuth.DefaultInstance;
                if (auth != null)
                {
                    auth.SignOut();
                    Debug.Log("ProfileMenuManager: Firebase logout successful");
                }
#endif
            }
            catch (System.Exception e)
            {
                Debug.LogError($"ProfileMenuManager: Firebase logout error: {e.Message}");
            }
        }

        // * Reset to guest mode and reload dashboard
        usernameText.text = guestUsername;
        Debug.Log("ProfileMenuManager: Returned to guest mode");

        // * Optionally reload the scene to reset state
        // SceneManager.LoadScene("Dashboard");
    }
}
