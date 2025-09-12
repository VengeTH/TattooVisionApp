using UnityEngine;
using System.Collections;

public class ARTattoo : MonoBehaviour
{
    [Header("Tattoo Properties")]
    public Sprite tattooSprite;
    public float fadeInDuration = 0.5f;
    public bool autoAdjustToSkin = true;
    
    [Header("Visual Effects")]
    public bool enablePulseEffect = false;
    public float pulseSpeed = 1f;
    public float pulseAmount = 0.1f;
    
    private Renderer tattooRenderer;
    private Material tattoMaterial;
    private float currentAlpha = 0f;
    private float targetAlpha = 1f;
    private Vector3 originalScale;
    private bool isPlaced = false;
    
    void Start()
    {
        tattooRenderer = GetComponent<Renderer>();
        if (tattooRenderer == null)
        {
            tattooRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        
        originalScale = transform.localScale;
        
        // Start with transparent tattoo
        SetupMaterial();
        StartCoroutine(FadeIn());
    }
    
    void SetupMaterial()
    {
        if (tattooSprite != null && tattooRenderer != null)
        {
            // Create material with transparent shader
            tattoMaterial = new Material(Shader.Find("Standard"));
            
            // Set rendering mode to transparent
            tattoMaterial.SetFloat("_Mode", 3); // Transparent mode
            tattoMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            tattoMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            tattoMaterial.SetInt("_ZWrite", 0);
            tattoMaterial.DisableKeyword("_ALPHATEST_ON");
            tattoMaterial.EnableKeyword("_ALPHABLEND_ON");
            tattoMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            tattoMaterial.renderQueue = 3000;
            
            // Apply tattoo texture
            tattoMaterial.mainTexture = tattooSprite.texture;
            
            // Set initial transparency
            Color color = tattoMaterial.color;
            color.a = currentAlpha;
            tattoMaterial.color = color;
            
            tattooRenderer.material = tattoMaterial;
        }
    }
    
    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            currentAlpha = Mathf.Lerp(0f, targetAlpha, elapsedTime / fadeInDuration);
            
            if (tattoMaterial != null)
            {
                Color color = tattoMaterial.color;
                color.a = currentAlpha;
                tattoMaterial.color = color;
            }
            
            yield return null;
        }
        
        isPlaced = true;
    }
    
    void Update()
    {
        if (isPlaced && enablePulseEffect)
        {
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            transform.localScale = originalScale * pulse;
        }
    }
    
    public void SetTattooSprite(Sprite sprite)
    {
        tattooSprite = sprite;
        SetupMaterial();
    }
    
    public void SetTransparency(float alpha)
    {
        targetAlpha = Mathf.Clamp01(alpha);
        
        if (tattoMaterial != null)
        {
            Color color = tattoMaterial.color;
            color.a = targetAlpha;
            tattoMaterial.color = color;
        }
        
        currentAlpha = targetAlpha;
    }
    
    public void SetSize(float scale)
    {
        originalScale = Vector3.one * scale;
        transform.localScale = originalScale;
    }
    
    public void EnableGlow(bool enable)
    {
        if (tattoMaterial != null)
        {
            if (enable)
            {
                tattoMaterial.EnableKeyword("_EMISSION");
                tattoMaterial.SetColor("_EmissionColor", Color.white * 0.3f);
            }
            else
            {
                tattoMaterial.DisableKeyword("_EMISSION");
            }
        }
    }
    
    public void AdjustToSkinTone(Color skinColor)
    {
        if (tattoMaterial != null && autoAdjustToSkin)
        {
            // Blend tattoo color with skin tone for more realistic appearance
            Color tattooColor = tattoMaterial.color;
            Color blendedColor = Color.Lerp(tattooColor, skinColor, 0.1f);
            blendedColor.a = tattooColor.a; // Preserve alpha
            tattoMaterial.color = blendedColor;
        }
    }
    
    public void Remove()
    {
        StartCoroutine(FadeOutAndDestroy());
    }
    
    IEnumerator FadeOutAndDestroy()
    {
        float elapsedTime = 0f;
        float startAlpha = currentAlpha;
        
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            currentAlpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeInDuration);
            
            if (tattoMaterial != null)
            {
                Color color = tattoMaterial.color;
                color.a = currentAlpha;
                tattoMaterial.color = color;
            }
            
            yield return null;
        }
        
        Destroy(gameObject);
    }
    
    void OnDestroy()
    {
        if (tattoMaterial != null)
        {
            Destroy(tattoMaterial);
        }
    }
}
