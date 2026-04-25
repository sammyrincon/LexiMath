using UnityEngine;

public class PreguntaTriggerRM : MonoBehaviour
{
    // ── Referencias ────────────────────────────────────────────
    [Header("UI del panel de pregunta")]
    [SerializeField] private PreguntaUIRM _preguntaUI;

    // ── Estado ─────────────────────────────────────────────────
    private bool _yaActivado = false;   // Evita que se active dos veces

    // ──────────────────────────────────────────────────────────
     private void OnTriggerEnter2D(Collider2D other)
    {
        if (_yaActivado) return;
        if (!other.CompareTag("Player")) return;

        _yaActivado = true;

        // Obtener pregunta actual del NivelManager
        PreguntaDataRM pregunta = NivelManagerRM.Instance.ObtenerPreguntaActual();

        if (pregunta == null)
        {
            Debug.LogWarning("[PreguntaTriggerRM] No hay más preguntas.");
            Destroy(gameObject);
            return;
        }

        // Desactivar collider para que no vuelva a dispararse
        GetComponent<Collider2D>().enabled = false;

        // Mostrar cuadro de pregunta
        _preguntaUI.MostrarPregunta(pregunta, OnRespuesta);

        Debug.Log($"[PreguntaTriggerRM] Activado: {pregunta.enunciado}");
    }

    // ──────────────────────────────────────────────────────────
    /// <summary>
    /// Callback que recibe si la respuesta fue correcta o no.
    /// Lo llama PreguntaUIRM cuando el jugador salta a una plataforma.
    /// </summary>
    private void OnRespuesta(bool esCorrecta)
    {
        NivelManagerRM.Instance.RegistrarRespuesta(esCorrecta);
        Destroy(gameObject);
        Debug.Log($"[PreguntaTriggerRM] Respuesta registrada. Correcta: {esCorrecta}");
    }
}
