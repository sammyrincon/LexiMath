using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// ApiManager — capa de red de LEXIMATH
/// Singleton que maneja todas las llamadas HTTP a la API REST.
/// Basado en UnityWebRequest según metodología del profesor.
/// </summary>
public class ApiManager : MonoBehaviour
{
    // ── Singleton ──────────────────────────────────────────────
    public static ApiManager Instance { get; private set; }

    // ── URL base de la API (ya desplegada en AWS) ──────────────
    private const string BASE_URL = 
        "https://vyw108sp52.execute-api.us-east-1.amazonaws.com/prod";

    // ── JWT Token (se guarda al hacer login) ───────────────────
    private string _jwtToken = "";

    // ───────────────────────────────────────────────────────────
    void Awake()
    {
        // Singleton: solo existe una instancia y no se destruye al cambiar de escena
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ── Guardar el token después del login ─────────────────────
    public void SetToken(string token)
    {
        _jwtToken = token;
    }

    // ── Verificar si hay token activo ──────────────────────────
    public bool IsLoggedIn()
    {
        return !string.IsNullOrEmpty(_jwtToken);
    }

    // MÉTODO PRINCIPAL — POST
    // Envía un JSON a un endpoint y devuelve la respuesta
    public IEnumerator Post(string endpoint, object body,
        Action<string> onSuccess, Action<string> onError)
    {
        // Convertir el objeto C# a JSON
        string jsonBody = JsonUtility.ToJson(body);
        string url = BASE_URL + endpoint;

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Agregar token JWT si existe
        if (!string.IsNullOrEmpty(_jwtToken))
            request.SetRequestHeader("Authorization", "Bearer " + _jwtToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            onSuccess?.Invoke(request.downloadHandler.text);
        else
            onError?.Invoke(request.error + " | " + request.downloadHandler.text);

        request.Dispose();
    }

    // MÉTODO PRINCIPAL — GET
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

    // MÉTODO PRINCIPAL — PATCH
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