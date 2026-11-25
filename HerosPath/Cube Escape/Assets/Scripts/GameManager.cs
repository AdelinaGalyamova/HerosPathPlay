using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int currentLevelId = 1;
    private string authToken = "";
    
    private int carryOverMoves = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCarryOverMoves(int moves)
    {
        carryOverMoves = moves;
        Debug.Log($"CarryOver ходов: {carryOverMoves}");
    }

    public int GetCarryOverMoves() => carryOverMoves;

    public void ResetCarryOverMoves()
    {
        carryOverMoves = 0;
    }

    public void SetLevelId(int levelId) => currentLevelId = levelId;
    public int GetLevelId() => currentLevelId;

    public void SetToken(string token)
    {
        authToken = token;
        PlayerPrefs.SetString("auth_token", token);
        PlayerPrefs.Save();
    }

    public string GetToken()
    {
        if (string.IsNullOrEmpty(authToken))
        {
            authToken = PlayerPrefs.GetString("auth_token", "");
        }
        return authToken;
    }

    public void ClearToken()
    {
        authToken = "";
        PlayerPrefs.DeleteKey("auth_token");
        PlayerPrefs.Save();
    }

    public int CalculateStars(int moves)
    {
        int levelId = GetLevelId();

        switch (levelId)
        {
            case 1:
                if (moves <= 25) return 3;
                if (moves <= 32) return 2;
                return 1;
            case 2:
                if (moves <= 43) return 3;
                if (moves <= 48) return 2;
                return 1;
            case 3:
                if (moves <= 48) return 3;
                if (moves <= 54) return 2;
                return 1;
            case 4:
                if (moves <= 85) return 3;
                if (moves <= 93) return 2;
                return 1;
            default:
                if (moves <= 85) return 3;
                if (moves <= 93) return 2;
                return 1;
        }
    }
}