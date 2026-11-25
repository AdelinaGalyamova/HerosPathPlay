using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUDButtons : MonoBehaviour
{
    public Button pauseButton;
    public Button soundButton;
    public Button restartButton;
    public Button menuButton;
    public Button infoButton;

    public Sprite soundOnIcon;
    public Sprite soundOffIcon;

    public GameObject infoPanel; 
    public Button closeInfoButton;

    private bool isSoundOn = true;

    private void Start()
    {
        if (pauseButton != null) pauseButton.onClick.AddListener(TogglePause);
        if (soundButton != null) soundButton.onClick.AddListener(ToggleSound);
        if (restartButton != null) restartButton.onClick.AddListener(RestartLevel);
        if (menuButton != null) menuButton.onClick.AddListener(GoToLevelSelect);
        if (infoButton != null) infoButton.onClick.AddListener(ToggleInfoPanel);
        if (closeInfoButton != null) closeInfoButton.onClick.AddListener(CloseInfoPanel);

        if (infoPanel != null) infoPanel.SetActive(false);

        LoadSoundSettings();
        UpdateSoundIcon();
    }

    private void TogglePause()
    {
        UIScript.Instance?.TogglePause();
    }

    private void ToggleSound()
    {
        isSoundOn = !isSoundOn;

        AudioListener.volume = isSoundOn ? 1f : 0f;

        PlayerPrefs.SetInt("SoundOn", isSoundOn ? 1 : 0);
        PlayerPrefs.Save();

        UpdateSoundIcon();
    }

    private void UpdateSoundIcon()
    {
        if (soundButton != null && soundButton.image != null && soundOnIcon != null && soundOffIcon != null)
        {
            soundButton.image.sprite = isSoundOn ? soundOnIcon : soundOffIcon;
        }
    }

    private void LoadSoundSettings()
    {
        isSoundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
        AudioListener.volume = isSoundOn ? 1f : 0f;
    }

    private void RestartLevel()
    {
        Time.timeScale = 1f;

        TileManager tm = FindFirstObjectByType<TileManager>();
        if (tm != null) tm.ResetMoves();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoToLevelSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelect");
    }

    private void ToggleInfoPanel()
    {
        if (infoPanel != null)
            infoPanel.SetActive(!infoPanel.activeSelf);
    }

    public void CloseInfoPanel()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }
}