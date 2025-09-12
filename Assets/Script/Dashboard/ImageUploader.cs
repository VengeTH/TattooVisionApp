using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NativeFilePickerNamespace;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class ImageUploader : MonoBehaviour
{
    [Header("Containers")]
    public Transform profileContainer;
    public Transform galleryContainer;

    [Header("Notifications")]
    public TMP_Text uploadNotificationText;

    [Header("Popup for Profile")]
    public GameObject popupPanel_Profile;
    public Image popupImage_Profile;

    [Header("Popup for Gallery")]
    public GameObject popupPanel_Gallery;
    public Image popupImage_Gallery;
    
    [Header("AR Integration")]
    public ARTattooManager arTattooManager; // Reference to AR tattoo manager
    public List<Sprite> uploadedTattoos = new List<Sprite>(); // Store uploaded tattoos

    public void PickImage()
    {
        if (NativeFilePicker.IsFilePickerBusy())
            return;

        NativeFilePicker.PickFile((path) =>
        {
            if (path == null)
            {
                Debug.Log("Operation cancelled");
                return;
            }

            byte[] imageData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(imageData))
            {
                Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

                AddImageToContainer(galleryContainer, newSprite, isGallery: true);
                AddImageToContainer(profileContainer, newSprite, isGallery: false);
                
                // Add to uploaded tattoos list
                uploadedTattoos.Add(newSprite);
                
                // Add to AR tattoo manager if available
                if (arTattooManager != null)
                {
                    arTattooManager.AddUploadedTattoo(newSprite);
                }

                StartCoroutine(ShowUploadNotification());
            }
            else
            {
                Debug.LogError("Failed to load image");
            }
        },
        new string[] { "image/png", "image/jpeg" });
    }

    void AddImageToContainer(Transform container, Sprite sprite, bool isGallery)
    {
        Transform template = container.GetChild(0); // Use disabled first child as template
        GameObject newImageObj = Instantiate(template.gameObject, container);
        newImageObj.SetActive(true);

        Image img = newImageObj.GetComponent<Image>();
        if (img != null)
        {
            img.sprite = sprite;

            Button btn = newImageObj.GetComponent<Button>();
            if (btn == null)
                btn = newImageObj.AddComponent<Button>();

            btn.onClick.RemoveAllListeners();

            if (isGallery)
                btn.onClick.AddListener(() => ShowPopupGallery(sprite));
            else
                btn.onClick.AddListener(() => ShowPopupProfile(sprite));
        }

    }

    void ShowPopupProfile(Sprite sprite)
    {
        popupImage_Profile.sprite = sprite;
        popupPanel_Profile.SetActive(true);
    }

    void ShowPopupGallery(Sprite sprite)
    {
        popupImage_Gallery.sprite = sprite;
        popupPanel_Gallery.SetActive(true);
        
        // Add option to use this tattoo in AR
        AddARButtonToPopup(popupPanel_Gallery, sprite);
    }

    public void ClosePopupProfile()
    {
        popupPanel_Profile.SetActive(false);
    }

    public void ClosePopupGallery()
    {
        popupPanel_Gallery.SetActive(false);
    }

    private IEnumerator ShowUploadNotification()
    {
        uploadNotificationText.alpha = 1f;
        yield return new WaitForSeconds(2f);
        uploadNotificationText.alpha = 0f;
    }
    
    void AddARButtonToPopup(GameObject popup, Sprite tattooSprite)
    {
        // Look for existing AR button or create one
        Transform arButton = popup.transform.Find("UseInARButton");
        
        if (arButton == null)
        {
            // Create AR button if it doesn't exist
            GameObject buttonObj = new GameObject("UseInARButton");
            buttonObj.transform.SetParent(popup.transform);
            
            // Add button component
            Button btn = buttonObj.AddComponent<Button>();
            
            // Add text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform);
            TMP_Text text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = "Use in AR";
            text.alignment = TextAlignmentOptions.Center;
            
            // Position button
            RectTransform rect = buttonObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0);
            rect.anchorMax = new Vector2(0.5f, 0);
            rect.anchoredPosition = new Vector2(0, 50);
            rect.sizeDelta = new Vector2(150, 40);
            
            arButton = buttonObj.transform;
        }
        
        // Set up button click event
        Button arBtn = arButton.GetComponent<Button>();
        if (arBtn != null)
        {
            arBtn.onClick.RemoveAllListeners();
            arBtn.onClick.AddListener(() => UseInAR(tattooSprite));
        }
    }
    
    void UseInAR(Sprite tattooSprite)
    {
        if (arTattooManager != null)
        {
            // Select this tattoo in AR manager
            arTattooManager.SelectTattooDesign(tattooSprite);
            
            // Close popup
            ClosePopupGallery();
            ClosePopupProfile();
            
            // Switch to AR camera scene if needed
            // NavigationManager could handle this
            Debug.Log("Tattoo selected for AR placement");
        }
    }
    
    public List<Sprite> GetUploadedTattoos()
    {
        return uploadedTattoos;
    }

}
