using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public float maxHealth = 100f;
    public float danoPorGolpe = 20f;

    [Header("Umbral marco rojo")]
    [Range(0f, 1f)]
    public float umbralPeligro = 0.2f;

    [Header("Referencia al Animator del Knight")]
    public Animator knightAnimator;

    [Header("Tiempo de invulnerabilidad tras golpe")]
    public float tiempoInvulnerable = 1f;

    [Header("Escena de Menú Principal")]
    public string nombreEscenaMenu = "MainMenu";

    public static PlayerHealth Instance { get; private set; }

    private float _currentHealth;
    private bool  _muerto       = false;
    private bool  _invulnerable = false;

    private VisualElement _healthFill;
    private VisualElement _dangerFrame;
    private VisualElement _gameOverPanel;
    private Button        _btnMenu;
    private Button        _btnRetry;

    private Coroutine _corrutinaParpadeo;

    public float VidaActual => _currentHealth;
    public bool  EstaMuerto => _muerto;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        _healthFill    = root.Q<VisualElement>("healthbar-fill");
        _dangerFrame   = root.Q<VisualElement>("danger-frame");
        _gameOverPanel = root.Q<VisualElement>("gameover-panel");
        _btnMenu       = root.Q<Button>("gameover-menu-btn");
        _btnRetry      = root.Q<Button>("gameover-retry-btn");

        _btnMenu?.RegisterCallback<ClickEvent>(_ => IrAlMenu());
        _btnRetry?.RegisterCallback<ClickEvent>(_ => ReiniciarNivel());

        OcultarMarcoRojo();
        OcultarGameOver();

        _currentHealth = maxHealth;
        _muerto        = false;
        _invulnerable  = false;
        ActualizarBarra();
    }

    public void RecibirDano()
    {
        RecibirDano(danoPorGolpe);
    }

    public void RecibirDano(float cantidad)
    {
        if (_muerto || _invulnerable) return;

        _currentHealth = Mathf.Clamp(_currentHealth - cantidad, 0, maxHealth);
        ActualizarBarra();

        if (knightAnimator != null && _currentHealth > 0)
            knightAnimator.SetTrigger("Hurt");

        RevisarVidaBaja();

        if (_currentHealth <= 0)
        {
            Morir();
        }
        else
        {
            StartCoroutine(Invulnerabilidad());
        }
    }

    public void Curar(float cantidad)
    {
        if (_muerto) return;
        _currentHealth = Mathf.Clamp(_currentHealth + cantidad, 0, maxHealth);
        ActualizarBarra();
        RevisarVidaBaja();
    }

    public void RestablecerVida()
    {
        _muerto        = false;
        _currentHealth = maxHealth;
        ActualizarBarra();
        OcultarMarcoRojo();
        OcultarGameOver();
    }

    private void ActualizarBarra()
    {
        if (_healthFill == null) return;
        float porcentaje = (_currentHealth / maxHealth) * 76f;
        _healthFill.style.width = new Length(porcentaje, LengthUnit.Pixel);
    }

    private void RevisarVidaBaja()
    {
        float ratio = _currentHealth / maxHealth;
        if (ratio <= umbralPeligro && ratio > 0)
            MostrarMarcoRojo();
        else
            OcultarMarcoRojo();
    }

    private void MostrarMarcoRojo()
    {
        if (_dangerFrame == null) return;
        _dangerFrame.RemoveFromClassList("danger-hidden");

        if (_corrutinaParpadeo == null)
            _corrutinaParpadeo = StartCoroutine(ParpadearMarco());
    }

    private void OcultarMarcoRojo()
    {
        if (_dangerFrame == null) return;

        if (_corrutinaParpadeo != null)
        {
            StopCoroutine(_corrutinaParpadeo);
            _corrutinaParpadeo = null;
        }

        _dangerFrame.AddToClassList("danger-hidden");
    }

    private IEnumerator ParpadearMarco()
    {
        while (true)
        {
            _dangerFrame.style.opacity = 0.3f;
            yield return new WaitForSeconds(0.35f);
            _dangerFrame.style.opacity = 1f;
            yield return new WaitForSeconds(0.35f);
        }
    }

    private void Morir()
    {
        _muerto = true;
        OcultarMarcoRojo();

        if (knightAnimator != null)
            knightAnimator.SetTrigger("Die");

        StartCoroutine(MostrarGameOverConDelay(1.5f));
    }

    private IEnumerator MostrarGameOverConDelay(float seg)
    {
        yield return new WaitForSeconds(seg);
        MostrarGameOver();
    }

    private void MostrarGameOver()
    {
        if (_gameOverPanel == null) return;
        _gameOverPanel.RemoveFromClassList("gameover-hidden");
        Time.timeScale = 0f;
    }

    private void OcultarGameOver()
    {
        if (_gameOverPanel == null) return;
        _gameOverPanel.AddToClassList("gameover-hidden");
        Time.timeScale = 1f;
    }

    private IEnumerator Invulnerabilidad()
    {
        _invulnerable = true;
        yield return new WaitForSeconds(tiempoInvulnerable);
        _invulnerable = false;
    }

    private void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void IrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}
