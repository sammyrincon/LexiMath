using UnityEngine;

/// <summary>
/// KnightController — LexiMath (todo-en-uno)
/// 
/// CONTROLES:
///   Mover   → A / D  ó  ← / →
///   Saltar  → Espacio
///   Atacar  → X
/// 
/// SETUP EN UNITY:
///   1. Knight debe tener:
///        • Rigidbody2D
///        • CapsuleCollider2D  (Is Trigger = false, es el cuerpo)
///        • BoxCollider2D      (Is Trigger = true, en los pies)
///        • Animator
///        • SpriteRenderer
///   2. Asigna el BoxCollider de los pies al campo "footCollider"
///   3. Ground Layer = Ground
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

    [Header("Detección de suelo")]
    [Tooltip("Capa del suelo (Ground)")]
    public LayerMask groundLayer;

    [Tooltip("BoxCollider2D en los pies del Knight (Is Trigger = true)")]
    public BoxCollider2D footCollider;

    // ── Referencias ───────────────────────────────────────────
    private Rigidbody2D    _rb;
    private Animator       _anim;
    private SpriteRenderer _sprite;

    // ── Estado ────────────────────────────────────────────────
    private float _inputH;
    private bool  _enSuelo         = false;
    private bool  _miraDerecha     = true;
    private float _tiempoSiguienteAtaque = 0f;

    void Awake()
    {
        _rb     = GetComponent<Rigidbody2D>();
        _anim   = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Detectar suelo usando el collider de los pies
        _enSuelo = DetectarSuelo();

        _inputH = Input.GetAxisRaw("Horizontal");

        // Salto
        if (Input.GetKeyDown(KeyCode.Space) && _enSuelo)
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
        _anim.SetBool("IsGrounded", _enSuelo);
    }

    void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2(_inputH * velocidad, _rb.linearVelocity.y);
    }

    /// <summary>
    /// Detecta el suelo usando Physics2D.OverlapBox con las dimensiones del footCollider.
    /// </summary>
    private bool DetectarSuelo()
    {
        if (footCollider == null) return false;

        Vector2 centro = (Vector2)transform.position 
                       + (Vector2)footCollider.offset * transform.lossyScale;
        Vector2 size   = footCollider.size * Mathf.Abs(transform.lossyScale.x);

        return Physics2D.OverlapBox(centro, size, 0f, groundLayer) != null;
    }

    private void Voltear()
    {
        _miraDerecha = !_miraDerecha;
        _sprite.flipX = !_miraDerecha;
    }

    // ── Debug gizmo ──────────────────────────────────────────
    void OnDrawGizmosSelected()
    {
        if (footCollider == null) return;
        Vector2 centro = (Vector2)transform.position 
                       + (Vector2)footCollider.offset * transform.lossyScale;
        Vector2 size   = footCollider.size * Mathf.Abs(transform.lossyScale.x);
        Gizmos.color = _enSuelo ? Color.green : Color.red;
        Gizmos.DrawWireCube(centro, size);
    }
}
