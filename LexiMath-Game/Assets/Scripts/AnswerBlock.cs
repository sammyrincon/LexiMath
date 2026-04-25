using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// AnswerBlock — LexiMath
/// 
/// Bloque de respuesta estilo Mario.
/// Cuando el Knight lo golpea por abajo (saltando), notifica al QuestionManager.
/// 
/// DETECCIÓN:
///   El Knight tiene un CircleCollider2D con Is Trigger = true en la cabeza.
///   Cuando ese trigger entra en contacto con este bloque, se activa.
///   Usamos tag "Player" (no necesita otro tag extra).
/// 
/// SETUP EN UNITY:
///   1. GameObject con Sprite Renderer (sprite del bloque)
///   2. BoxCollider2D (NO trigger — para colisión sólida)
///   3. Este script
///   4. Hijo con TextMeshPro - Text
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class AnswerBlock : MonoBehaviour
{
    [Header("Contenido")]
    public string valor = "1";
    public bool esCorrecto = false;

    [Header("Visual")]
    public TextMeshPro textoVisor;
    public SpriteRenderer spriteRenderer;
    public Sprite spriteNormal;
    public Sprite spriteGolpeado;

    [Header("Animación de rebote")]
    public float alturaRebote = 0.3f;
    public float duracionRebote = 0.12f;

    // ── Privados ─────────────────────────────────────────────
    private bool _yaGolpeado = false;
    private Vector3 _posOriginal;
    private QuestionManager _manager;

    void Start()
    {
        _posOriginal = transform.position;
        RefrescarTexto();
    }

    public void Configurar(string nuevoValor, bool correcto, QuestionManager manager)
    {
        valor = nuevoValor;
        esCorrecto = correcto;
        _manager = manager;
        _yaGolpeado = false;

        if (spriteRenderer != null && spriteNormal != null)
            spriteRenderer.sprite = spriteNormal;

        RefrescarTexto();
    }

    private void RefrescarTexto()
    {
        if (textoVisor != null) textoVisor.text = valor;
    }

    // ── Detección de golpe por abajo (tipo Mario) ─────────────
    void OnTriggerEnter2D(Collider2D other)
    {
        if (_yaGolpeado) return;

        // Verifica que sea el jugador (tag Player)
        if (!other.CompareTag("Player")) return;

        // Verifica que el golpe venga desde abajo
        // La posición Y del Knight debe estar por debajo del bloque
        if (other.transform.position.y < transform.position.y)
            Golpear();
    }

    public void Golpear()
    {
        if (_yaGolpeado) return;
        _yaGolpeado = true;

        if (spriteRenderer != null && spriteGolpeado != null)
            spriteRenderer.sprite = spriteGolpeado;

        StartCoroutine(AnimacionRebote());

      //  if (_manager != null)
        //    _manager.OnBloqueGolpeado(this);
    }

    private IEnumerator AnimacionRebote()
    {
        Vector3 posFinal = _posOriginal + Vector3.up * alturaRebote;
        float t = 0f;

        while (t < duracionRebote)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(_posOriginal, posFinal, t / duracionRebote);
            yield return null;
        }
        t = 0f;
        while (t < duracionRebote)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(posFinal, _posOriginal, t / duracionRebote);
            yield return null;
        }
        transform.position = _posOriginal;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = esCorrecto ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.9f);
    }
}
