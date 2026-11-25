
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutscenePlayer : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;
    public VideoClip[] videos;

    [Header("UI")]
    public RawImage videoDisplay;
    public Button replayButton;
    public Button skipButton;

    private string targetScene;
    private bool isFinished = false;

    private void Start()
    {
        AudioManager.Instance?.StopMusic();
        targetScene = CutsceneFlow.GetTargetScene();

        if (videoPlayer == null || videos == null ||
            CutsceneFlow.GetVideoIndex() >= videos.Length)
        {
            Debug.LogError("Не настроен VideoPlayer или отсутствует видео!");
            LoadTargetScene();
            return;
        }

        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.waitForFirstFrame = true;

        videoPlayer.loopPointReached += OnVideoFinished;

        if (replayButton != null)
            replayButton.onClick.AddListener(ReplayVideo);

        if (skipButton != null)
            skipButton.onClick.AddListener(SkipVideo);

        videoPlayer.clip = videos[CutsceneFlow.GetVideoIndex()];

        if (videoDisplay != null)
            videoDisplay.texture = videoPlayer.targetTexture;

        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.Prepare();
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        vp.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        LoadTargetScene();
    }

    public void ReplayVideo()
    {
        if (isFinished) return;

        videoPlayer.Stop();
        videoPlayer.Play();
    }

    public void SkipVideo()
    {
        LoadTargetScene();
    }

    private void LoadTargetScene()
    {
        if (isFinished) return;

        isFinished = true;

        if (videoPlayer != null)
            videoPlayer.Stop();

        videoPlayer.loopPointReached -= OnVideoFinished;

        if (!string.IsNullOrEmpty(targetScene))
        {
            SceneManager.LoadScene(targetScene);
        }
        else
        {
            Debug.LogError("Не указана следующая сцена!");
        }
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
            videoPlayer.prepareCompleted -= OnVideoPrepared;
        }
    }
}
