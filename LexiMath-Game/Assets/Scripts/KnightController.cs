using UnityEngine;

/// <summary>
/// KnightController — LexiMath (estilo simple de clase)
/// 
/// CONTROLES:
///   Mover   → A / D  ó  ← / →
///   Saltar  → Espacio
///   Atacar  → X
/// 
/// SETUP EN UNITY:
///   Knight debe tener:
///     • Rigidbody2D
///     • CapsuleCollider2D  (Is Trigger = false — cuerpo)
///     • BoxCollider2D      (Is Trigger = true  — pies, detecta suelo)
///     • Animator
///     • SpriteRenderer
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class KnightController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad   = 5f;
    public float fuerzaSalto = 12f;

    [Header("Ataque")]
    public float cooldownAtaque = 0.5f;

    // ── Estado ────────────────────────────────────────────────
    public bool estaEnPiso { get; private set; } = false;

    // ── Referencias ───────────────────────────────────────────
    private Rigidbody2D    _rb;
    private Animator       _anim;
    private SpriteRenderer _sprite;

    // ── Privados ──────────────────────────────────────────────
    private float _inputH;
    private bool  _miraDerecha           = true;
    private float _tiempoSiguienteAtaque = 0f;

    void Awake()
    {
        _rb     = GetComponent<Rigidbody2D>();
        _anim   = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        _inputH = Input.GetAxisRaw("Horizontal");

        // Salto
        if (Input.GetKeyDown(KeyCode.Space) && estaEnPiso)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, fuerzaSalto);
            _anim.SetTrigger("Jump");
        }

        // Ataque
        if (Input.GetKeyDown(KeyCode.X) && Time.time >= _tiempoSiguienteAtaque)
        {
            _anim.SetTrigger("Attack");
            _tiempoSiguienteAtaque = Time.time + cooldownAtaque;
        }

        // Voltear sprite
        if (_inputH > 0 && !_miraDerecha) Voltear();
        if (_inputH < 0 &&  _miraDerecha) Voltear();

        // Animator params
        _anim.SetFloat("Speed", Mathf.Abs(_inputH));
        _anim.SetBool("IsGrounded", estaEnPiso);
    }

    void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2(_inputH * velocidad, _rb.linearVelocity.y);
    }

    // ── Detección de suelo (como en clase) ────────────────────
    void OnTriggerEnter2D(Collider2D collision)
    {
        estaEnPiso = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        estaEnPiso = false;
    }

    private void Voltear()
    {
        _miraDerecha = !_miraDerecha;
        _sprite.flipX = !_miraDerecha;
    }
}
