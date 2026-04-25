using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class PreguntaUIRM : MonoBehaviour
{
    // ── UI Toolkit ─────────────────────────────────────────────
    [Header("UI Document con el panel de pregunta")]
    [SerializeField] private UIDocument _uiDocument;

    // ── Cubitos físicos en el mundo ────────────────────────────
    [Header("Los 3 cubitos físicos (inactivos al inicio)")]
    [SerializeField] private PlataformaRespuestaRM _cubitoA;
    [SerializeField] private PlataformaRespuestaRM _cubitoB;
    [SerializeField] private PlataformaRespuestaRM _cubitoC;

    // ── Jugador ────────────────────────────────────────────────
    [Header("Transform del jugador")]
    [SerializeField] private Transform _jugador;

    [Header("Altura de los cubitos sobre el jugador")]
    [SerializeField] private float _alturaCubitos  = 2.5f;

    [Header("Separación horizontal entre cubitos")]
    [SerializeField] private float _separacion = 1.5f;

    // ── Elementos del UXML ─────────────────────────────────────
    private VisualElement _panelPregunta;
    private Label         _labelEnunciado;

    // ── Estado ─────────────────────────────────────────────────
    private System.Action<bool> _callbackRespuesta;
    private bool _respondido = false;

    // ──────────────────────────────────────────────────────────
    void OnEnable()
    {
        // Conectar elementos del UXML
        var root = _uiDocument.rootVisualElement;

        _panelPregunta  = root.Q<VisualElement>("panel-pregunta");
        _labelEnunciado = root.Q<Label>("label-enunciado");

        // Validar que existan en el UXML
        if (_panelPregunta == null)
            Debug.LogError("[PreguntaUIRM] No se encontró 'panel-pregunta' en el UXML.");
        if (_labelEnunciado == null)
            Debug.LogError("[PreguntaUIRM] No se encontró 'label-enunciado' en el UXML.");

        // Ocultar panel al inicio
        OcultarPanel();
    }

    // ──────────────────────────────────────────────────────────
    void Awake()
    {
        // Cubitos inactivos al inicio
        OcultarCubitos();
    }

    // ──────────────────────────────────────────────────────────
    /// <summary>
    /// Muestra el panel UI con el enunciado y posiciona
    /// los 3 cubitos frente al jugador en el mundo.
    /// Llamado por PreguntaTriggerRM.
    /// </summary>
    public void MostrarPregunta(PreguntaDataRM pregunta, System.Action<bool> callback)
    {
        _callbackRespuesta = callback;
        _respondido        = false;

        // Mostrar panel con enunciado
        MostrarPanel(pregunta.enunciado);

        // Mezclar opciones aleatoriamente
        int[]  valores   = { pregunta.opcionA, pregunta.opcionB, pregunta.opcionC };
        bool[] correctas = { pregunta.indiceCorrecta == 0,
                             pregunta.indiceCorrecta == 1,
                             pregunta.indiceCorrecta == 2 };
        MezclarOpciones(valores, correctas);

        // Posicionar cubitos frente al jugador
        PosicionarCubitos();

        // Activar e inicializar cubitos
        ActivarCubito(_cubitoA, valores[0], correctas[0]);
        ActivarCubito(_cubitoB, valores[1], correctas[1]);
        ActivarCubito(_cubitoC, valores[2], correctas[2]);

        Debug.Log($"[PreguntaUIRM] Pregunta activa: {pregunta.enunciado}");
    }

    // ──────────────────────────────────────────────────────────
    /// <summary>
    /// Muestra el panel UI con el enunciado.
    /// </summary>
    private void MostrarPanel(string enunciado)
    {
        if (_panelPregunta == null || _labelEnunciado == null) return;

        _labelEnunciado.text              = enunciado;
        _panelPregunta.style.display      = DisplayStyle.Flex;
        _panelPregunta.style.visibility   = Visibility.Visible;
    }

    // ──────────────────────────────────────────────────────────
    /// <summary>
    /// Oculta el panel UI.
    /// </summary>
    private void OcultarPanel()
    {
        if (_panelPregunta == null) return;

        _panelPregunta.style.display    = DisplayStyle.None;
        _panelPregunta.style.visibility = Visibility.Hidden;
    }

    // ──────────────────────────────────────────────────────────
    /// <summary>
    /// Posiciona los 3 cubitos en el mundo frente al jugador.
    /// B al centro, A a la izquierda, C a la derecha.
    /// </summary>
    private void PosicionarCubitos()
    {
        if (_jugador == null) return;

        Vector3 centro = new Vector3(
            _jugador.position.x,
            _jugador.position.y + _alturaCubitos,
            0f
        );

        if (_cubitoA != null)
            _cubitoA.transform.position = centro + new Vector3(-_separacion, 0f, 0f);

        if (_cubitoB != null)
            _cubitoB.transform.position = centro;

        if (_cubitoC != null)
            _cubitoC.transform.position = centro + new Vector3(_separacion, 0f, 0f);
    }

    // ──────────────────────────────────────────────────────────
    /// <summary>
    /// Activa un cubito y lo inicializa con su valor.
    /// </summary>
    private void ActivarCubito(PlataformaRespuestaRM cubito, int valor, bool esCorrecta)
    {
        if (cubito == null)
        {
            Debug.LogError("[PreguntaUIRM] Cubito no asignado en el Inspector.");
            return;
        }

        cubito.gameObject.SetActive(true);
        cubito.Inicializar(valor, esCorrecta, OnJugadorEligio);
    }

    // ──────────────────────────────────────────────────────────
    /// <summary>
    /// El jugador golpeó un cubito.
    /// Cierra TODO sin importar si acertó o no.
    /// </summary>
    private void OnJugadorEligio(bool esCorrecta)
    {
        // Evitar doble llamada si golpea 2 cubitos casi al mismo tiempo
        if (_respondido) return;
        _respondido = true;

        // Ocultar panel UI
        OcultarPanel();

        // Ocultar los 3 cubitos siempre
        OcultarCubitos();

        // Notificar al trigger
        _callbackRespuesta?.Invoke(esCorrecta);

        Debug.Log($"[PreguntaUIRM] Cerrado. Correcta: {esCorrecta}");
    }

    // ──────────────────────────────────────────────────────────
    /// <summary>
    /// Desactiva los 3 cubitos en el mundo.
    /// </summary>
    private void OcultarCubitos()
    {
        if (_cubitoA != null) _cubitoA.gameObject.SetActive(false);
        if (_cubitoB != null) _cubitoB.gameObject.SetActive(false);
        if (_cubitoC != null) _cubitoC.gameObject.SetActive(false);
    }

    // ──────────────────────────────────────────────────────────
    /// <summary>
    /// Fisher-Yates — mezcla opciones para que la respuesta
    /// correcta no siempre esté en el mismo cubito.
    /// </summary>
    private void MezclarOpciones(int[] valores, bool[] correctas)
    {
        for (int i = valores.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            int  tempVal  = valores[i];
            valores[i]    = valores[j];
            valores[j]    = tempVal;

            bool tempBool = correctas[i];
            correctas[i]  = correctas[j];
            correctas[j]  = tempBool;
        }
    }
}

