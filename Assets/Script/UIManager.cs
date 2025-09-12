using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registrationPanel;
    [SerializeField] private GameObject emailVerificationPanel;
    [SerializeField] private GameObject forgotPasswordPanel;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI emailVerificationText;
    [SerializeField] private TextMeshProUGUI forgotPasswordStatusText;
    [SerializeField] public TextMeshProUGUI countdownText;

    [Header("Password Toggle")]
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private Image eyeIconImage;
    [SerializeField] private Sprite eyeOpenSprite;
    [SerializeField] private Sprite eyeClosedSprite;

    [Header("Sign Up Password Toggle")]
    [SerializeField] private TMP_InputField signupPasswordInputField;
    [SerializeField] private TMP_InputField signupConfirmPasswordInputField;
    [SerializeField] private Image signupEyeButtonImage;
    [SerializeField] private Image signupConfirmEyeButtonImage; 

    private bool isSignupPasswordVisible = false;
    private bool isSignupConfirmPasswordVisible = false;
    private bool isPasswordVisible = false;
    private Coroutine countdownCoroutine;

    private void Awake()
    {
        CreateInstance();
    }

    private void CreateInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void ClearUI()
    {
        loginPanel.SetActive(false);
        registrationPanel.SetActive(false);
        emailVerificationPanel.SetActive(false);
        forgotPasswordPanel.SetActive(false);
    }

    public void OpenLoginPanel()
    {
        ClearUI();
        loginPanel.SetActive(true);
    }

    public void OpenRegistrationPanel()
    {
        ClearUI();
        registrationPanel.SetActive(true);
    }

    public void OpenForgotPasswordPanel()
    {
        ClearUI();
        forgotPasswordPanel.SetActive(true);
        forgotPasswordStatusText.text = ""; // Clear previous messages
    }

    public void ShowVerificationResponse(bool isEmailSent, string emailId, string errorMessage)
    {
        ClearUI();
        emailVerificationPanel.SetActive(true);

        if (isEmailSent)
        {
            emailVerificationText.text = $"Verification email sent to: {emailId}\nPlease verify your email within 90 seconds.";
            if (countdownCoroutine == null)
            {
                StartVerificationCountdown(90);
            }
        }
        else
        {
            emailVerificationText.text = $"Couldn't send email: {errorMessage}";
        }
    }

    public void StopCountdown()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
            countdownText.text = "";
        }
    }


    public void UpdateEmailVerificationText(string message)
    {
        emailVerificationText.text = message;
    }


    public void ShowForgotPasswordResponse(bool isSuccess, string message)
    {
        forgotPasswordStatusText.text = message;
    }

    public void StartVerificationCountdown(int seconds)
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }
        countdownCoroutine = StartCoroutine(VerificationCountdown(seconds));
    }

    private IEnumerator VerificationCountdown(int timeRemaining)
    {
        while (timeRemaining > 0)
        {
            countdownText.text = $"Time remaining: {timeRemaining}s";
            yield return new WaitForSeconds(1f);
            timeRemaining--;
        }

        countdownText.text = "Time expired. Please try again.";
    }

    public void TogglePasswordVisibility()
    {
        isPasswordVisible = !isPasswordVisible;

        passwordInputField.contentType = isPasswordVisible ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        passwordInputField.ForceLabelUpdate();

        eyeIconImage.sprite = isPasswordVisible ? eyeOpenSprite : eyeClosedSprite;
    }

    public void ToggleSignupPasswordVisibility()
    {
        isSignupPasswordVisible = !isSignupPasswordVisible;
        signupPasswordInputField.contentType = isSignupPasswordVisible ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        signupPasswordInputField.ForceLabelUpdate();
        signupEyeButtonImage.sprite = isSignupPasswordVisible ? eyeOpenSprite : eyeClosedSprite;
    }

    public void ToggleSignupConfirmPasswordVisibility()
    {
        isSignupConfirmPasswordVisible = !isSignupConfirmPasswordVisible;
        signupConfirmPasswordInputField.contentType = isSignupConfirmPasswordVisible ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        signupConfirmPasswordInputField.ForceLabelUpdate();
        signupConfirmEyeButtonImage.sprite = isSignupConfirmPasswordVisible ? eyeOpenSprite : eyeClosedSprite;
    }
}

