using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MainMenuController : MonoBehaviour
{
    // ── Configura desde el Inspector ─────────────────────────────
    [Header("Escenas de Matemáticas")]
    public List<NivelData> nivelesMatematicas = new List<NivelData>
    {
        new NivelData { nombre = "Tutorial", nombreEscena = "TutorialScene", desbloqueado = true },
        new NivelData { nombre = "Sumas",    nombreEscena = "Mates_Nivel1",  desbloqueado = false },
        new NivelData { nombre = "Restas",   nombreEscena = "Mates_Nivel2",  desbloqueado = false },
    };

    [Header("Escenas de Español")]
    public List<NivelData> nivelesEspanol = new List<NivelData>
    {
        new NivelData { nombre = "Vocales",  nombreEscena = "Espanol_Nivel1", desbloqueado = false },
        new NivelData { nombre = "Palabras", nombreEscena = "Espanol_Nivel2", desbloqueado = false },
    };

    // ── Referencias UI ────────────────────────────────────────────
    private VisualElement menuLateral;
    private VisualElement nivelesMatContainer;
    private VisualElement nivelesEspContainer;
    private Label flechaMates;
    private Label flechaEspanol;
    private bool matesExpandido   = true;
    private bool espanolExpandido = false;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Botón hamburguesa
        root.Q<Button>("btn-hamburguesa").clicked += AbrirMenu;

        // Menú lateral
        menuLateral         = root.Q<VisualElement>("menu-lateral");
        nivelesMatContainer = root.Q<VisualElement>("niveles-mates");
        nivelesEspContainer = root.Q<VisualElement>("niveles-espanol");
        flechaMates         = root.Q<Label>("flecha-mates");
        flechaEspanol       = root.Q<Label>("flecha-espanol");

        root.Q<Button>("btn-cerrar-menu").clicked  += CerrarMenu;
        root.Q<Button>("btn-mates-header").clicked += ToggleMates;
        root.Q<Button>("btn-espanol-header").clicked += ToggleEspanol;
        root.Q<Button>("btn-salir").clicked        += () => Application.Quit();

        // Generar listas de niveles
        GenerarNiveles(nivelesMatContainer, nivelesMatematicas);
        GenerarNiveles(nivelesEspContainer, nivelesEspanol);

        // Estado inicial
        nivelesMatContainer.style.display = DisplayStyle.Flex;
        nivelesEspContainer.style.display = DisplayStyle.None;
    }

    // ── Menú hamburguesa ─────────────────────────────────────────

    void AbrirMenu()  => menuLateral.style.display = DisplayStyle.Flex;
    void CerrarMenu() => menuLateral.style.display = DisplayStyle.None;

    // ── Acordeones ───────────────────────────────────────────────

    void ToggleMates()
    {
        matesExpandido = !matesExpandido;
        nivelesMatContainer.style.display = matesExpandido ? DisplayStyle.Flex : DisplayStyle.None;
        flechaMates.text = matesExpandido ? "∧" : "∨";
    }

    void ToggleEspanol()
    {
        espanolExpandido = !espanolExpandido;
        nivelesEspContainer.style.display = espanolExpandido ? DisplayStyle.Flex : DisplayStyle.None;
        flechaEspanol.text = espanolExpandido ? "∧" : "∨";
    }

    // ── Generar botones de niveles ───────────────────────────────

    void GenerarNiveles(VisualElement contenedor, List<NivelData> niveles)
    {
        contenedor.Clear();
        for (int i = 0; i < niveles.Count; i++)
        {
            var nivel = niveles[i];
            var btn = new Button();
            btn.AddToClassList("btn-nivel-item");

            // Badge con número
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

            // Solo los desbloqueados navegan
            if (nivel.desbloqueado)
            {
                string escena = nivel.nombreEscena;
                btn.clicked += () => SceneManager.LoadScene(escena);
            }

            contenedor.Add(btn);
        }
    }
}

// ── Estructura de datos de un nivel ──────────────────────────────
[System.Serializable]
public class NivelData
{
    public string nombre;
    public string nombreEscena;
    public bool   desbloqueado;
}
