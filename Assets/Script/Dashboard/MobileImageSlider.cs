using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MobileImageSlider : MonoBehaviour
{
    public Image displayImage;
    private List<Sprite> imageSprites = new List<Sprite>();

    private int currentIndex = 0;
    private float changeInterval = 3f;
    private Coroutine autoRandomize;

    private Vector2 touchStartPos;
    private bool swiped = false;
    private float swipeThreshold = 50f; // pixels

    void Start()
    {
        // Load all PNGs from Resources/TattooPreDesign
        Sprite[] loadedSprites = Resources.LoadAll<Sprite>("TattooPreDesign");
        imageSprites.AddRange(loadedSprites);

        if (imageSprites.Count > 0)
        {
            displayImage.sprite = imageSprites[0];
            autoRandomize = StartCoroutine(RandomizeImage());
        }
        else
        {
            Debug.LogWarning("No sprites found in TattooPreDesign folder.");
        }
    }

    void Update()
    {
        HandleSwipeInput();
    }

    void HandleSwipeInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    swiped = false;
                    break;

                case TouchPhase.Moved:
                    float swipeDelta = touch.position.x - touchStartPos.x;

                    if (!swiped && Mathf.Abs(swipeDelta) > swipeThreshold)
                    {
                        StopCoroutine(autoRandomize);

                        if (swipeDelta > 0)
                            ShowPreviousImage();
                        else
                            ShowNextImage();

                        swiped = true;
                        autoRandomize = StartCoroutine(RandomizeImage());
                    }
                    break;
            }
        }
    }

    IEnumerator RandomizeImage()
    {
        while (true)
        {
            yield return new WaitForSeconds(changeInterval);
            int newIndex;
            do
            {
                newIndex = Random.Range(0, imageSprites.Count);
            } while (newIndex == currentIndex);

            currentIndex = newIndex;
            displayImage.sprite = imageSprites[currentIndex];
        }
    }

    void ShowNextImage()
    {
        currentIndex = (currentIndex + 1) % imageSprites.Count;
        displayImage.sprite = imageSprites[currentIndex];
    }

    void ShowPreviousImage()
    {
        currentIndex = (currentIndex - 1 + imageSprites.Count) % imageSprites.Count;
        displayImage.sprite = imageSprites[currentIndex];
    }
}
