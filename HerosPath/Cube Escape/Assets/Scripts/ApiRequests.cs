using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ApiRequests : MonoBehaviour
{
    private static string apiUrl = "http://localhost:5127/api/"; 

    public static string AuthToken
    {
        get => GameManager.Instance?.GetToken() ?? "";
        set => GameManager.Instance?.SetToken(value);
    }

    public static void Register(string username, string email, string password, Action<AuthResponse> callback)
    {
        if (GameManager.Instance == null)
        {
            callback?.Invoke(new AuthResponse { status = false, message = "GameManager не найден" });
            return;
        }

        var request = new RegisterRequest
        {
            username = username,
            email = email,
            password = password
        };
        GameManager.Instance.StartCoroutine(RegisterCoroutine(callback, request));
    }

    public static void Login(string email, string password, Action<AuthResponse> callback)
    {
        if (GameManager.Instance == null)
        {
            callback?.Invoke(new AuthResponse { status = false, message = "GameManager не найден" });
            return;
        }

        var request = new LoginRequest
        {
            email = email,
            password = password
        };
        GameManager.Instance.StartCoroutine(LoginCoroutine(callback, request));
    }

    public static void Logout(Action<LogoutResponse> callback)
    {
        if (GameManager.Instance == null)
        {
            callback?.Invoke(new LogoutResponse { status = false });
            return;
        }
        GameManager.Instance.StartCoroutine(LogoutCoroutine(callback));
    }

    public static void GetUser(Action<UserResponse> callback)
    {
        string token = AuthToken;
        if (string.IsNullOrEmpty(token))
        {
            callback?.Invoke(new UserResponse { status = false });
            return;
        }
        GameManager.Instance.StartCoroutine(GetUserCoroutine(callback, token));
    }

    public static void GetLevels(Action<LevelsResponse> callback)
    {
        string token = AuthToken;
        if (string.IsNullOrEmpty(token))
        {
            callback?.Invoke(new LevelsResponse { status = false });
            return;
        }
        GameManager.Instance.StartCoroutine(GetLevelsCoroutine(callback, token));
    }

    public static void GetProgress(Action<ProgressResponse> callback)
    {
        string token = AuthToken;
        if (string.IsNullOrEmpty(token))
        {
            callback?.Invoke(new ProgressResponse { status = false });
            return;
        }
        GameManager.Instance.StartCoroutine(GetProgressCoroutine(callback, token));
    }

    public static void SaveProgress(int levelId, int stars, int moves, Action<SaveProgressResponse> callback)
    {
        string token = AuthToken;
        Debug.Log($"levelId: {levelId}, stars: {stars}, moves: {moves}");

        if (string.IsNullOrEmpty(token))
        {
            callback?.Invoke(new SaveProgressResponse { status = false });
            return;
        }

        var request = new SaveProgressRequest
        {
            token = token,
            levelId = levelId,
            stars = stars,
            moves = moves
        };

        string jsonData = JsonUtility.ToJson(request);

        GameManager.Instance.StartCoroutine(SaveProgressCoroutine(callback, request));
    }

    private static IEnumerator RegisterCoroutine(Action<AuthResponse> callback, RegisterRequest request)
    {
        string jsonData = JsonUtility.ToJson(request);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest webRequest = new UnityWebRequest(apiUrl + "HerosPath/register", "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log($"Register response: {jsonResponse}");
                AuthResponse response = JsonUtility.FromJson<AuthResponse>(jsonResponse);
                if (response.status)
                {
                    AuthToken = response.token;
                }
                callback?.Invoke(response);
            }
            else
            {
                Debug.LogError($"Register error: {webRequest.error}");
                callback?.Invoke(new AuthResponse { status = false, message = "Ошибка соединения" });
            }
        }
    }

    private static IEnumerator LoginCoroutine(Action<AuthResponse> callback, LoginRequest request)
    {
        string jsonData = JsonUtility.ToJson(request);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest webRequest = new UnityWebRequest(apiUrl + "HerosPath/login", "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log($"Login response: {jsonResponse}");
                AuthResponse response = JsonUtility.FromJson<AuthResponse>(jsonResponse);
                if (response.status)
                {
                    AuthToken = response.token;
                }
                callback?.Invoke(response);
            }
            else
            {
                Debug.LogError($"Login error: {webRequest.error}");
                callback?.Invoke(new AuthResponse { status = false, message = "Ошибка соединения" });
            }
        }
    }

    private static IEnumerator LogoutCoroutine(Action<LogoutResponse> callback)
    {
        string token = AuthToken;
        string jsonData = JsonUtility.ToJson(new { token });
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest webRequest = new UnityWebRequest(apiUrl + "HerosPath/logout", "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                LogoutResponse response = JsonUtility.FromJson<LogoutResponse>(jsonResponse);
                if (response.status)
                {
                    AuthToken = "";
                }
                callback?.Invoke(response);
            }
            else
            {
                callback?.Invoke(new LogoutResponse { status = false });
            }
        }
    }

    private static IEnumerator GetUserCoroutine(Action<UserResponse> callback, string token)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl + $"HerosPath/user/{token}"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log($"GetUser JSON: {jsonResponse}");

                UserResponse response = JsonUtility.FromJson<UserResponse>(jsonResponse);
                callback?.Invoke(response);
            }
            else
            {
                Debug.LogError($"GetUser error: {request.error}");
                callback?.Invoke(new UserResponse { status = false });
            }
        }
    }

    private static IEnumerator GetLevelsCoroutine(Action<LevelsResponse> callback, string token)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl + $"HerosPath/levels/{token}"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log($"GetLevels JSON: {jsonResponse}");

                LevelsResponse response = JsonUtility.FromJson<LevelsResponse>(jsonResponse);
                callback?.Invoke(response);
            }
            else
            {
                Debug.LogError($"GetLevels error: {request.error}");
                callback?.Invoke(new LevelsResponse { status = false });
            }
        }
    }

    private static IEnumerator GetProgressCoroutine(Action<ProgressResponse> callback, string token)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl + $"HerosPath/progress/{token}"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                ProgressResponse response = JsonUtility.FromJson<ProgressResponse>(jsonResponse);
                callback?.Invoke(response);
            }
            else
            {
                callback?.Invoke(new ProgressResponse { status = false });
            }
        }
    }

    private static IEnumerator SaveProgressCoroutine(Action<SaveProgressResponse> callback, SaveProgressRequest request)
    {
        string jsonData = JsonUtility.ToJson(request);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        Debug.Log($"URL: {apiUrl}HerosPath/save-progress");

        using (UnityWebRequest webRequest = new UnityWebRequest(apiUrl + "HerosPath/save-progress", "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            Debug.Log($"Результат: {webRequest.result}");
            Debug.Log($"Код ответа: {webRequest.responseCode}");
            Debug.Log($"Ответ: {webRequest.downloadHandler.text}");

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                SaveProgressResponse response = JsonUtility.FromJson<SaveProgressResponse>(jsonResponse);
                callback?.Invoke(response);
            }
            else
            {
                Debug.LogError($"Ошибка: {webRequest.error}");
                callback?.Invoke(new SaveProgressResponse { status = false });
            }
        }
    }
}

