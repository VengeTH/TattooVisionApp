using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NativeFilePickerNamespace;
using System.IO;
using System.Collections;

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

}
