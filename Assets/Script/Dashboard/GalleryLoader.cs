using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryLoader : MonoBehaviour
{
    [Header("Gallery Setup")]
    public Transform contentContainer;      // The Scroll View content for gallery
    public GameObject imageTemplate;        // Disabled image prefab in editor
    public string resourceFolder = "TattooPreDesign";

    [Header("Popup")]
    public GameObject popupPanel;
    public Image popupImage;
    public Button tryDesignButton;

    [Header("Favorite")]
    public Button globalFavoriteButton;     // Heart button (outside prefab)
    public Image globalHeartImage;          // Image on the heart button
    public Sprite heartUnfilled;            // Empty heart
    public Sprite heartFilled;              // Filled heart

    [Header("Panels")]
    public GameObject galleryPanel;
    public GameObject tryDesignPanel;
    public Image tryDesignImage;

    [Header("Profile/Favorites Panel")]
    public Transform profileContentContainer;  // Where favorites go

    [Header("Notification")]
    public GameObject notifPanel;           // Panel that shows notification text
    public Text notifText;                  // Text for notifications
    public float notifDuration = 2f;

    private Sprite selectedSprite;
    private bool isFavorite = false;

    private Dictionary<Sprite, GameObject> favoriteItems = new Dictionary<Sprite, GameObject>();

    void Start()
    {
        popupPanel.SetActive(false);
        tryDesignPanel.SetActive(false);
        notifPanel.SetActive(false);

        globalFavoriteButton.onClick.AddListener(OnGlobalFavoriteClicked);

        LoadImages();
    }

    void LoadImages()
    {
        Sprite[] images = Resources.LoadAll<Sprite>(resourceFolder);
        Debug.Log("Loaded images: " + images.Length);

        if (images.Length == 0)
        {
            Debug.LogError("No images found in Resources/" + resourceFolder);
            return;
        }

        foreach (Sprite sprite in images)
        {
            GameObject imgObj = Instantiate(imageTemplate, contentContainer);
            imgObj.SetActive(true);

            Image img = imgObj.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = sprite;
            }

            Button btn = imgObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => OnImageClicked(sprite));
            }
        }
    }

    void OnImageClicked(Sprite sprite)
    {
        selectedSprite = sprite;
        popupImage.sprite = sprite;
        popupPanel.SetActive(true);

        // Update heart state
        isFavorite = favoriteItems.ContainsKey(sprite);
        globalHeartImage.sprite = isFavorite ? heartFilled : heartUnfilled;
    }

    public void OnTryDesignClicked()
    {
        popupPanel.SetActive(false);
        galleryPanel.SetActive(false);
        tryDesignPanel.SetActive(true);
        tryDesignImage.sprite = selectedSprite;
    }

    public void OnClosePopup()
    {
        popupPanel.SetActive(false);
    }

    public void OnBackToGallery()
    {
        tryDesignPanel.SetActive(false);
        galleryPanel.SetActive(true);
    }

    void OnGlobalFavoriteClicked()
    {
        if (selectedSprite == null) return;

        if (!isFavorite)
        {
            isFavorite = true;
            globalHeartImage.sprite = heartFilled;
            ShowNotification("Added to favorites");
            AddToProfile(selectedSprite);
        }
        else
        {
            isFavorite = false;
            globalHeartImage.sprite = heartUnfilled;
            ShowNotification("Removed from favorites");
            RemoveFromProfile(selectedSprite);
        }
    }

    void AddToProfile(Sprite sprite)
    {
        if (favoriteItems.ContainsKey(sprite)) return;

        GameObject imgObj = Instantiate(imageTemplate, profileContentContainer);
        imgObj.SetActive(true);

        Image img = imgObj.GetComponent<Image>();
        if (img != null)
        {
            img.sprite = sprite;
        }

        favoriteItems.Add(sprite, imgObj);
    }

    void RemoveFromProfile(Sprite sprite)
    {
        if (favoriteItems.ContainsKey(sprite))
        {
            Destroy(favoriteItems[sprite]);
            favoriteItems.Remove(sprite);
        }
    }

    void ShowNotification(string message)
    {
        StopAllCoroutines();
        StartCoroutine(ShowNotifCoroutine(message));
    }

    System.Collections.IEnumerator ShowNotifCoroutine(string message)
    {
        notifText.text = message;
        notifPanel.SetActive(true);
        yield return new WaitForSeconds(notifDuration);
        notifPanel.SetActive(false);
    }
}
