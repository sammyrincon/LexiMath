using UnityEngine;
using UnityEngine.UIElements;

public class HUDController : MonoBehaviour
{
    private Label labelNivel;
    private Label labelEstrellas;

    private int nivelActual = 1;
    private int estrellasRecogidas = 0;
    private int estrellasTotal = 3;

    // Singleton para que otros scripts puedan acceder fácilmente
    public static HUDController Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        labelNivel    = root.Q<Label>("label-nivel");
        labelEstrellas = root.Q<Label>("label-estrellas");

        ActualizarNivel();
        ActualizarEstrellas();
    }

    // ── Nivel ──────────────────────────────────────

    public void SetNivel(int numero)
    {
        nivelActual = numero;
        ActualizarNivel();
    }

    private void ActualizarNivel()
    {
        if (labelNivel != null)
            labelNivel.text = $"Level {nivelActual}";
    }

    // ── Estrellas ──────────────────────────────────

    /// <summary>
    /// Llama esto al iniciar cada nivel para configurar el total de estrellas.
    /// </summary>
    public void SetEstrellasTotal(int total)
    {
        estrellasTotal    = total;
        estrellasRecogidas = 0;
        ActualizarEstrellas();
    }

    /// <summary>
    /// Llama esto cuando el jugador recoge una estrella.
    /// </summary>
    public void RecogerEstrella()
    {
        estrellasRecogidas = Mathf.Clamp(estrellasRecogidas + 1, 0, estrellasTotal);
        ActualizarEstrellas();
    }

    private void ActualizarEstrellas()
    {
        if (labelEstrellas != null)
            labelEstrellas.text = $"★ {estrellasRecogidas} / {estrellasTotal}";
    }
}
