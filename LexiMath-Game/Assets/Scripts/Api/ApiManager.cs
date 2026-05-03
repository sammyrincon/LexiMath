using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ApiManager : MonoBehaviour
{
    public static ApiManager Instance { get; private set; }

    private const string BASE_URL =
        "https://leximath.com.mx/api";

    private string _jwtToken = "";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetToken(string token)
    {
        _jwtToken = token;
    }

    public bool IsLoggedIn()
    {
        return !string.IsNullOrEmpty(_jwtToken);
    }

    public IEnumerator Post(string endpoint, object body,
        Action<string> onSuccess, Action<string> onError)
    {
        string jsonBody = JsonUtility.ToJson(body);
        string url = BASE_URL + endpoint;

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        if (!string.IsNullOrEmpty(_jwtToken))
            request.SetRequestHeader("Authorization", "Bearer " + _jwtToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            onSuccess?.Invoke(request.downloadHandler.text);
        else
            onError?.Invoke(request.error + " | " + request.downloadHandler.text);

        request.Dispose();
    }

    public IEnumerator Get(string endpoint,
        Action<string> onSuccess, Action<string> onError)
    {
        string url = BASE_URL + endpoint;

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");

        if (!string.IsNullOrEmpty(_jwtToken))
            request.SetRequestHeader("Authorization", "Bearer " + _jwtToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            onSuccess?.Invoke(request.downloadHandler.text);
        else
            onError?.Invoke(request.error + " | " + request.downloadHandler.text);

        request.Dispose();
    }

    public IEnumerator Patch(string endpoint, object body,
        Action<string> onSuccess, Action<string> onError)
    {
        string jsonBody = JsonUtility.ToJson(body);
        string url = BASE_URL + endpoint;

        UnityWebRequest request = new UnityWebRequest(url, "PATCH");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        if (!string.IsNullOrEmpty(_jwtToken))
            request.SetRequestHeader("Authorization", "Bearer " + _jwtToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            onSuccess?.Invoke(request.downloadHandler.text);
        else
            onError?.Invoke(request.error + " | " + request.downloadHandler.text);

        request.Dispose();
    }
}
