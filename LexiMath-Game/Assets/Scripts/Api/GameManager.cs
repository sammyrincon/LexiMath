using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [HideInInspector] public int    IdEstudiante;
    [HideInInspector] public string NombreEstudiante;
    [HideInInspector] public int    PuntosTotal;
    [HideInInspector] public int    MonedasDisponibles;
    [HideInInspector] public bool   TutorialMecanicas;
    [HideInInspector] public bool   TutorialUI;

    [HideInInspector] public int    IdSesionActiva;
    [HideInInspector] public int    IdNivelActivo;

    [HideInInspector] public int    PuntosSesion;
    [HideInInspector] public int    MonedasSesion;
    [HideInInspector] public int    EnemigosEliminados;
    [HideInInspector] public int    RespuestasCorrectas;
    [HideInInspector] public int    AtaquesEspada;

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

    public void SetTutorialCompletado()
    {
        TutorialMecanicas = true;
        Debug.Log("[GameManager] Tutorial completado — TutorialMecanicas = true");
    }
}
