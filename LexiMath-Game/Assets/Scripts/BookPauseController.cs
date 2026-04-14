using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class BookPauseController : MonoBehaviour
{
    private VisualElement pauseScreen;
    private VisualElement bookContainer;
    private VisualElement buttonsContainer;
    
    // Referencias a tus 3 botones específicos
    private Button btnContinuar;
    private Button btnSonido;
    private Button btnSalir;

    [Header("Configuración de Sprites")]
    public List<Sprite> framesDelLibro;
    public float velocidadFrame = 0.05f;

    private bool estaAnimando = false;
    private bool estaPausado = false;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        // 1. Buscamos los contenedores 
        pauseScreen = root.Q<VisualElement>("pause-screen");
        bookContainer = root.Q<VisualElement>("book-animation-container");
        buttonsContainer = root.Q<VisualElement>("buttons-container");
        
        // 2. Buscamos los botones por sus nuevos nombres
        btnContinuar = root.Q<Button>("button-continuar");
        btnSonido = root.Q<Button>("button-sonido");
        btnSalir = root.Q<Button>("button-salir");

        // 3. Conectamos las acciones a los botones
        if (btnContinuar != null) btnContinuar.clicked += IntentarCambiarPausa;
        if (btnSalir != null) btnSalir.clicked += () => Application.Quit();
        
        // El botón de sonido lo dejamos listo para que luego le pongas tu lógica
        if (btnSonido != null) btnSonido.clicked += () => Debug.Log("Cambiando Sonido...");
    }

    void Update()
    {
        // También podemos abrirlo/cerrarlo con la tecla Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IntentarCambiarPausa();
        }
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
        estaPausado = true;
        Time.timeScale = 0; // Pausa el juego
        
        pauseScreen.style.display = DisplayStyle.Flex;
        buttonsContainer.style.display = DisplayStyle.None; // Botones ocultos al inicio

        for (int i = 0; i < framesDelLibro.Count; i++)
        {
            bookContainer.style.backgroundImage = new StyleBackground(framesDelLibro[i]);
            yield return new WaitForSecondsRealtime(velocidadFrame);
        }
        
        // Aparecen los botones después de la animación
        buttonsContainer.style.display = DisplayStyle.Flex;
        estaAnimando = false;
    }

    IEnumerator CerrarLibro()
    {
        estaAnimando = true;
        buttonsContainer.style.display = DisplayStyle.None; // Ocultar botones antes de cerrar

        for (int i = framesDelLibro.Count - 1; i >= 0; i--)
        {
            bookContainer.style.backgroundImage = new StyleBackground(framesDelLibro[i]);
            yield return new WaitForSecondsRealtime(velocidadFrame);
        }

        pauseScreen.style.display = DisplayStyle.None;
        Time.timeScale = 1; // Reanudar el juego
        estaPausado = false;
        estaAnimando = false;
    }
}