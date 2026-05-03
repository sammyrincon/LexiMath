using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class LoginController : MonoBehaviour
{
    private UIDocument _doc;

    private TextField _inputUsuario;
    private TextField _inputContrasena;
    private Button    _botonLogin;
    private Button    _botonRegistrarse;
    private Button    _botonAdmin;
    private Label     _textoError;

    [SerializeField] private Texture2D _cursorMano;

    void OnEnable()
    {
        _doc = GetComponent<UIDocument>();
        var root = _doc.rootVisualElement;

        _inputUsuario     = root.Q<TextField>("input-usuario");
        _inputContrasena  = root.Q<TextField>("input-contrasena");
        _botonLogin       = root.Q<Button>("boton-login");
        _botonRegistrarse = root.Q<Button>("boton-registrarse");
        _botonAdmin       = root.Q<Button>("boton-admin");
        _textoError       = root.Q<Label>("texto-error");

        _textoError.style.display = DisplayStyle.None;

        _botonLogin.clicked       += OnClickLogin;
        _botonRegistrarse.clicked += OnClickRegistrarse;
        _botonAdmin.clicked       += OnClickAdmin;

        AgregarCursor(_botonLogin);
        AgregarCursor(_botonRegistrarse);
        AgregarCursor(_botonAdmin);
    }

    void OnDisable()
    {
        UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        _botonLogin.clicked       -= OnClickLogin;
        _botonRegistrarse.clicked -= OnClickRegistrarse;
        _botonAdmin.clicked       -= OnClickAdmin;
    }

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

    private void OnClickRegistrarse() =>
        SceneManager.LoadScene("RegistroScene");

    private void OnClickAdmin() =>
        Application.OpenURL("https://leximath.com.mx/admin/login.html");

    private void AgregarCursor(Button boton)
    {
        boton.RegisterCallback<MouseEnterEvent>(_ =>
            UnityEngine.Cursor.SetCursor(_cursorMano, Vector2.zero, CursorMode.Auto));
        boton.RegisterCallback<MouseLeaveEvent>(_ =>
            UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto));
    }

    private void MostrarError(string mensaje)
    {
        _textoError.text = mensaje;
        _textoError.style.display = DisplayStyle.Flex;
    }
}
