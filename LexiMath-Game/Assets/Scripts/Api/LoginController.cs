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
    private TextField _inputUsuario;
    private TextField _inputContrasena;
    private Button    _botonLogin;
    private Button    _botonRegistrarse;
    private Button    _botonAdmin;
    private Label     _textoError;

    // ── Cursor personalizado ───────────────────────────────────
    [SerializeField] private Texture2D _cursorMano;

    // ───────────────────────────────────────────────────────────
    void OnEnable()
    {
        _doc = GetComponent<UIDocument>();
        var root = _doc.rootVisualElement;

        // Conectar elementos por nombre
        _inputUsuario     = root.Q<TextField>("input-usuario");
        _inputContrasena  = root.Q<TextField>("input-contrasena");
        _botonLogin       = root.Q<Button>("boton-login");
        _botonRegistrarse = root.Q<Button>("boton-registrarse");
        _botonAdmin       = root.Q<Button>("boton-admin");
        _textoError       = root.Q<Label>("texto-error");

        // Ocultar error al inicio
        _textoError.style.display = DisplayStyle.None;

        // Registrar eventos de botones
        _botonLogin.clicked       += OnClickLogin;
        _botonRegistrarse.clicked += OnClickRegistrarse;
        _botonAdmin.clicked       += OnClickAdmin;

        // Cursor personalizado
        AgregarCursor(_botonLogin);
        AgregarCursor(_botonRegistrarse);
        AgregarCursor(_botonAdmin);
    }

    // ───────────────────────────────────────────────────────────
    void OnDisable()
    {
        UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        _botonLogin.clicked       -= OnClickLogin;
        _botonRegistrarse.clicked -= OnClickRegistrarse;
        _botonAdmin.clicked       -= OnClickAdmin;
    }

    // ───────────────────────────────────────────────────────────
    // BOTÓN LOGIN
    // ───────────────────────────────────────────────────────────
    private void OnClickLogin()
    {
        _textoError.style.display = DisplayStyle.None;

        if (string.IsNullOrEmpty(_inputUsuario.value) ||
            string.IsNullOrEmpty(_inputContrasena.value))
        {
            MostrarError("Por favor llena todos los campos");
            return;
        }

        AuthManager.Instance.Login(
            _inputUsuario.value.Trim(),
            _inputContrasena.value,
            onSuccess: () =>
            {
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
    private void OnClickRegistrarse() =>
        SceneManager.LoadScene("RegistroScene");

    // ───────────────────────────────────────────────────────────
    // BOTÓN ADMIN
    // ───────────────────────────────────────────────────────────
    private void OnClickAdmin() =>
        Application.OpenURL("https://leximath.app/admin");

    // ───────────────────────────────────────────────────────────
    // Cursor personalizado en botones
    // ───────────────────────────────────────────────────────────
    private void AgregarCursor(Button boton)
    {
        boton.RegisterCallback<MouseEnterEvent>(_ =>
            UnityEngine.Cursor.SetCursor(_cursorMano, Vector2.zero, CursorMode.Auto));
        boton.RegisterCallback<MouseLeaveEvent>(_ =>
            UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto));
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