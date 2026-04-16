using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // En este bloque declaro el patrón Singleton. Lo hice así para poder llamar a este script
    // desde cualquier otro lado (como el GameManager) sin tener que usar GetComponent cada vez.
    public static UIManager instance;

    [Header("Elementos del HUD en Gameplay")]
    private Label labelMonedas;
    private Label labelPuntosHUD;

    [Header("Elementos de la Pantalla de Victoria")]
    private VisualElement victoryPanel;
    private Label pointsNumber;
    private VisualElement warningLevel;
    
    // Uso un arreglo aquí para guardar las 3 estrellas juntas y que sea más fácil 
    // recorrerlas luego con un ciclo 'for'.
    private VisualElement[] estrellas = new VisualElement[3];

    [Header("Botones")]
    private Button btnMenu;
    private Button btnRetry;
    private Button btnContinue;

    void Awake()
    {
        // Asigno la instancia a este script justo cuando el juego despierta.
        if (instance == null) instance = this;
    }

    void OnEnable()
    {
        // En este bloque de código lo que se hace es buscar la "raíz" (root) de mi interfaz gráfica.
        // Es como agarrar la carpeta principal donde están todos los elementos de diseño de UI Builder.
        var root = GetComponent<UIDocument>().rootVisualElement;

        // --- VINCULACIÓN DEL HUD ---
        // Aquí busco los textos de monedas y puntos usando los nombres exactos de mi archivo UXML.
        labelMonedas = root.Q<Label>("ContadorTexto");
        labelPuntosHUD = root.Q<Label>("PuntosGameplay");

        // --- VINCULACIÓN DEL PANEL DE VICTORIA ---
        victoryPanel = root.Q<VisualElement>("Victory-panel");
        pointsNumber = root.Q<Label>("PointsNumber");
        warningLevel = root.Q<VisualElement>("warningLevel");

        // Lleno mi arreglo de estrellas buscando una por una. El símbolo "$" me permite 
        // meter el número de la variable directamente en la cadena de texto de forma dinámica.
        for (int i = 0; i < 3; i++)
        {
            estrellas[i] = root.Q<VisualElement>($"Star{i + 1}");
        }

        // --- VINCULACIÓN DE BOTONES ---
        btnMenu = root.Q<Button>("menuButton");
        btnRetry = root.Q<Button>("retryButton");
        btnContinue = root.Q<Button>("continueButton");

        // Me aseguro de que el panel de victoria empiece apagado para que no tape la pantalla al jugar.
        if (victoryPanel != null) victoryPanel.style.display = DisplayStyle.None;

        // --- ASIGNACIÓN DE EVENTOS A LOS BOTONES ---
        // En este bloque de código, lo que hago es decirle a cada botón qué función debe 
        // ejecutar cuando el jugador le dé clic en la pantalla.
        // Uso "=>" (expresiones lambda) para enlazar la función en una sola línea.
        if (btnRetry != null) btnRetry.clicked += () => ReiniciarNivel();
        if (btnMenu != null) btnMenu.clicked += () => IrAlMenu();
        if (btnContinue != null) btnContinue.clicked += () => SiguienteNivel();
    }

    // --- FUNCIONES QUE ACTUALIZAN EL TEXTO EN PANTALLA ---

    public void ActualizarMonedas(int cantidad)
    {
        // Verifico que el label no sea nulo antes de cambiarlo para evitar el clásico error NullReferenceException.
        if (labelMonedas != null) labelMonedas.text = cantidad.ToString();
    }

    public void ActualizarPuntos(int cantidad)
    {
        if (labelPuntosHUD != null) labelPuntosHUD.text = cantidad.ToString();
    }

    // --- FUNCIÓN PRINCIPAL DE VICTORIA ---

    // Esta función es llamada por el GameManager cuando el jugador toca el portal.
    // Recibe los puntos finales y cuántas estrellas se ganó calculadas previamente en el otro script.
    public void MostrarVictoria(int puntosFinales, int cantidadEstrellas)
    {
        // 1. Prendo el panel oscuro principal cambiando su display a Flex.
        if (victoryPanel != null) victoryPanel.style.display = DisplayStyle.Flex;
        
        // 2. Muestro los puntos en el texto grande rojo del mapa de madera.
        if (pointsNumber != null) pointsNumber.text = puntosFinales.ToString();

        // 3. ENCENDIDO DE ESTRELLAS
        // En este bloque de código lo que se hace es recorrer el arreglo de las 3 estrellas.
        // Uso el operador ternario "(condición) ? verdadero : falso" para que sea más eficiente.
        // Funciona así: Si la posición de la estrella que estoy revisando es menor al número de 
        // estrellas que ganó el jugador, le quito el filtro y se ve blanca (Color.white). 
        // Si no, le pongo un filtro gris (Color.gray) para que se vea apagada.
        for (int i = 0; i < 3; i++)
        {
            estrellas[i].style.unityBackgroundImageTintColor = (i < cantidadEstrellas) ? Color.white : Color.gray;
        }

        // 4. LÓGICA DE BLOQUEO DE NIVEL
        // En este bloque evaluamos la regla de negocio: ¿Tiene 2 o más estrellas?
        if (cantidadEstrellas < 2)
        {
            // Como tiene menos de 2, muestro el cuadro amarillo de advertencia.
            warningLevel.style.display = DisplayStyle.Flex; 
            
            // Desactivo el botón de continuar para que no pueda presionarlo por accidente,
            // y le bajo la opacidad a la mitad (0.5f) para darle feedback visual al usuario.
            btnContinue.SetEnabled(false); 
            btnContinue.style.opacity = 0.5f;
        }
        else
        {
            // Como sí pasó el nivel, oculto la advertencia.
            warningLevel.style.display = DisplayStyle.None; 
            
            // Y dejo el botón de continuar funcionando al 100%.
            btnContinue.SetEnabled(true); 
            btnContinue.style.opacity = 1f;
        }
    }

    // --- FUNCIONES DE NAVEGACIÓN DE ESCENAS ---

    private void ReiniciarNivel()
    {
        // Regreso el tiempo a la normalidad (1f) en caso de que el GameManager lo haya pausado (0f).
        Time.timeScale = 1f;
        // Le pido al SceneManager que recargue la escena en la que estoy actualmente.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void IrAlMenu()
    {
        Time.timeScale = 1f;
        // OJO: Aquí tengo que asegurarme de escribir exactamente el nombre de la escena del menú.
        SceneManager.LoadScene("MenuPrincipal"); 
    }

    private void SiguienteNivel()
    {
        Time.timeScale = 1f;
        // OJO: Aquí tengo que poner el nombre real de mi siguiente mundo/nivel.
        SceneManager.LoadScene("Nivel2"); 
    }
}