using UnityEngine;
using TMPro;

public class PlataformaRespuestaRM : MonoBehaviour
{
    // ── Referencias ────────────────────────────────────────────
    [Header("Texto con el número de la opción")]
    [SerializeField] private TextMeshPro _textoRespuesta;

    [Header("Sprites del cubito")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite         _spriteNormal;
    [SerializeField] private Sprite         _spriteCorrecto;
    [SerializeField] private Sprite         _spriteIncorrecto;

    // ── Datos internos ─────────────────────────────────────────
    private bool                _esCorrecta;
    private bool                _yaGolpeado = false;
    private System.Action<bool> _onRespuesta;

    // ──────────────────────────────────────────────────────────
    /// <summary>
    /// Inicializa el cubito con su valor y si es la respuesta correcta.
    /// Llamado por PreguntaUIRM al activarse.
    /// </summary>
    public void Inicializar(int valor, bool esCorrecta, System.Action<bool> callback)
    {
        _esCorrecta  = esCorrecta;
        _onRespuesta = callback;
        _yaGolpeado  = false;

        // Mostrar número en el cubito
        if (_textoRespuesta != null)
            _textoRespuesta.text = valor.ToString();

        // Resetear sprite al normal
        if (_spriteRenderer != null && _spriteNormal != null)
            _spriteRenderer.sprite = _spriteNormal;
    }

    // ──────────────────────────────────────────────────────────
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_yaGolpeado) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        // Verificar que el jugador viene desde abajo
        if (!JugadorVieneDeAbajo(collision)) return;

        _yaGolpeado = true;

        // Mostrar feedback visual inmediato
        MostrarFeedback();

        // Animar y notificar
        StartCoroutine(AnimarYNotificar());

        Debug.Log($"[PlataformaRespuestaRM] Golpeado. " +
                  $"Valor: {_textoRespuesta?.text} — Correcta: {_esCorrecta}");
    }

    // ──────────────────────────────────────────────────────────
    /// <summary>
    /// El jugador viene de abajo cuando la normal del contacto
    /// apunta hacia arriba (y positivo en 2D desde el cubito).
    /// </summary>
    private bool JugadorVieneDeAbajo(Collision2D collision)
    {
        foreach (ContactPoint2D contacto in collision.contacts)
        {
            if (contacto.normal.y > 0.5f)
                return true;
        }
        return false;
    }

    // ──────────────────────────────────────────────────────────
    /// <summary>
    /// Cambia el sprite según si era correcta o no.
    /// </summary>
    private void MostrarFeedback()
    {
        if (_spriteRenderer == null) return;

        if (_esCorrecta && _spriteCorrecto != null)
            _spriteRenderer.sprite = _spriteCorrecto;
        else if (!_esCorrecta && _spriteIncorrecto != null)
            _spriteRenderer.sprite = _spriteIncorrecto;
    }

    // ──────────────────────────────────────────────────────────
    /// <summary>
    /// Animación de golpe hacia arriba como Mario Bros.
    /// Sube 0.3 unidades y baja, luego notifica el resultado.
    /// </summary>
    private System.Collections.IEnumerator AnimarYNotificar()
    {
        Vector3 posOriginal = transform.position;
        Vector3 posArriba   = posOriginal + new Vector3(0f, 0.3f, 0f);

        // Subir rápido
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 15f;
            transform.position = Vector3.Lerp(posOriginal, posArriba, t);
            yield return null;
        }

        // Bajar rápido
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 15f;
            transform.position = Vector3.Lerp(posArriba, posOriginal, t);
            yield return null;
        }

        // Notificar resultado al PreguntaUIRM
        _onRespuesta?.Invoke(_esCorrecta);
    }
}