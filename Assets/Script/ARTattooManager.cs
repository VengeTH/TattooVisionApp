using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ARTattooManager : MonoBehaviour
{
    [Header("AR Components")]
    public ARRaycastManager arRaycastManager;
    public ARPlaneManager arPlaneManager;
    public Camera arCamera;
    
    [Header("Tattoo Settings")]
    public GameObject tattooPrefab; // Prefab for tattoo placement
    public float defaultTattooScale = 0.1f; // Default size in meters
    public float minScale = 0.05f;
    public float maxScale = 0.5f;
    
    [Header("UI References")]
    public Button applyTattooButton;
    public Slider scaleSlider;
    public Button rotateButton;
    public Button deleteButton;
    public GameObject tattooSelectionPanel; // Panel showing available tattoos
    public Transform tattooGridContainer; // Grid container for tattoo thumbnails
    
    [Header("Tattoo Library")]
    public List<Sprite> availableTattoos = new List<Sprite>(); // Tattoo designs
    private Sprite selectedTattooDesign;
    
    // Active tattoo management
    private GameObject currentTattooObject;
    private List<GameObject> placedTattoos = new List<GameObject>();
    private bool isPlacingTattoo = false;
    private Vector3 lastPlacementPosition;
    private Quaternion lastPlacementRotation;
    
    // Touch handling
    private float rotationSpeed = 10f;
    private bool isRotating = false;
    
    void Start()
    {
        // Initialize UI
        if (applyTattooButton != null)
        {
            applyTattooButton.onClick.AddListener(StartTattooPlacement);
        }
        
        if (scaleSlider != null)
        {
            scaleSlider.minValue = minScale;
            scaleSlider.maxValue = maxScale;
            scaleSlider.value = defaultTattooScale;
            scaleSlider.onValueChanged.AddListener(OnScaleChanged);
        }
        
        if (rotateButton != null)
        {
            rotateButton.onClick.AddListener(ToggleRotation);
        }
        
        if (deleteButton != null)
        {
            deleteButton.onClick.AddListener(DeleteCurrentTattoo);
        }
        
        // Ensure AR components are assigned
        if (arRaycastManager == null)
        {
            arRaycastManager = FindObjectOfType<ARRaycastManager>();
        }
        
        if (arPlaneManager == null)
        {
            arPlaneManager = FindObjectOfType<ARPlaneManager>();
        }
        
        if (arCamera == null)
        {
            arCamera = Camera.main;
        }
        
        // Hide controls initially
        ShowTattooControls(false);
        
        // Load available tattoos
        LoadAvailableTattoos();
    }
    
    void Update()
    {
        if (!isPlacingTattoo || currentTattooObject == null)
            return;
        
        // Handle touch input for placement
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            // Check if touch is over UI
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;
            
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
            {
                // Perform AR raycast to detect planes
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                if (arRaycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    ARRaycastHit hit = hits[0];
                    
                    // Update tattoo position
                    currentTattooObject.transform.position = hit.pose.position;
                    
                    // Orient tattoo to match plane normal
                    currentTattooObject.transform.rotation = Quaternion.LookRotation(hit.pose.up, hit.pose.forward);
                    
                    // Store placement info
                    lastPlacementPosition = hit.pose.position;
                    lastPlacementRotation = currentTattooObject.transform.rotation;
                    
                    // Make tattoo visible if it was hidden
                    if (!currentTattooObject.activeSelf)
                    {
                        currentTattooObject.SetActive(true);
                        ShowTattooControls(true);
                    }
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                // Confirm placement on touch release
                if (currentTattooObject.activeSelf)
                {
                    ConfirmTattooPlacement();
                }
            }
        }
        
        // Handle rotation if enabled
        if (isRotating && currentTattooObject != null)
        {
            currentTattooObject.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
    
    public void StartTattooPlacement()
    {
        // Show tattoo selection panel if no design is selected
        if (selectedTattooDesign == null)
        {
            ShowTattooSelection();
            return;
        }
        
        // Create new tattoo object if needed
        if (currentTattooObject == null)
        {
            CreateTattooObject();
        }
        
        isPlacingTattoo = true;
        currentTattooObject.SetActive(false); // Hide until placed
        ShowTattooControls(false);
    }
    
    void CreateTattooObject()
    {
        // Create tattoo object from prefab or create a new one
        if (tattooPrefab != null)
        {
            currentTattooObject = Instantiate(tattooPrefab);
        }
        else
        {
            // Create a simple quad to display the tattoo
            currentTattooObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            currentTattooObject.name = "AR Tattoo";
            
            // Remove collider for AR object
            Destroy(currentTattooObject.GetComponent<Collider>());
        }
        
        // Apply selected tattoo texture
        ApplyTattooTexture(currentTattooObject, selectedTattooDesign);
        
        // Set initial scale
        currentTattooObject.transform.localScale = Vector3.one * defaultTattooScale;
        
        // Initially hide the object
        currentTattooObject.SetActive(false);
    }
    
    void ApplyTattooTexture(GameObject tattooObject, Sprite tattooSprite)
    {
        if (tattooSprite == null) return;
        
        Renderer renderer = tattooObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Create material with transparent shader
            Material tatMat = new Material(Shader.Find("Sprites/Default"));
            tatMat.mainTexture = tattooSprite.texture;
            
            // Enable transparency
            tatMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            tatMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            tatMat.SetInt("_ZWrite", 0);
            tatMat.DisableKeyword("_ALPHATEST_ON");
            tatMat.EnableKeyword("_ALPHABLEND_ON");
            tatMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            tatMat.renderQueue = 3000;
            
            renderer.material = tatMat;
        }
    }
    
    void ConfirmTattooPlacement()
    {
        if (currentTattooObject == null) return;
        
        // Add to placed tattoos list
        placedTattoos.Add(currentTattooObject);
        
        // Reset for next placement
        currentTattooObject = null;
        isPlacingTattoo = false;
        ShowTattooControls(false);
        
        // Optionally show success message
        Debug.Log("Tattoo placed successfully!");
    }
    
    void OnScaleChanged(float value)
    {
        if (currentTattooObject != null)
        {
            currentTattooObject.transform.localScale = Vector3.one * value;
        }
    }
    
    void ToggleRotation()
    {
        isRotating = !isRotating;
        
        // Update button visual state if needed
        if (rotateButton != null)
        {
            ColorBlock colors = rotateButton.colors;
            colors.normalColor = isRotating ? Color.green : Color.white;
            rotateButton.colors = colors;
        }
    }
    
    void DeleteCurrentTattoo()
    {
        if (currentTattooObject != null)
        {
            Destroy(currentTattooObject);
            currentTattooObject = null;
            isPlacingTattoo = false;
            ShowTattooControls(false);
        }
    }
    
    public void DeleteAllTattoos()
    {
        foreach (GameObject tattoo in placedTattoos)
        {
            if (tattoo != null)
                Destroy(tattoo);
        }
        placedTattoos.Clear();
        
        if (currentTattooObject != null)
        {
            Destroy(currentTattooObject);
            currentTattooObject = null;
        }
        
        isPlacingTattoo = false;
        ShowTattooControls(false);
    }
    
    void ShowTattooControls(bool show)
    {
        if (scaleSlider != null)
            scaleSlider.gameObject.SetActive(show);
        
        if (rotateButton != null)
            rotateButton.gameObject.SetActive(show);
        
        if (deleteButton != null)
            deleteButton.gameObject.SetActive(show);
    }
    
    void ShowTattooSelection()
    {
        if (tattooSelectionPanel != null)
        {
            tattooSelectionPanel.SetActive(true);
            PopulateTattooGrid();
        }
    }
    
    void LoadAvailableTattoos()
    {
        // Load tattoos from Resources folder or from uploaded images
        // This method should be integrated with ImageUploader
        
        // For now, load sample tattoos from Resources
        Sprite[] resourceTattoos = Resources.LoadAll<Sprite>("Tattoos");
        if (resourceTattoos != null && resourceTattoos.Length > 0)
        {
            availableTattoos.AddRange(resourceTattoos);
        }
    }
    
    void PopulateTattooGrid()
    {
        if (tattooGridContainer == null) return;
        
        // Clear existing items
        foreach (Transform child in tattooGridContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create buttons for each available tattoo
        foreach (Sprite tattoo in availableTattoos)
        {
            GameObject buttonObj = new GameObject("TattooButton");
            buttonObj.transform.SetParent(tattooGridContainer);
            
            // Add image component
            Image img = buttonObj.AddComponent<Image>();
            img.sprite = tattoo;
            
            // Add button component
            Button btn = buttonObj.AddComponent<Button>();
            Sprite capturedTattoo = tattoo; // Capture for lambda
            btn.onClick.AddListener(() => SelectTattooDesign(capturedTattoo));
            
            // Set size
            RectTransform rect = buttonObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 100);
        }
    }
    
    public void SelectTattooDesign(Sprite design)
    {
        selectedTattooDesign = design;
        
        // Hide selection panel
        if (tattooSelectionPanel != null)
        {
            tattooSelectionPanel.SetActive(false);
        }
        
        // Start placement
        StartTattooPlacement();
    }
    
    // Method to add uploaded images as tattoos
    public void AddUploadedTattoo(Sprite tattooSprite)
    {
        if (!availableTattoos.Contains(tattooSprite))
        {
            availableTattoos.Add(tattooSprite);
        }
    }
    
    // Method to integrate with skin detection
    public void PlaceTattooOnSkin(Vector3 skinPosition, Vector3 skinNormal)
    {
        if (currentTattooObject == null)
        {
            CreateTattooObject();
        }
        
        currentTattooObject.transform.position = skinPosition;
        currentTattooObject.transform.rotation = Quaternion.LookRotation(skinNormal);
        currentTattooObject.SetActive(true);
        ShowTattooControls(true);
    }
}
