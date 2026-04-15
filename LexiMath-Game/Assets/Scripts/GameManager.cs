using UnityEngine;

public class GameManager : MonoBehaviour
{
    // En este bloque declaro el patrón Singleton. Lo configuré así para que este GameManager 
    // sea único en toda la escena y actúe como el "cerebro central". Cualquier enemigo, 
    // moneda o portal puede encontrarlo fácilmente escribiendo 'GameManager.instance'.
    public static GameManager instance;

    [Header("Economía y Puntos")]
    // Aquí guardo los datos duros de la partida de Mael. Las mantengo privadas para que 
    // otros scripts no las modifiquen por accidente, solo pueden hacerlo a través de mis funciones.
    private int monedasActuales = 0;
    private int puntosTotales = 0;

    [Header("Reglas del Nivel")]
    // Estas variables son públicas para que pueda ajustarlas desde el Inspector de Unity 
    // dependiendo de la dificultad de cada nivel, sin tener que abrir el código.
    public int puntosPara2Estrellas = 500;
    public int puntosPara3Estrellas = 1000;

    void Awake()
    {
        // En este bloque de código lo que se hace es asegurar que solo exista un GameManager.
        // Si al iniciar el juego la instancia está vacía, este script se asigna a sí mismo.
        if (instance == null) instance = this;
    }

    // --- FUNCIONES DE RECOLECCIÓN ---

    // Esta función la van a llamar las monedas físicas cuando Mael las toque.
    // Le puse un valor por defecto (cantidad = 1) por si en el futuro hago una moneda gigante que valga más.
    public void SumarMoneda(int cantidad = 1)
    {
        // 1. Lógica interna: sumo el valor a mi billetera.
        monedasActuales += cantidad;
        
        // 2. Delegación visual: Le aviso al UIManager (el encargado de la pantalla) 
        // que el número cambió para que actualice el texto. Yo no toco la interfaz desde aquí.
        UIManager.instance.ActualizarMonedas(monedasActuales);
    }

    // Esta función la llamarán los enemigos al morir (o el Boss).
    public void SumarPuntos(int cantidad)
    {
        // Lógica de datos: acumulo los puntos.
        puntosTotales += cantidad;

        // Actualizo el marcador de puntos en vivo en el HUD.
        UIManager.instance.ActualizarPuntos(puntosTotales);
    }

    // --- FUNCIÓN DE FINALIZACIÓN DE NIVEL ---

    // El portal llama a esta función cuando Mael lo cruza. Es la parte más compleja 
    // porque aquí se evalúa todo el rendimiento del jugador.
    public void FinalizarNivel()
    {
        // 1. CONGELAR EL JUEGO
        // En este bloque de código lo que se hace es detener el tiempo de Unity. 
        // Al poner el timeScale en 0, las físicas, enemigos y animaciones se pausan 
        // para que el jugador pueda ver la pantalla de victoria con calma.
        Time.timeScale = 0f;

        // 2. PERSISTENCIA DE DATOS (EL BANCO)
        // En este bloque utilizo PlayerPrefs, que es como un pequeño disco duro de Unity.
        // Primero, consulto cuántas monedas tenía guardadas de niveles anteriores.
        // (El 0 es por si es la primera vez que juega y no tiene fondos).
        int monedasGuardadas = PlayerPrefs.GetInt("MonedasTotales", 0);
        
        // Luego, sumo lo que ya tenía en el banco con lo que acaba de ganar en este nivel
        // y lo vuelvo a guardar de forma permanente para que las use en la tienda.
        PlayerPrefs.SetInt("MonedasTotales", monedasGuardadas + monedasActuales);
        PlayerPrefs.Save();

        // 3. CÁLCULO DE ESTRELLAS
        // Aquí evalúo el rendimiento del jugador. Le doy 1 estrella de base solo por sobrevivir y llegar al portal.
        int estrellasGanadas = 1; 
        
        // Uso condicionales de mayor a menor para asignar la recompensa justa según los umbrales.
        if (puntosTotales >= puntosPara3Estrellas) 
        {
            estrellasGanadas = 3;
        }
        else if (puntosTotales >= puntosPara2Estrellas) 
        {
            estrellasGanadas = 2;
        }

        // 4. EJECUCIÓN VISUAL FINAL
        // Finalmente, ya que hice todos los cálculos matemáticos y de guardado,
        // le paso el resultado final al UIManager para que haga su magia y dibuje la pantalla.
        UIManager.instance.MostrarVictoria(puntosTotales, estrellasGanadas);
    }
}