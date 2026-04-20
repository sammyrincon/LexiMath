using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// MainMenuController — LexiMath
/// 
/// Controla el menú principal.
/// Los niveles de cada mundo están hardcodeados (no visibles en el Inspector)
/// para evitar que valores viejos del Inspector sobrescriban la configuración.
/// 
/// SETUP EN UNITY:
///   1. GameObject "MainMenu" con UIDocument + este script
///   2. Asigna MainMenu.uxml como Source Asset del UIDocument
///   3. (Opcional) Ajusta nombres de escenas en el Inspector si fuera necesario
/// </summary>
public class MainMenuController : MonoBehaviour
{
    // ══════════════════════════════════════════════════════════
    //  CONFIGURACIÓN EDITABLE DESDE EL INSPECTOR
    //  (solo los nombres de escenas principales, por si cambian)
    // ══════════════════════════════════════════════════════════

    [Header("Escenas principales")]
    [SerializeField] private string escenaAccesoMates   = "Nivel 1 Mate - BB";
    [SerializeField] private string escenaAccesoEspanol = "Nivel 1 Esp - BB";
    [SerializeField] private string escenaTutorial      = "TutorialScene";

    // ══════════════════════════════════════════════════════════
    //  NIVELES HARDCODEADOS (no editables desde Inspector)
    // ══════════════════════════════════════════════════════════

    private List<NivelData> NivelesMatematicas() => new List<NivelData>
    {
        new NivelData("Sumas",          "Nivel 1 Mate - BB", true ),
        new NivelData("Restas",         "Mates_Nivel2",      false),
        new NivelData("Multiplicación", "Mates_Nivel3",      false),
        new NivelData("División",       "Mates_Nivel4",      false),
        new NivelData("Jefe Final",     "Mates_Jefe",        false),
    };

    private List<NivelData> NivelesEspanol() => new List<NivelData>
    {
        new NivelData("Vocales",    "Nivel 1 Esp - BB", true ),
        new NivelData("Palabras",   "Lecto_Nivel2",     false),
        new NivelData("Oraciones",  "Lecto_Nivel3",     false),
        new NivelData("Gramática",  "Lecto_Nivel4",     false),
        new NivelData("Jefe Final", "Lecto_Jefe",       false),
    };

    // ══════════════════════════════════════════════════════════
    //  REFERENCIAS UI (privadas)
    // ══════════════════════════════════════════════════════════
    private VisualElement _menuLateral;
    private VisualElement _nivelesMatContainer;
    private VisualElement _nivelesEspContainer;
    private Label         _flechaMates;
    private Label         _flechaEspanol;

    private bool _matesExpandido   = true;
    private bool _espanolExpandido = false;

    // ══════════════════════════════════════════════════════════
    //  CICLO DE VIDA
    // ══════════════════════════════════════════════════════════
    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // ── Referencias ───────────────────────────────────────
        _menuLateral         = root.Q<VisualElement>("menu-lateral");
        _nivelesMatContainer = root.Q<VisualElement>("niveles-mates");
        _nivelesEspContainer = root.Q<VisualElement>("niveles-espanol");
        _flechaMates         = root.Q<Label>("flecha-mates");
        _flechaEspanol       = root.Q<Label>("flecha-espanol");

        // ── Botón hamburguesa ─────────────────────────────────
        root.Q<Button>("btn-hamburguesa").clicked += AbrirMenu;

        // ── Cards de acceso rápido (circulitos 1 y 2) ────────
        var btnCardMates   = root.Q<Button>("btn-card-mates");
        var btnCardEspanol = root.Q<Button>("btn-card-espanol");
        if (btnCardMates   != null) btnCardMates.clicked   += () => CargarEscena(escenaAccesoMates);
        if (btnCardEspanol != null) btnCardEspanol.clicked += () => CargarEscena(escenaAccesoEspanol);

        // ── Menú lateral ──────────────────────────────────────
        root.Q<Button>("btn-cerrar-menu").clicked    += CerrarMenu;
        root.Q<Button>("btn-tutorial").clicked       += () => CargarEscena(escenaTutorial);
        root.Q<Button>("btn-mates-header").clicked   += ToggleMates;
        root.Q<Button>("btn-espanol-header").clicked += ToggleEspanol;

