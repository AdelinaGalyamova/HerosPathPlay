using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class AuthUIScript : MonoBehaviour
{
    public bool isRegisterMode = false;

    public TMP_InputField nikInputField;
    public TMP_InputField passwordInputField;
    public TMP_InputField emailInputField;

    public Button actionButton;
    public Button backButton;

    public TextMeshProUGUI errorText;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI actionButtonText;

    private void Start()
    {
        AudioManager.Instance?.PlayMenuMusic();
        if (titleText != null)  titleText.text = isRegisterMode ? "Регистрация" : "Вход";

        if (actionButtonText != null) actionButtonText.text = isRegisterMode ? "Зарегистрироваться" : "Войти";

        if (emailInputField != null) emailInputField.gameObject.SetActive(isRegisterMode);

        if (actionButton != null) actionButton.onClick.AddListener(OnActionButton);

        if (backButton != null) backButton.onClick.AddListener(GoBack);

        if (!isRegisterMode)
        {
            GameManager.Instance?.ClearToken();
        }
    }

    public void OnActionButton()
    {
        if (isRegisterMode)
            OnRegister();
        else
            OnLogin();
    }

    private void OnLogin()
    {
        string email = nikInputField?.text ?? "";
        string password = passwordInputField?.text ?? "";

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            if (errorText != null) errorText.text = "Заполните все поля";
            return;
        }

        if (errorText != null) errorText.text = "Вход...";

        ApiRequests.Login(email, password, OnLoginResponse);
    }

    private void OnLoginResponse(AuthResponse response)
    {
        if (response.status)
        {
            SceneManager.LoadScene("LevelSelect");
        }
        else
        {
            if (errorText != null) errorText.text = response.message;
        }
    }

    private void OnRegister()
    {
        string username = nikInputField?.text ?? "";
        string email = emailInputField?.text ?? "";
        string password = passwordInputField?.text ?? "";

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            if (errorText != null) errorText.text = "Заполните все поля";
            return;
        }

        if (errorText != null)
            errorText.text = "Регистрация...";

        ApiRequests.Register(username, email, password, OnRegisterResponse);
    }

    private void OnRegisterResponse(AuthResponse response)
    {
        if (response.status)
        {
            SceneManager.LoadScene("LevelSelect");
        }
        else
        {
            if (errorText != null) errorText.text = response.message;
        }
    }

    public void GoBack()
    {
        if (isRegisterMode) SceneManager.LoadScene("Authorization");
        else SceneManager.LoadScene("MainMenu");
    }
}