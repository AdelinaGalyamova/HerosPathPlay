using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour
{
    public static UIScript Instance;

    public TextMeshProUGUI keyCountText;
    public TextMeshProUGUI moveCountText;

    public GameObject pausePanel;
    public GameObject victoryPanel;

    public TextMeshProUGUI victoryStarsText;
    public TextMeshProUGUI victoryMovesText;

    public GameObject star1;
    public GameObject star2;
    public GameObject star3;

    public Button pauseButton;      
    public Button resumeButton;     
    public Button restartButton;    
    public Button menuButton;       

    public Button nextLevelButton;  
    public Button victoryRestartButton; 
    public Button victoryMenuButton;    

    private bool isPaused = false;
    private int currentKeys = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (pauseButton != null) pauseButton.onClick.AddListener(TogglePause);
        if (resumeButton != null) resumeButton.onClick.AddListener(TogglePause);
        if (restartButton != null) restartButton.onClick.AddListener(RestartLevel);
        if (menuButton != null) menuButton.onClick.AddListener(GoToLevelSelect);

        if (nextLevelButton != null) nextLevelButton.onClick.AddListener(NextLevel);
        if (victoryRestartButton != null) victoryRestartButton.onClick.AddListener(RestartLevel);
        if (victoryMenuButton != null) victoryMenuButton.onClick.AddListener(GoToLevelSelect);

        if (pausePanel != null) pausePanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void ToggleSound()
    {
        AudioManager.Instance?.ToggleMute();
    }

    public void UpdateMoveCount(int count)
    {
        if (moveCountText != null) moveCountText.text = $"Ходов: {count}";
    }

    public void AddKey()
    {
        currentKeys++;
        if (keyCountText != null) keyCountText.text = $"Ключей: {currentKeys}";
    }

    public int GetKeysCount()
    {
        return currentKeys;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (pausePanel != null) pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        AudioManager.Instance?.ResumeMusic();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToLevelSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelect");
    }


    public void NextLevel()
    {
        Time.timeScale = 1f;

        int currentLevelId = GameManager.Instance?.GetLevelId() ?? 1;
        int nextLevelId = currentLevelId + 1;

        if (nextLevelId == 3)
        {
            CutsceneFlow.PlayThenLoad(1, "Level_3");
            return;
        }

        string nextLevelName = $"Level_{nextLevelId}";

        if (Application.CanStreamedLevelBeLoaded(nextLevelName))
        {
            AudioManager.Instance?.PlayLevelMusic(nextLevelId);
            SceneManager.LoadScene(nextLevelName);
        }
        else
        {
            AudioManager.Instance?.PlayMenuMusic();
            SceneManager.LoadScene("Victory");
        }
    }


    public void ShowVictory(int stars, int moves)
    {
        Time.timeScale = 0f;
        AudioManager.Instance?.PauseMusic();
        AudioManager.Instance?.PlayVictory();

        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        if (victoryStarsText != null)
            victoryStarsText.text = $"Звезды: {stars}";

        if (victoryMovesText != null)
            victoryMovesText.text = $"{moves}";

        if (star1 != null) star1.SetActive(stars >= 1);
        if (star2 != null) star2.SetActive(stars >= 2);
        if (star3 != null) star3.SetActive(stars >= 3);
    }
}