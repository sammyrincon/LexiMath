using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private UIDocument _doc;

    // ── Header ─────────────────────────────────────────────────
    private Button _botonMenu;
    private Label  _textoPuntos;

    // ── Contenido ──────────────────────────────────────────────
    private Label _textoBienvenida;

    // ── Menú lateral ───────────────────────────────────────────
    private VisualElement _overlay;
    private VisualElement _menuLateral;
    private Button        _botonCerrarMenu;
    private VisualElement _listaMates;
    private VisualElement _listaEspanol;
    private Button        _btnSeccionMates;
    private Button        _btnSeccionEspanol;

    // ── Niveles Matemáticas ────────────────────────────────────
    private Button _nivelMates1, _nivelMates2, _nivelMates3,
                   _nivelMates4, _nivelMates5;

    // ── Niveles Español ────────────────────────────────────────
    private Button _nivelEspanol1, _nivelEspanol2, _nivelEspanol3,
                   _nivelEspanol4, _nivelEspanol5;

    // ── Footer menú ────────────────────────────────────────────
    private Button _botonPerfil;
    private Button _botonConfig;
    private Button _botonSalir;

    [SerializeField] private Texture2D _cursorMano;

    // ── Estado del menú ────────────────────────────────────────
    private bool _matesExpandido   = false;
    private bool _espanolExpandido = false;

    // ───────────────────────────────────────────────────────────
    void OnEnable()
    {
        _doc = GetComponent<UIDocument>();
        var root = _doc.rootVisualElement;

        // Header
        _botonMenu   = root.Q<Button>("boton-menu");
        _textoPuntos = root.Q<Label>("texto-puntos");

        // Bienvenida
        _textoBienvenida = root.Q<Label>("texto-bienvenida");

        // Menú lateral
        _overlay           = root.Q<VisualElement>("overlay");
        _menuLateral       = root.Q<VisualElement>("menu-lateral");
        _botonCerrarMenu   = root.Q<Button>("boton-cerrar-menu");
        _listaMates        = root.Q<VisualElement>("lista-mates");
        _listaEspanol      = root.Q<VisualElement>("lista-espanol");
        _btnSeccionMates   = root.Q<Button>("btn-seccion-mates");
        _btnSeccionEspanol = root.Q<Button>("btn-seccion-espanol");

        // Niveles Matemáticas
        _nivelMates1 = root.Q<Button>("nivel-mates-1");
        _nivelMates2 = root.Q<Button>("nivel-mates-2");
        _nivelMates3 = root.Q<Button>("nivel-mates-3");
        _nivelMates4 = root.Q<Button>("nivel-mates-4");
        _nivelMates5 = root.Q<Button>("nivel-mates-5");

        // Niveles Español
        _nivelEspanol1 = root.Q<Button>("nivel-espanol-1");
        _nivelEspanol2 = root.Q<Button>("nivel-espanol-2");
        _nivelEspanol3 = root.Q<Button>("nivel-espanol-3");
        _nivelEspanol4 = root.Q<Button>("nivel-espanol-4");
        _nivelEspanol5 = root.Q<Button>("nivel-espanol-5");

        // Footer
        _botonPerfil = root.Q<Button>("boton-perfil");
        _botonConfig = root.Q<Button>("boton-config");
        _botonSalir  = root.Q<Button>("boton-salir");

        // ── Llenar datos del jugador ───────────────────────────
        if (GameManager.Instance != null)
        {
            _textoBienvenida.text = $"¡Bienvenido, {GameManager.Instance.NombreEstudiante}!";
            _textoPuntos.text     = GameManager.Instance.PuntosTotal.ToString();
        }

        // ── Estado inicial menú ────────────────────────────────
        _listaMates.style.display   = DisplayStyle.None;
        _listaEspanol.style.display = DisplayStyle.None;
        _btnSeccionMates.text       = "▶  MATEMÁTICAS";
        _btnSeccionEspanol.text     = "▶  ESPAÑOL";

        // ── Eventos ───────────────────────────────────────────
        _botonMenu.clicked         += AbrirMenu;
        _botonCerrarMenu.clicked   += CerrarMenu;
        _overlay.RegisterCallback<ClickEvent>(_ => CerrarMenu());

        _btnSeccionMates.clicked   += ToggleMates;
        _btnSeccionEspanol.clicked += ToggleEspanol;

        _botonSalir.clicked  += OnClickSalir;
        _botonPerfil.clicked += OnClickPerfil;
        _botonConfig.clicked += OnClickConfig;

        // ── Cursores ──────────────────────────────────────────
        AgregarCursor(_botonMenu);
        AgregarCursor(_botonCerrarMenu);
        AgregarCursor(_btnSeccionMates);
        AgregarCursor(_btnSeccionEspanol);
        AgregarCursor(_botonPerfil);
        AgregarCursor(_botonConfig);
        AgregarCursor(_botonSalir);
    }

    // ───────────────────────────────────────────────────────────
    void OnDisable()
    {
        UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        _botonMenu.clicked         -= AbrirMenu;
        _botonCerrarMenu.clicked   -= CerrarMenu;
        _btnSeccionMates.clicked   -= ToggleMates;
        _btnSeccionEspanol.clicked -= ToggleEspanol;
        _botonSalir.clicked        -= OnClickSalir;
        _botonPerfil.clicked       -= OnClickPerfil;
        _botonConfig.clicked       -= OnClickConfig;
    }

    // ── Abrir / Cerrar menú lateral ───────────────────────────
    private void AbrirMenu()
    {
        _overlay.style.display     = DisplayStyle.Flex;
        _menuLateral.style.display = DisplayStyle.Flex;
    }

    private void CerrarMenu()
    {
        _overlay.style.display     = DisplayStyle.None;
        _menuLateral.style.display = DisplayStyle.None;
    }

    // ── Toggle secciones ──────────────────────────────────────
    private void ToggleMates()
    {
        _matesExpandido = !_matesExpandido;
        _listaMates.style.display = _matesExpandido
            ? DisplayStyle.Flex : DisplayStyle.None;
        _btnSeccionMates.text = (_matesExpandido ? "▼" : "▶") + "  MATEMÁTICAS";
    }

    private void ToggleEspanol()
    {
        _espanolExpandido = !_espanolExpandido;
        _listaEspanol.style.display = _espanolExpandido
            ? DisplayStyle.Flex : DisplayStyle.None;
        _btnSeccionEspanol.text = (_espanolExpandido ? "▼" : "▶") + "  ESPAÑOL";
    }

    // ── Footer ────────────────────────────────────────────────
    private void OnClickSalir()
    {
        ApiManager.Instance.SetToken("");
        SceneManager.LoadScene("LoginScene");
    }

    private void OnClickPerfil()
    {
        Debug.Log("Perfil — próximamente");
    }

    private void OnClickConfig()
    {
        Debug.Log("Config — próximamente");
    }

    // ── Cursor ────────────────────────────────────────────────
    private void AgregarCursor(Button boton)
    {
        boton.RegisterCallback<MouseEnterEvent>(_ =>
            UnityEngine.Cursor.SetCursor(_cursorMano, Vector2.zero, CursorMode.Auto));
        boton.RegisterCallback<MouseLeaveEvent>(_ =>
            UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto));
    }
}