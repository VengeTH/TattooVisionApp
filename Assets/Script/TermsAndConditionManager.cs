using UnityEngine;
using UnityEngine.UI;

public class TermsAndConditionsManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panelTermsPopup;                    // The Terms popup panel
    public ScrollRect scrollTermsContent;                 // The scrollable content
    public Toggle togglePopupAgreement;                   // Checkbox inside popup
    public Toggle toggleMainAgreement;                    // Checkbox on create account screen
    public Button buttonCreateAccount;                    // Create Account button

    private bool hasScrolledToBottom = false;

    void Start()
    {
        // Initial state setup
        panelTermsPopup.SetActive(false);
        toggleMainAgreement.interactable = false;
        toggleMainAgreement.isOn = false;
        buttonCreateAccount.interactable = false;

        togglePopupAgreement.isOn = false;
        togglePopupAgreement.interactable = false;

        // Scroll and checkbox listeners
        scrollTermsContent.onValueChanged.AddListener(OnScroll);
        togglePopupAgreement.onValueChanged.AddListener(OnPopupCheckboxChecked);
    }

    // Called when the "Read Terms" button is clicked
    public void OpenTermsPopup()
    {
        panelTermsPopup.SetActive(true);
        scrollTermsContent.verticalNormalizedPosition = 1f; // scroll to top
        togglePopupAgreement.isOn = false;
        togglePopupAgreement.interactable = false;
        hasScrolledToBottom = false;
    }

    // Detects when user scrolls to bottom
    private void OnScroll(Vector2 scrollPosition)
    {
        if (!hasScrolledToBottom && scrollTermsContent.verticalNormalizedPosition <= 0.01f)
        {
            hasScrolledToBottom = true;
            togglePopupAgreement.interactable = true;
        }
    }

    // Detects checkbox interaction inside popup
    private void OnPopupCheckboxChecked(bool isOn)
    {
        if (isOn && hasScrolledToBottom)
        {
            ClosePopupAndEnableMainAgreement();
        }
    }

    // Finalize agreement and enable account creation
    private void ClosePopupAndEnableMainAgreement()
    {
        panelTermsPopup.SetActive(false);
        toggleMainAgreement.interactable = true;
        toggleMainAgreement.isOn = true;
        buttonCreateAccount.interactable = true;
    }
}
