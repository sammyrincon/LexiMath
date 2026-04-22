using UnityEngine;

/// <summary>
/// GameManager — datos globales del jugador en LEXIMATH
/// Singleton que persiste entre escenas y guarda el estado del jugador después del login.
/// </summary>
public class GameManager : MonoBehaviour
{
    // ── Singleton ──────────────────────────────────────────────
    public static GameManager Instance { get; private set; }

    // ── Datos del jugador (se llenan al hacer login) ───────────
    [HideInInspector] public int    IdEstudiante;
    [HideInInspector] public string NombreEstudiante;
    [HideInInspector] public int    PuntosTotal;
    [HideInInspector] public int    MonedasDisponibles;
    [HideInInspector] public bool   TutorialMecanicas;
    [HideInInspector] public bool   TutorialUI;

    // ── Datos de la sesión activa ──────────────────────────────
    [HideInInspector] public int    IdSesionActiva;
    [HideInInspector] public int    IdNivelActivo;

    // ── Puntos ganados en la sesión actual ─────────────────────
    [HideInInspector] public int    PuntosSesion;
    [HideInInspector] public int    MonedasSesion;
    [HideInInspector] public int    EnemigosEliminados;
    [HideInInspector] public int    RespuestasCorrectas;
    [HideInInspector] public int    AtaquesEspada;
    public int puntosPara2Estrellas = 500;
    public int puntosPara3Estrellas = 1000;

    // ───────────────────────────────────────────────────────────
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

    // ── Llenar datos al recibir respuesta del login ────────────
    public void SetDatosJugador(int id, string nombre, int puntos,
        int monedas, bool tutMecanicas, bool tutUI)
    {
        IdEstudiante       = id;
        NombreEstudiante   = nombre;
        PuntosTotal        = puntos;
        MonedasDisponibles = monedas;
        TutorialMecanicas  = tutMecanicas;
        TutorialUI         = tutUI;
    }

    // ── Reiniciar contadores al iniciar una sesión nueva ───────
    public void IniciarSesion(int idSesion, int idNivel)
    {
        IdSesionActiva     = idSesion;
        IdNivelActivo      = idNivel;
        PuntosSesion       = 0;
        MonedasSesion      = 0;
        EnemigosEliminados = 0;
        RespuestasCorrectas = 0;
        AtaquesEspada      = 0;
    }

    // ── Sumar puntos durante el juego ──────────────────────────
    public void SumarMoneda()
    {
        // 1. Lógica interna: sumo el valor a mi billetera.
        MonedasSesion++;
        MonedasDisponibles++;
        
        // 2. Delegación visual: Le aviso al UIManager (el encargado de la pantalla) 
        // que el número cambió para que actualice el texto. Yo no toco la interfaz desde aquí.
        UIManager.instance.ActualizarMonedas(MonedasSesion);
        PuntosSesion++;
        UIManager.instance.ActualizarPuntos(PuntosSesion);
    }

    public void SumarEnemigo()
    {
        EnemigosEliminados++;
        PuntosSesion += 25;
        UIManager.instance.ActualizarPuntos(PuntosSesion);
    }

    public void SumarRespuestaCorrecta()
    {
        RespuestasCorrectas++;
        PuntosSesion += 35;
        AtaquesEspada += 5;
        UIManager.instance.ActualizarPuntos(PuntosSesion);
    }
    public void FinalizarNivel()
    {
        // 1. CONGELAR EL JUEGO
        // En este bloque de código lo que se hace es detener el tiempo de Unity. 
        // Al poner el timeScale en 0, las físicas, enemigos y animaciones se pausan 
        // para que el jugador pueda ver la pantalla de victoria con calma.
        Time.timeScale = 0f;

        // 2. PERSISTENCIA DE DATOS (EL BANCO)
        // En este bloque utilizo PlayerPrefs, que es como un pequeño disco duro de Unity.
        // Primero, consulto cuántas monedas tenía guardadas de niveles anteriores.
        // (El 0 es por si es la primera vez que juega y no tiene fondos).
        int monedasGuardadas = PlayerPrefs.GetInt("MonedasTotales", 0);
        
        // Luego, sumo lo que ya tenía en el banco con lo que acaba de ganar en este nivel
        // y lo vuelvo a guardar de forma permanente para que las use en la tienda.
        PlayerPrefs.SetInt("MonedasTotales", monedasGuardadas + MonedasSesion);
        PlayerPrefs.Save();

        // 3. CÁLCULO DE ESTRELLAS
        // Aquí evalúo el rendimiento del jugador. Le doy 1 estrella de base solo por sobrevivir y llegar al portal.
        int estrellasGanadas = 1; 
        
        // Uso condicionales de mayor a menor para asignar la recompensa justa según los umbrales.
        if (PuntosSesion >= puntosPara3Estrellas) 
        {
            estrellasGanadas = 3;
        }
        else if (PuntosSesion >= puntosPara2Estrellas) 
        {
            estrellasGanadas = 2;
        }

        // 4. EJECUCIÓN VISUAL FINAL
        // Finalmente, ya que hice todos los cálculos matemáticos y de guardado,
        // le paso el resultado final al UIManager para que haga su magia y dibuje la pantalla.
        UIManager.instance.MostrarVictoria(PuntosSesion, estrellasGanadas);
    }
}