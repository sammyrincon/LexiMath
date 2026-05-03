using UnityEngine;
using TMPro;

public class NPCDialog : MonoBehaviour
{
    [Header("Caja de diálogo")]
    [Tooltip("GameObject que contiene la caja visual del diálogo")]
    public GameObject dialogBox;

    [Tooltip("TextMeshPro donde se escribe el texto")]
    public TMP_Text textoDialogo;

    [Header("Configuración")]
    [Tooltip("¿Ocultar al iniciar la escena?")]
    public bool ocultarAlInicio = false;

    void Start()
    {
        if (ocultarAlInicio)
            OcultarDialogo();
    }

    public void MostrarDialogo(string texto)
    {
        if (dialogBox != null)
            dialogBox.SetActive(true);

        if (textoDialogo != null)
            textoDialogo.text = texto;
    }

    public void OcultarDialogo()
    {
        if (dialogBox != null)
            dialogBox.SetActive(false);
    }
}