        // ── Footer ────────────────────────────────────────────
        var btnPerfil = root.Q<Button>("btn-perfil");
        var btnConfig = root.Q<Button>("btn-config");
        var btnSalir  = root.Q<Button>("btn-salir");
        if (btnPerfil != null) btnPerfil.clicked += () => Debug.Log("[MainMenu] Perfil (pendiente)");
        if (btnConfig != null) btnConfig.clicked += () => Debug.Log("[MainMenu] Config (pendiente)");
        if (btnSalir  != null) btnSalir.clicked  += SalirJuego;

        // ── Generar listas de niveles (hardcodeadas) ──────────
        GenerarNiveles(_nivelesMatContainer, NivelesMatematicas());
        GenerarNiveles(_nivelesEspContainer, NivelesEspanol());

        // ── Estado inicial de acordeones ──────────────────────
        _nivelesMatContainer.style.display = DisplayStyle.Flex;
        _nivelesEspContainer.style.display = DisplayStyle.None;
    }

    // ══════════════════════════════════════════════════════════
    //  MENÚ HAMBURGUESA
    // ══════════════════════════════════════════════════════════
    void AbrirMenu()  => _menuLateral.style.display = DisplayStyle.Flex;
    void CerrarMenu() => _menuLateral.style.display = DisplayStyle.None;

    // ══════════════════════════════════════════════════════════
    //  ACORDEONES
    // ══════════════════════════════════════════════════════════
    void ToggleMates()
    {
        _matesExpandido = !_matesExpandido;
        _nivelesMatContainer.style.display = _matesExpandido ? DisplayStyle.Flex : DisplayStyle.None;
        _flechaMates.text = _matesExpandido ? "∧" : "∨";
    }

    void ToggleEspanol()
    {
        _espanolExpandido = !_espanolExpandido;
        _nivelesEspContainer.style.display = _espanolExpandido ? DisplayStyle.Flex : DisplayStyle.None;
        _flechaEspanol.text = _espanolExpandido ? "∧" : "∨";
    }

    // ══════════════════════════════════════════════════════════
    //  GENERAR BOTONES DE NIVELES (con candado para bloqueados)
    // ══════════════════════════════════════════════════════════
    void GenerarNiveles(VisualElement contenedor, List<NivelData> niveles)
    {
        contenedor.Clear();

        for (int i = 0; i < niveles.Count; i++)
        {
            var nivel = niveles[i];
            var btn = new Button();
            btn.AddToClassList("btn-nivel-item");

            // Badge numérico
            var badge = new VisualElement();
            badge.AddToClassList("nivel-numero-badge");
            if (!nivel.desbloqueado) badge.AddToClassList("nivel-numero-badge-bloqueado");

            var badgeTexto = new Label((i + 1).ToString());
            badgeTexto.AddToClassList("nivel-badge-texto");
            badge.Add(badgeTexto);

            // Nombre del nivel
            var nombre = new Label(nivel.nombre);
            nombre.AddToClassList("nivel-nombre-texto");
            if (!nivel.desbloqueado) nombre.AddToClassList("nivel-nombre-bloqueado");

            btn.Add(badge);
            btn.Add(nombre);

            // Candado para niveles bloqueados
            if (!nivel.desbloqueado)
            {
                var candado = new Label("[X]");
                candado.AddToClassList("nivel-candado");
                btn.Add(candado);
            }

            // Solo los desbloqueados navegan
            if (nivel.desbloqueado)
            {
                string escena = nivel.nombreEscena;
                btn.clicked += () => CargarEscena(escena);
            }

            contenedor.Add(btn);
        }
    }

    // ══════════════════════════════════════════════════════════
    //  NAVEGACIÓN Y SALIDA
    // ══════════════════════════════════════════════════════════
    void CargarEscena(string nombreEscena)
    {
        if (string.IsNullOrEmpty(nombreEscena))
        {
            Debug.LogWarning("[MainMenu] Nombre de escena vacío");
            return;
        }
        Debug.Log($"[MainMenu] Cargando escena: {nombreEscena}");
        SceneManager.LoadScene(nombreEscena);
    }

    void SalirJuego()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

// ══════════════════════════════════════════════════════════════
//  ESTRUCTURA DE DATOS (sin [System.Serializable] para que no
//  aparezca en Inspector si se usa con listas públicas)
// ══════════════════════════════════════════════════════════════
public class NivelData
{
    public string nombre;
    public string nombreEscena;
    public bool   desbloqueado;

    public NivelData(string nombre, string nombreEscena, bool desbloqueado)
    {
        this.nombre       = nombre;
        this.nombreEscena = nombreEscena;
        this.desbloqueado = desbloqueado;
    }
}
