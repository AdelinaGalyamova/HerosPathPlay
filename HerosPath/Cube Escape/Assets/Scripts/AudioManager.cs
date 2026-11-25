using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip menuMusic;
    public AudioClip[] levelMusicClips; 

    public AudioClip stepSound;
    public AudioClip pushBoxSound;
    public AudioClip collectRingSound;
    public AudioClip collectKeySound;
    public AudioClip openBoxSound;
    public AudioClip openDoorSound;
    public AudioClip victorySound;
    public AudioClip buttonClickSound;

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private bool isMuted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.volume = 0.35f;
        sfxSource.loop = false;
        sfxSource.volume = 0.8f;
    }

    public void PlayMenuMusic()
    {
        if (menuMusic == null || musicSource == null) return;
        if (musicSource.clip == menuMusic && musicSource.isPlaying) return;
        musicSource.clip = menuMusic;
        musicSource.Play();
    }

    public void PlayLevelMusic(int levelId)
    {
        if (levelId <= 0 || levelId > levelMusicClips.Length) return;
        AudioClip clip = levelMusicClips[levelId - 1];
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource?.Stop();
    }

    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Pause();
    }

    public void ResumeMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
            musicSource.UnPause();
    }

    public void PlayStep() => PlaySFX(stepSound);
    public void PlayPushBox() => PlaySFX(pushBoxSound);
    public void PlayCollectRing() => PlaySFX(collectRingSound);
    public void PlayCollectKey() => PlaySFX(collectKeySound);
    public void PlayOpenBox() => PlaySFX(openBoxSound);
    public void PlayOpenDoor() => PlaySFX(openDoorSound);
    public void PlayButtonClick() => PlaySFX(buttonClickSound);
    public void PlayVictory() => PlaySFX(victorySound);

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null || isMuted) return;
        sfxSource.PlayOneShot(clip);
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        if (musicSource != null) musicSource.mute = isMuted;
        if (sfxSource != null) sfxSource.mute = isMuted;
        Debug.Log($"Çâóê: {(isMuted ? "ÂÛÊË" : "ÂÊË")}");
    }

    public bool IsMuted() => isMuted;
}