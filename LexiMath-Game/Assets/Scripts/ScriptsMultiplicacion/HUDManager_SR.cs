using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class HUDManager_SR : MonoBehaviour
{
    public static HUDManager_SR Instance { get; private set; }

    private VisualElement healthbarFill;
    private Label labelNivel;
    private Label labelContador;

    private VisualElement victoryPanel;
    private Label victoryTitle;
    private VisualElement[] estrellas = new VisualElement[3];
    private Label pointsNumber;
    private Label monedasVictoryLabel;
    private VisualElement warningLevel;
    private Button btnMenu;
    private Button btnRetry;
    private Button btnContinue;
    private Button btnClose;

    private VisualElement gameoverPanel;
    private Button btnGameoverMenu;
    private Button btnGameoverRetry;

    private int puntosAcumulados   = 0;
    private int monedasAcumuladas  = 0;
    private int maxHealthRef       = 100;

    [Header("Siguiente escena al continuar")]
    public string siguienteEscena = "MainMenu";

    [Header("Umbrales de estrellas")]
    public int puntosPara2Estrellas = 300;
    public int puntosPara3Estrellas = 600;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void OnEnable()
    {
        var doc  = GetComponent<UIDocument>() ?? FindFirstObjectByType<UIDocument>();
        if (doc == null) { Debug.LogError("HUDManager_SR: no hay UIDocument en la escena."); return; }
        var root = doc.rootVisualElement;

        healthbarFill = root.Q<VisualElement>("healthbar-fill");
        labelNivel    = root.Q<Label>("label-nivel");
        labelContador = root.Q<Label>("ContadorTexto");

        victoryPanel  = root.Q<VisualElement>("Victory-panel");
        victoryTitle  = root.Q<Label>("victoryTitle");
        pointsNumber  = root.Q<Label>("PointsNumber");
        monedasVictoryLabel = root.Q<Label>("monedasVictory");
        warningLevel  = root.Q<VisualElement>("warningLevel");

        for (int i = 0; i < 3; i++)
            estrellas[i] = root.Q<VisualElement>($"Star{i + 1}");

        btnMenu     = root.Q<Button>("menuButton");
        btnRetry    = root.Q<Button>("retryButton");
        btnContinue = root.Q<Button>("continueButton");
        btnClose    = root.Q<Button>("closeButton");

        if (victoryPanel != null) victoryPanel.style.display = DisplayStyle.None;

        if (btnMenu     != null) btnMenu.clicked     += IrAlMenu;
        if (btnRetry    != null) btnRetry.clicked    += Reintentar;
        if (btnContinue != null) btnContinue.clicked += Continuar;
        if (btnClose    != null) btnClose.clicked    += CerrarJuego;

        gameoverPanel     = root.Q<VisualElement>("gameover-panel");
        btnGameoverMenu   = root.Q<Button>("gameover-menu-btn");
        btnGameoverRetry  = root.Q<Button>("gameover-retry-btn");

        if (gameoverPanel   != null) gameoverPanel.AddToClassList("gameover-hidden");
        if (btnGameoverMenu  != null) btnGameoverMenu.clicked  += IrAlMenu;
        if (btnGameoverRetry != null) btnGameoverRetry.clicked += Reintentar;
    }

    public void SetMaxHealth(int max) => maxHealthRef = max;

    public void ActualizarVida(int current)
    {
        if (healthbarFill == null) return;
        float pct = Mathf.Clamp01((float)current / maxHealthRef);
        healthbarFill.style.width = new StyleLength(new Length(pct * 76f, LengthUnit.Pixel));
    }

    public void AgregarPuntos(int cantidad)
    {
        puntosAcumulados += cantidad;
    }

    public void AgregarMonedas(int cantidad)
    {
        monedasAcumuladas += cantidad;
        if (labelContador != null) labelContador.text = monedasAcumuladas.ToString();
    }

    public void SetNivel(string nombre)
    {
        if (labelNivel != null) labelNivel.text = nombre;
    }

    public void MostrarVictoria()
    {
        Time.timeScale = 0f;

        int estrellaCount = 1;
        if (puntosAcumulados >= puntosPara3Estrellas)      estrellaCount = 3;
        else if (puntosAcumulados >= puntosPara2Estrellas) estrellaCount = 2;

        if (victoryPanel != null) victoryPanel.style.display = DisplayStyle.Flex;
        if (pointsNumber != null) pointsNumber.text = puntosAcumulados.ToString();
        if (monedasVictoryLabel != null) monedasVictoryLabel.text = monedasAcumuladas.ToString();

        for (int i = 0; i < 3; i++)
            if (estrellas[i] != null)
                estrellas[i].style.unityBackgroundImageTintColor =
                    i < estrellaCount ? Color.white : new Color(0.3f, 0.3f, 0.3f, 0.6f);

        bool pasa = estrellaCount >= 2;
        if (warningLevel  != null) warningLevel.style.display = pasa ? DisplayStyle.None : DisplayStyle.Flex;
        if (btnContinue   != null) { btnContinue.SetEnabled(pasa); btnContinue.style.opacity = pasa ? 1f : 0.4f; }
    }

    public void MostrarGameOver()
    {
        if (gameoverPanel == null) return;
        gameoverPanel.RemoveFromClassList("gameover-hidden");
        gameoverPanel.AddToClassList("gameover-visible");
    }

    private void IrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void Reintentar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Continuar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(siguienteEscena);
    }

    private void CerrarJuego()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
