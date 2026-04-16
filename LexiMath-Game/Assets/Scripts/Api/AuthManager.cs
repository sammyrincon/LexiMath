using System;
using System.Collections;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }

    private RegistroRequest _registroTemp;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _registroTemp = new RegistroRequest();
    }

    // ── LOGIN ──────────────────────────────────────────────────
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
                LoginResponse resp = JsonUtility.FromJson<LoginResponse>(json);
                ApiManager.Instance.SetToken(resp.token);
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

    // ── REGISTRO ───────────────────────────────────────────────
    public void GuardarPaso1(string nombre, int edad, string genero)
    {
        _registroTemp.nombre = nombre;
        _registroTemp.edad   = edad;
        _registroTemp.genero = genero;
    }

    public void GuardarPaso2(string usuario, string correo_tutor,
        string contrasena)
    {
        _registroTemp.usuario      = usuario;
        _registroTemp.correo_tutor = correo_tutor;
        _registroTemp.contrasena   = contrasena;
    }

    public void RegistrarPaso3(string codigo_nide,
        Action onSuccess, Action<string> onError)
    {
        _registroTemp.codigo_acceso = codigo_nide;

        StartCoroutine(ApiManager.Instance.Post("/registro", _registroTemp,
            (json) =>
            {
                // Guardar usuario en GameManager para pantalla de bienvenida
                GameManager.Instance.NombreEstudiante = _registroTemp.usuario;
                onSuccess?.Invoke();
            },
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
    public string codigo_acceso;
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