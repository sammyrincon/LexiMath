using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class BookPauseController : MonoBehaviour
{
    private VisualElement pauseScreen;
    private VisualElement bookContainer;
    private VisualElement buttonsContainer;
    private VisualElement rightPageContainer; // Página derecha del libro

    // Botones originales
    private Button btnContinuar;
    private Button btnSonido;
    private Button btnSalir;

    // Botones nuevos
    private Button btnControles;
    private Button btnGraficos;

    [Header("Configuración de Sprites")]
    public List<Sprite> framesDelLibro;
    public float velocidadFrame = 0.05f;

    [Header("Audio (opcional)")]
    public AudioMixer audioMixer; // Arrastra tu AudioMixer aquí en el Inspector

    private bool estaAnimando = false;
    private bool estaPausado = false;
    private string panelActivo = ""; // Qué panel está mostrando la página derecha

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Contenedores
        pauseScreen        = root.Q<VisualElement>("pause-screen");
        bookContainer      = root.Q<VisualElement>("book-animation-container");
        buttonsContainer   = root.Q<VisualElement>("buttons-container");
        rightPageContainer = root.Q<VisualElement>("right-page-container"); // Nuevo en el UXML

        // Botones originales
        btnContinuar = root.Q<Button>("button-continuar");
        btnSonido    = root.Q<Button>("button-sonido");
        btnSalir     = root.Q<Button>("button-salir");

        // Botones nuevos
        btnControles = root.Q<Button>("button-controles");
        btnGraficos  = root.Q<Button>("button-graficos");

        // Acciones
        if (btnContinuar != null) btnContinuar.clicked += IntentarCambiarPausa;
        if (btnSalir     != null) btnSalir.clicked     += IrAlMenuPrincipal;
        if (btnSonido    != null) btnSonido.clicked    += () => MostrarPanel("sonido");
        if (btnControles != null) btnControles.clicked += () => MostrarPanel("controles");
        if (btnGraficos  != null) btnGraficos.clicked  += () => MostrarPanel("graficos");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            IntentarCambiarPausa();
    }

    // ─────────────────────────────────────────────
    //  PÁGINA DERECHA: mostrar panel según botón
    // ─────────────────────────────────────────────

    void MostrarPanel(string panel)
    {
        if (rightPageContainer == null) return;

        // Si ya estaba abierto ese panel, lo cerramos (toggle)
        if (panelActivo == panel)
        {
            rightPageContainer.Clear();
            rightPageContainer.style.display = DisplayStyle.None;
            panelActivo = "";
            return;
        }

        panelActivo = panel;
        rightPageContainer.Clear();
        rightPageContainer.style.display = DisplayStyle.Flex;

        switch (panel)
        {
            case "sonido":    ConstruirPanelSonido();    break;
            case "controles": ConstruirPanelControles(); break;
            case "graficos":  ConstruirPanelGraficos();  break;
        }
    }

    void ConstruirPanelSonido()
    {
        AgregarTitulo("Sonido");

        AgregarSlider("Música", "slider-musica", 0f, 1f, 0.8f, valor =>
        {
            // Si tienes AudioMixer: audioMixer.SetFloat("VolumenMusica", Mathf.Log10(valor) * 20);
            Debug.Log($"Música: {valor}");
        });

        AgregarSlider("Efectos", "slider-efectos", 0f, 1f, 1f, valor =>
        {
            // audioMixer.SetFloat("VolumenEfectos", Mathf.Log10(valor) * 20);
            Debug.Log($"Efectos: {valor}");
        });
    }

    void ConstruirPanelControles()
    {
        AgregarTitulo("Controles");

        var controles = new Dictionary<string, string>
        {
            { "Mover",      "A / D  ó  ← →" },
            { "Saltar",     "Espacio" },
            { "Atacar",     "J  ó  Click" },
            { "Pausar",     "Escape" },
            { "Dash",       "Shift" },
        };

        foreach (var kvp in controles)
            AgregarFilaControles(kvp.Key, kvp.Value);
    }

    void ConstruirPanelGraficos()
    {
        AgregarTitulo("Gráficos");

        // Toggle de pantalla completa
        var toggle = new Toggle("Pantalla Completa");
        toggle.value = Screen.fullScreen;
        toggle.AddToClassList("right-page-toggle");
        toggle.RegisterValueChangedCallback(evt => Screen.fullScreen = evt.newValue);
        rightPageContainer.Add(toggle);

        // Resoluciones comunes
        AgregarTitulo("Resolución");
        var resoluciones = new[] { "1280x720", "1920x1080", "2560x1440" };
        foreach (var res in resoluciones)
        {
            var btn = new Button(() => AplicarResolucion(res)) { text = res };
            btn.AddToClassList("right-page-button");
            rightPageContainer.Add(btn);
        }
    }

    // ─────────────────────────────────────────────
    //  HELPERS para construir la UI de la página
    // ─────────────────────────────────────────────

    void AgregarTitulo(string texto)
    {
        var label = new Label(texto);
        label.AddToClassList("right-page-title");
        rightPageContainer.Add(label);
    }

    void AgregarSlider(string nombre, string name, float min, float max, float valorInicial, System.Action<float> onChange)
    {
        var label = new Label(nombre);
        label.AddToClassList("right-page-label");

        var slider = new Slider(min, max) { name = name, value = valorInicial };
        slider.AddToClassList("right-page-slider");
        slider.RegisterValueChangedCallback(evt => onChange(evt.newValue));

        rightPageContainer.Add(label);
        rightPageContainer.Add(slider);
    }

    void AgregarFilaControles(string accion, string tecla)
    {
        var fila = new VisualElement();
        fila.AddToClassList("right-page-row");

        var lblAccion = new Label(accion);
        lblAccion.AddToClassList("right-page-label");

        var lblTecla = new Label(tecla);
        lblTecla.AddToClassList("right-page-key");

        fila.Add(lblAccion);
        fila.Add(lblTecla);
        rightPageContainer.Add(fila);
    }

    void AplicarResolucion(string resString)
    {
        var partes = resString.Split('x');
        if (partes.Length == 2 && int.TryParse(partes[0], out int w) && int.TryParse(partes[1], out int h))
            Screen.SetResolution(w, h, Screen.fullScreen);
    }

    // ─────────────────────────────────────────────
    //  ANIMACIÓN DEL LIBRO (igual que antes)
    // ─────────────────────────────────────────────

    void IrAlMenuPrincipal()
    {
        Time.timeScale = 1; // Aseguramos que el tiempo vuelva a la normalidad
        SceneManager.LoadScene("MainMenu");
    }

    void IntentarCambiarPausa()
    {
        if (estaAnimando) return;
        if (!estaPausado) StartCoroutine(AbrirLibro());
        else StartCoroutine(CerrarLibro());
    }

    IEnumerator AbrirLibro()
    {
        estaAnimando = true;
        estaPausado  = true;
        Time.timeScale = 0;

        pauseScreen.style.display    = DisplayStyle.Flex;
        buttonsContainer.style.display = DisplayStyle.None;
        if (rightPageContainer != null)
            rightPageContainer.style.display = DisplayStyle.None;

        for (int i = 0; i < framesDelLibro.Count; i++)
        {
            bookContainer.style.backgroundImage = new StyleBackground(framesDelLibro[i]);
            yield return new WaitForSecondsRealtime(velocidadFrame);
        }

        buttonsContainer.style.display = DisplayStyle.Flex;
        estaAnimando = false;
    }

    IEnumerator CerrarLibro()
    {
        estaAnimando = true;
        buttonsContainer.style.display = DisplayStyle.None;

        // Limpiar página derecha al cerrar
        if (rightPageContainer != null)
        {
            rightPageContainer.Clear();
            rightPageContainer.style.display = DisplayStyle.None;
        }
        panelActivo = "";

        for (int i = framesDelLibro.Count - 1; i >= 0; i--)
        {
            bookContainer.style.backgroundImage = new StyleBackground(framesDelLibro[i]);
            yield return new WaitForSecondsRealtime(velocidadFrame);
        }

        pauseScreen.style.display = DisplayStyle.None;
        Time.timeScale = 1;
        estaPausado  = false;
        estaAnimando = false;
    }
}