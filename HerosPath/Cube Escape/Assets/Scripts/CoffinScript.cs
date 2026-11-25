using UnityEngine;
using UnityEngine.SceneManagement;

public class CoffinScript : MonoBehaviour
{
    public string nextSceneName;
    public int requiredKeys = 0;
    private bool isUnlocked => UIScript.Instance == null ||
                               UIScript.Instance.GetKeysCount() >= requiredKeys;

    public void TryEnter()
    {
        if (!isUnlocked)
        {
            Debug.Log($"Coffin заперт! Нужно {requiredKeys} ключей.");
            return;
        }

        if (string.IsNullOrEmpty(nextSceneName)) return;

        TileManager tm = FindFirstObjectByType<TileManager>();
        int currentMoves = tm != null ? tm.GetCurrentMoves() : 0;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCarryOverMoves(currentMoves);
            Debug.Log($"Coffin: передаём {currentMoves} ходов в {nextSceneName}");
        }

        SceneManager.LoadScene(nextSceneName);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            TryEnter();
    }
}