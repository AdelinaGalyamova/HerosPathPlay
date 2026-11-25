
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CutsceneFlow
{
    private static int videoIndex;
    private static string targetScene;

    public static void PlayThenLoad(int index, string sceneName)
    {
        videoIndex = index;
        targetScene = sceneName;

        Time.timeScale = 1f;

        AudioManager.Instance?.StopMusic();

        SceneManager.LoadScene("Cutscene");
    }

    public static int GetVideoIndex()
    {
        return videoIndex;
    }

    public static string GetTargetScene()
    {
        return targetScene;
    }
}
