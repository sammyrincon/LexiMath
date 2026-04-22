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
        MonedasSesion++;
        MonedasDisponibles++;
        PuntosSesion++;
    }

    public void SumarEnemigo()
    {
        EnemigosEliminados++;
        PuntosSesion += 25;
    }

    public void SumarRespuestaCorrecta()
    {
        RespuestasCorrectas++;
        PuntosSesion += 35;
        AtaquesEspada += 5;
    }
}