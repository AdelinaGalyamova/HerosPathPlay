using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        AudioManager.Instance?.PlayButtonClick();
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneByIndex(int sceneIndex)
    {
        AudioManager.Instance?.PlayButtonClick();
        SceneManager.LoadScene(sceneIndex);
    }

    public void GoToAutorization()
    {
        AudioManager.Instance?.PlayButtonClick();
        AudioManager.Instance?.PlayMenuMusic();
        SceneManager.LoadScene("Authorization");
    }

    public void GoToRegistration()
    {
        AudioManager.Instance?.PlayButtonClick();
        AudioManager.Instance?.PlayMenuMusic();
        SceneManager.LoadScene("Registration");
    }

    public void GoToMainMenu()
    {
        AudioManager.Instance?.PlayButtonClick();
        AudioManager.Instance?.PlayMenuMusic();
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToLevelSelect()
    {
        AudioManager.Instance?.PlayButtonClick();
        AudioManager.Instance?.PlayMenuMusic();
        SceneManager.LoadScene("LevelSelect");
    }

    public void GoToLevel(int levelNumber)
    {
        AudioManager.Instance?.PlayButtonClick();
        AudioManager.Instance?.PlayLevelMusic(levelNumber);
        SceneManager.LoadScene("Level_" + levelNumber);
    }

    public void ReloadCurrentScene()
    {
        AudioManager.Instance?.PlayButtonClick();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}