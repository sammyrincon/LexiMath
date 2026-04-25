using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class BienvenidaController : MonoBehaviour
{
    private UIDocument _doc;
    private Label      _textoBienvenida;
    private Label      _textoOrg;
    private Button     _botonEmpezar;
    private Button     _botonCerrarSesion;

    [SerializeField] private Texture2D _cursorMano;

    void OnEnable()
    {
        _doc = GetComponent<UIDocument>();
        var root = _doc.rootVisualElement;

        _textoBienvenida   = root.Q<Label>("texto-bienvenida");
        _textoOrg          = root.Q<Label>("texto-org");
        _botonEmpezar      = root.Q<Button>("boton-empezar");
        _botonCerrarSesion = root.Q<Button>("boton-cerrar-sesion");

        // Mostrar usuario del jugador
        _textoBienvenida.text = $"¡BIENVENIDO, {GameManager.Instance.NombreEstudiante}!";

        // Botones
        _botonEmpezar.clicked      += OnClickEmpezar;
        _botonCerrarSesion.clicked += OnClickCerrarSesion;

        AgregarCursor(_botonEmpezar);
        AgregarCursor(_botonCerrarSesion);
    }

    void OnDisable()
    {
        UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        _botonEmpezar.clicked      -= OnClickEmpezar;
        _botonCerrarSesion.clicked -= OnClickCerrarSesion;
    }

    private void OnClickEmpezar() =>
        // SceneManager.LoadScene("TutorialScene");
        SceneManager.LoadScene("MainMenu");

    private void OnClickCerrarSesion()
    {
        // Limpiar token y datos
        ApiManager.Instance.SetToken("");
        SceneManager.LoadScene("LoginScene");
    }

    private void AgregarCursor(Button boton)
    {
        boton.RegisterCallback<MouseEnterEvent>(_ =>
            UnityEngine.Cursor.SetCursor(_cursorMano, Vector2.zero, CursorMode.Auto));
        boton.RegisterCallback<MouseLeaveEvent>(_ =>
            UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto));
    }
}