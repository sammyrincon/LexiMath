using UnityEngine;
using UnityEngine.SceneManagement;

public class NivelManagerRM : MonoBehaviour
{
    public static NivelManagerRM Instance { get; private set; }

    // ── Configuración ──────────────────────────────────────────
    [Header("Banco de preguntas")]
    [SerializeField] private BancoPreguntasRM _banco;

    [Header("Configuración del nivel")]
    [SerializeField] private int _vidasIniciales   = 3;
    [SerializeField] private int _ataquesIniciales = 0;

    // ── Estado del nivel ───────────────────────────────────────
    public int Puntos               { get; private set; }
    public int Vidas                { get; private set; }
    public int AtaquesEspada        { get; private set; }
    public int PreguntasRespondidas { get; private set; }
    public int Aciertos             { get; private set; }
    public int Errores              { get; private set; }

    // Preguntas cargadas para esta sesión
    public PreguntaDataRM[] Preguntas { get; private set; }

    // ── Constantes de puntuación ───────────────────────────────
    public const int PTS_MONEDA      = 1;
    public const int PTS_ENEMIGO     = 25;
    public const int PTS_PREGUNTA    = 35;
    public const int PTS_JEFE        = 50;
    public const int ATAQUES_PREMIO  = 5;
    public const int TOTAL_PREGUNTAS = 15;

    // ── Eventos para que el HUD reaccione ─────────────────────
    public System.Action OnPuntosActualizados;
    public System.Action OnVidasActualizadas;
    public System.Action OnAtaquesActualizados;
    public System.Action OnNivelCompletado;

    // ──────────────────────────────────────────────────────────
    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Inicializar estado
        Vidas         = _vidasIniciales;
        AtaquesEspada = _ataquesIniciales;
        Puntos        = 0;

        // Semilla aleatoria — garantiza preguntas distintas cada intento
        Random.InitState(System.DateTime.Now.Millisecond +
                         System.DateTime.Now.Second * 1000);

        // Cargar preguntas aleatorias del banco
        Preguntas = _banco.ObtenerPreguntasAleatorias(TOTAL_PREGUNTAS);
        Debug.Log($"[NivelManagerRM] {Preguntas.Length} preguntas cargadas para este intento.");
    }

    // ── Métodos públicos ───────────────────────────────────────

    /// <summary>Sumar punto por recoger moneda.</summary>
    public void SumarMoneda()
    {
        Puntos += PTS_MONEDA;
        OnPuntosActualizados?.Invoke();
    }

    /// <summary>Sumar puntos por matar enemigo.</summary>
    public void SumarEnemigo()
    {
        Puntos += PTS_ENEMIGO;
        OnPuntosActualizados?.Invoke();
    }

    /// <summary>Sumar puntos por derrotar al jefe final.</summary>
    public void SumarJefe()
    {
        Puntos += PTS_JEFE;
        OnPuntosActualizados?.Invoke();
    }

    /// <summary>
    /// Registra la respuesta del jugador.
    /// Correcto → +35 pts + 5 ataques.
    /// Incorrecto → solo registra el error.
    /// En ambos casos la pregunta desaparece.
    /// </summary>
    public void RegistrarRespuesta(bool esCorrecta)
    {
        PreguntasRespondidas++;

        if (esCorrecta)
        {
            Puntos        += PTS_PREGUNTA;
            AtaquesEspada += ATAQUES_PREMIO;
            Aciertos++;
            OnPuntosActualizados?.Invoke();
            OnAtaquesActualizados?.Invoke();
            Debug.Log($"[NivelManagerRM] ✅ Correcta! Ataques: {AtaquesEspada} Puntos: {Puntos}");
        }
        else
        {
            Errores++;
            Debug.Log($"[NivelManagerRM] ❌ Incorrecta. Errores: {Errores}");
        }

        // Verificar si completó todas las preguntas
        if (PreguntasRespondidas >= TOTAL_PREGUNTAS)
        {
            Debug.Log("[NivelManagerRM] ✅ 15 preguntas completadas.");
            OnNivelCompletado?.Invoke();
        }
    }

    /// <summary>
    /// Intenta usar un ataque de espada.
    /// Regresa true si tenía ataques disponibles.
    /// </summary>
    public bool UsarAtaque()
    {
        if (AtaquesEspada <= 0) return false;

        AtaquesEspada--;
        OnAtaquesActualizados?.Invoke();
        Debug.Log($"[NivelManagerRM] ⚔️ Ataque usado. Quedan: {AtaquesEspada}");
        return true;
    }

    /// <summary>
    /// Quita una vida al jugador.
    /// Si llega a 0 regresa al MainMenu.
    /// </summary>
    public void PerderVida()
    {
        Vidas--;
        OnVidasActualizadas?.Invoke();
        Debug.Log($"[NivelManagerRM] ❤️ Vida perdida. Vidas restantes: {Vidas}");

        if (Vidas <= 0)
        {
            Debug.Log("[NivelManagerRM] 💀 Game Over — regresando al MainMenu.");
            SceneManager.LoadScene("MainMenu");
        }
    }

    /// <summary>
    /// Regresa la pregunta actual según cuántas se han respondido.
    /// </summary>
    public PreguntaDataRM ObtenerPreguntaActual()
    {
        if (PreguntasRespondidas < Preguntas.Length)
            return Preguntas[PreguntasRespondidas];

        Debug.LogWarning("[NivelManagerRM] No hay más preguntas disponibles.");
        return null;
    }
}
