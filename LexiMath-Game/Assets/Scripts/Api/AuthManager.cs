using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// AuthManager — maneja login y registro de estudiantes
/// Se comunica con ApiManager para llamar a la API REST.
/// </summary>
public class AuthManager : MonoBehaviour
{
    // ── Singleton ──────────────────────────────────────────────
    public static AuthManager Instance { get; private set; }

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

    // ───────────────────────────────────────────────────────────
    // LOGIN — POST /login
    // ───────────────────────────────────────────────────────────
    public void Login(string usuario, string contrasena,
        Action onSuccess, Action<string> onError)
    {
        var body = new LoginRequest
        {
            usuario    = usuario,
            contrasena = contrasena
        };

        StartCoroutine(ApiManager.Instance.Post("/login", body,
            (json) =>
            {
                // Parsear respuesta del servidor
                LoginResponse resp = JsonUtility.FromJson<LoginResponse>(json);

                // Guardar JWT en ApiManager
                ApiManager.Instance.SetToken(resp.token);

                // Guardar datos del jugador en GameManager
                GameManager.Instance.SetDatosJugador(
                    resp.estudiante.id_estudiante,
                    resp.estudiante.nombre,
                    resp.estudiante.puntos_total,
                    resp.inventario.monedas_disponibles,
                    resp.estudiante.tutorial_mecanicas,
                    resp.estudiante.tutorial_ui
                );

                onSuccess?.Invoke();
            },
            (error) => onError?.Invoke(error)
        ));
    }

    // ───────────────────────────────────────────────────────────
    // REGISTRO — 3 pasos, datos se acumulan localmente
    // ───────────────────────────────────────────────────────────
    private RegistroRequest _registroTemp = new RegistroRequest();

    // Paso 1 — nombre, edad, género
    public void GuardarPaso1(string nombre, int edad, string genero)
    {
        _registroTemp.nombre = nombre;
        _registroTemp.edad   = edad;
        _registroTemp.genero = genero;
    }

    // Paso 2 — usuario, correo tutor, contraseña
    public void GuardarPaso2(string usuario, string correo_tutor,
        string contrasena)
    {
        _registroTemp.usuario      = usuario;
        _registroTemp.correo_tutor = correo_tutor;
        _registroTemp.contrasena   = contrasena;
    }

    // Paso 3 — código NIDE y envío final a la API
    // POST /registro
    public void RegistrarPaso3(string codigo_nide,
        Action onSuccess, Action<string> onError)
    {
        _registroTemp.codigo_nide = codigo_nide;

        StartCoroutine(ApiManager.Instance.Post("/registro", _registroTemp,
            (json) => onSuccess?.Invoke(),
            (error) => onError?.Invoke(error)
        ));
    }
}

// ── Modelos de datos ───────────────────────────────────────────

[Serializable]
public class LoginRequest
{
    public string usuario;
    public string contrasena;
}

[Serializable]
public class RegistroRequest
{
    public string nombre;
    public int    edad;
    public string genero;
    public string usuario;
    public string correo_tutor;
    public string contrasena;
    public string codigo_nide;
}

[Serializable]
public class LoginResponse
{
    public string token;
    public EstudianteData estudiante;
    public InventarioData inventario;
}

[Serializable]
public class EstudianteData
{
    public int    id_estudiante;
    public string nombre;
    public int    puntos_total;
    public bool   tutorial_mecanicas;
    public bool   tutorial_ui;
}

[Serializable]
public class InventarioData
{
    public int monedas_disponibles;
}