using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DoorScript : MonoBehaviour
{
    public Sprite openDoorSprite;
    public int requiredKeys = 1;

    private SpriteRenderer spriteRenderer;
    private bool isOpen = false;
    private bool isTransitioning = false;
    public string nextLevelName = "";

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.tag = "DoorOne";
    }

    private void Update()
    {
        if (!isOpen && UIScript.Instance != null && UIScript.Instance.GetKeysCount() >= requiredKeys)
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        AudioManager.Instance?.PlayOpenDoor();
        if (spriteRenderer != null && openDoorSprite != null)
        {
            spriteRenderer.sprite = openDoorSprite;
        }
    }


    public void TryEnter()
    {
        if (!isOpen || isTransitioning) return;

        isTransitioning = true;

        TileManager tm = FindFirstObjectByType<TileManager>();

        int totalMoves = tm != null
            ? tm.GetCurrentMoves()
            : 0;

        int stars = GameManager.Instance?.CalculateStars(totalMoves) ?? 1;
        int levelId = GameManager.Instance?.GetLevelId() ?? 1;

        GameManager.Instance?.ResetCarryOverMoves();

        ApiRequests.SaveProgress(levelId, stars, totalMoves, (response) =>
        {
            Debug.Log($"Сохранение прогресса: {response?.status}");

            if (levelId == 4)
            {
                CutsceneFlow.PlayThenLoad(2, "Victory");
            }
            else
            {
                UIScript.Instance?.ShowVictory(stars, totalMoves);
            }
        });
    }


    private IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(2f);

        string nextSceneName;
        int nextLevelId;

        if (!string.IsNullOrEmpty(nextLevelName))
        {
            nextSceneName = nextLevelName;
            string[] parts = nextLevelName.Split('_');
            if (parts.Length == 2 && int.TryParse(parts[1], out nextLevelId))
                AudioManager.Instance?.PlayLevelMusic(nextLevelId);
            else
                AudioManager.Instance?.PlayLevelMusic((GameManager.Instance?.GetLevelId() ?? 1) + 1);
        }
        else
        {
            int currentLevelId = GameManager.Instance?.GetLevelId() ?? 1;
            nextLevelId = currentLevelId + 1;
            nextSceneName = $"Level_{nextLevelId}";
            AudioManager.Instance?.PlayLevelMusic(nextLevelId);
        }

        SceneManager.LoadScene(nextSceneName);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TryEnter();
        }
    }
}