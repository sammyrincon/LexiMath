using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;
    
    public string backendURL = "https://tu-servidor.com/api/respuestas";

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegistrarRespuesta(string tema, bool esCorrecta)
    {
        StartCoroutine(EnviarDatosCoroutine(tema, esCorrecta));
    }

    private IEnumerator EnviarDatosCoroutine(string tema, bool esCorrecta)
    {
        WWWForm form = new WWWForm();
        form.AddField("tema", tema);
        form.AddField("esCorrecta", esCorrecta ? "1" : "0");
        form.AddField("timestamp", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        using (UnityWebRequest www = UnityWebRequest.Post(backendURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Dato registrado");
            }
        }
    }
}