using UnityEngine;
using TMPro;

/// <summary>
/// NPCDialog — LexiMath
/// 
/// Caja de diálogo flotante sobre un NPC (MushroomEnemy quieto).
/// El QuestionManager la usa para mostrar las preguntas.
/// 
/// SETUP EN UNITY:
///   1. Selecciona el MushroomEnemy (o duplícalo y quítale EnemyBasic)
///   2. Desactiva el script EnemyBasic para que no patrulle ni ataque
///   3. Añade un hijo vacío llamado "DialogBox"
///   4. Dentro de DialogBox, añade un Canvas (World Space) con:
///        - Image (fondo blanco con borde)
///        - Hijo Text (TextMeshPro - Text (UI)) centrado
///   5. Añade este script al MushroomEnemy
///   6. Asigna las referencias en el Inspector
/// </summary>
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