[Serializable]
public class RegisterRequest
{
    public string username;
    public string email;
    public string password;
}

[Serializable]
public class LoginRequest
{
    public string email;
    public string password;
}

[Serializable]
public class SaveProgressRequest
{
    public string token;
    public int levelId;
    public int stars;
    public int moves;
}

[Serializable]
public class AuthResponse
{
    public bool status;
    public string token;
    public string message;
    public int userId;
    public string username;
    public string email;
}

[Serializable]
public class LogoutResponse
{
    public bool status;
    public string message;
}

[Serializable]
public class UserResponse
{
    public bool status;
    public int userId;
    public string username;
    public string email;
}

[Serializable]
public class LevelData
{
    public int id;
    public string name;
    public int levelNumber;
    public bool isLocked;
    public LevelProgressData progress;
}

[Serializable]
public class LevelProgressData
{
    public int stars;
    public int bestMoves;
    public bool isCompleted;
}

[Serializable]
public class LevelsResponse
{
    public bool status;
    public LevelData[] levels;
}

[Serializable]
public class ProgressItem
{
    public int levelId;
    public string levelName;
    public int stars;
    public int bestMoves;
    public bool isCompleted;
    public string completedAt;
}

[Serializable]
public class ProgressResponse
{
    public bool status;
    public int totalStars;
    public int completedLevels;
    public int totalLevels;
    public ProgressItem[] progress;
}

[Serializable]
public class SaveProgressResponse
{
    public bool status;
    public string message;
    public int totalStars;
}