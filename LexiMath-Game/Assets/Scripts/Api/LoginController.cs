using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

/// <summary>
/// LoginController — controla la pantalla de Login con UI Toolkit
/// Lee los campos del UXML y conecta con AuthManager.
/// </summary>
public class LoginController : MonoBehaviour
{
    // ── Referencias al UIDocument ──────────────────────────────
    private UIDocument _doc;

    // ── Elementos del UXML ─────────────────────────────────────
    private TextField  _inputUsuario;
    private TextField  _inputContrasena;
    private Button     _botonLogin;
    private Button     _botonRegistrarse;
    private Button     _botonAdmin;
    private Label      _textoError;

    // ───────────────────────────────────────────────────────────
    void OnEnable()
    {
        // Obtener la raíz del documento UI
        _doc = GetComponent<UIDocument>();
        var root = _doc.rootVisualElement;

        // Conectar elementos por nombre (igual que en el UXML)
        _inputUsuario    = root.Q<TextField>("input-usuario");
        _inputContrasena = root.Q<TextField>("input-contrasena");
        _botonLogin      = root.Q<Button>("boton-login");
        _botonRegistrarse = root.Q<Button>("boton-registrarse");
        _botonAdmin      = root.Q<Button>("boton-admin");
        _textoError      = root.Q<Label>("texto-error");

        // Ocultar error al inicio
        _textoError.style.display = DisplayStyle.None;

        // Registrar eventos de los botones
        _botonLogin.clicked      += OnClickLogin;
        _botonRegistrarse.clicked += OnClickRegistrarse;
        _botonAdmin.clicked      += OnClickAdmin;
    }

    // ───────────────────────────────────────────────────────────
    void OnDisable()
    {
        // Desregistrar eventos para evitar memory leaks
        _botonLogin.clicked      -= OnClickLogin;
        _botonRegistrarse.clicked -= OnClickRegistrarse;
        _botonAdmin.clicked      -= OnClickAdmin;
    }

    // ───────────────────────────────────────────────────────────
    // BOTÓN LOGIN
    // ───────────────────────────────────────────────────────────
    private void OnClickLogin()
    {
        // Ocultar error anterior
        _textoError.style.display = DisplayStyle.None;

        // Validar campos vacíos
        if (string.IsNullOrEmpty(_inputUsuario.value) ||
            string.IsNullOrEmpty(_inputContrasena.value))
        {
            MostrarError("Por favor llena todos los campos");
            return;
        }

        // Llamar al AuthManager con las credenciales
        AuthManager.Instance.Login(
            _inputUsuario.value.Trim(),
            _inputContrasena.value,
            onSuccess: () =>
            {
                // Si nunca vio el tutorial → TutorialScene
                // Si ya lo vio → MapaScene
                if (!GameManager.Instance.TutorialMecanicas)
                    SceneManager.LoadScene("TutorialScene");
                else
                    SceneManager.LoadScene("MainMenu");
            },
            onError: (error) =>
            {
                MostrarError("Usuario o contraseña incorrectos");
                Debug.LogError("Login error: " + error);
            }
        );
    }

    // ───────────────────────────────────────────────────────────
    // BOTÓN REGISTRARSE
    // ───────────────────────────────────────────────────────────
    private void OnClickRegistrarse()
    {
        SceneManager.LoadScene("RegistroScene");
    }

    // ───────────────────────────────────────────────────────────
    // BOTÓN ADMIN
    // ───────────────────────────────────────────────────────────
    private void OnClickAdmin()
    {
        Application.OpenURL("https://leximath.app/admin");
    }

    // ───────────────────────────────────────────────────────────
    // Mostrar mensaje de error
    // ───────────────────────────────────────────────────────────
    private void MostrarError(string mensaje)
    {
        _textoError.text = mensaje;
        _textoError.style.display = DisplayStyle.Flex;
    }
}