using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

public class LevelSelectUIScript : MonoBehaviour
{
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI totalStarsText;

    public Button[] levelButtons;

    public Sprite starFilled;  
    public Sprite starEmpty;   

    private void Start()
    {
        AudioManager.Instance?.PlayMenuMusic();
        string token = GameManager.Instance?.GetToken();
        if (string.IsNullOrEmpty(token))
        {
            SceneManager.LoadScene("Authorization");
            return;
        }

        ApiRequests.GetUser(OnUserReceived);
        ApiRequests.GetLevels(OnLevelsReceived);
    }

    private void OnUserReceived(UserResponse response)
    {
        if (response.status && usernameText != null)
        {
            usernameText.text = response.username;
        }
        else
        {
            GameManager.Instance?.ClearToken();
            SceneManager.LoadScene("Authorization");
        }
    }

    private void OnLevelsReceived(LevelsResponse response)
    {
        if (!response.status || response.levels == null)
        {
            Debug.LogError("Не удалось получить уровни!");
            return;
        }

        for (int i = 0; i < response.levels.Length && i < levelButtons.Length; i++)
        {
            int levelId = response.levels[i].id;
            int stars = response.levels[i].progress?.stars ?? 0;
            bool isLocked = response.levels[i].isLocked;

            levelButtons[i].interactable = !isLocked;

            TextMeshProUGUI buttonText = levelButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = $"{response.levels[i].name}\n{stars} звезд";
            }

            SetStarSprite(levelButtons[i].transform, "Star1", stars >= 1);
            SetStarSprite(levelButtons[i].transform, "Star2", stars >= 2);
            SetStarSprite(levelButtons[i].transform, "Star3", stars >= 3);

            levelButtons[i].onClick.RemoveAllListeners();
            levelButtons[i].onClick.AddListener(() => LoadLevel(levelId));
        }

        int totalStars = response.levels.Sum(l => l.progress?.stars ?? 0);
        if (totalStarsText != null)
            totalStarsText.text = $"Звезды: {totalStars}";
    }

    private void SetStarSprite(Transform buttonTransform, string starName, bool isActive)
    {
        Transform starTransform = buttonTransform.Find(starName);
        if (starTransform != null)
        {
            SpriteRenderer starSprite = starTransform.GetComponent<SpriteRenderer>();
            if (starSprite != null)
            {
                starSprite.sprite = isActive ? starFilled : starEmpty;
            }
        }
    }

    private void LoadLevel(int levelId)
    {
        GameManager.Instance?.SetLevelId(levelId);

        if (levelId == 3)
        {
            CutsceneFlow.PlayThenLoad(1, "Level_3");
            return;
        }

        if (levelId == 1)
        {
            CutsceneFlow.PlayThenLoad(0, "Level_1");
            return;
        }

        SceneManager.LoadScene($"Level_{levelId}");
    }

    public void Logout()
    {
        ApiRequests.Logout((response) => {
            GameManager.Instance?.ClearToken();
            SceneManager.LoadScene("MainMenu");
        });
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}