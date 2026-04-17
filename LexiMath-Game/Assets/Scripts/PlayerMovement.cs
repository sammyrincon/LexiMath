using System.Collections;
using UnityEngine;

/// <summary>
/// PlayerMovement — LexiMath
/// Maneja ÚNICAMENTE el movimiento del personaje:
///   • Caminar   → A / D  ó  ← / →
///   • Saltar    → Espacio
///   • Dash      → Shift
///
/// El ataque va en un script separado (PlayerAttack.cs)
///
/// SETUP EN UNITY (paso a paso):
///   1. Selecciona el GameObject del personaje en la escena
///   2. Añade componente Rigidbody2D:
///        - Gravity Scale: 3
///        - Collision Detection: Continuous
///        - Freeze Rotation Z: ✓ (palomita)
///   3. Añade componente CapsuleCollider2D (ajusta al cuerpo del sprite)
///   4. Añade ESTE script (PlayerMovement)
///   5. Crea un GameObject hijo llamado "GroundCheck":
///        - Ponlo justo debajo de los pies del personaje
///        - Arrastralo al campo "groundCheck" en el Inspector
///   6. En el Inspector asigna:
///        - groundCheck → el hijo "GroundCheck"
///        - groundLayer → selecciona la capa de tus plataformas
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Velocidad")]
    public float velocidad   = 6f;
    public float fuerzaSalto = 14f;

    [Header("Dash")]
    public float fuerzaDash    = 18f;
    public float duracionDash  = 0.18f;
    public float cooldownDash  = 0.8f;

    [Header("Detección de suelo")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float     groundRadius = 0.15f;

    // ── Privados ─────────────────────────────────────────────
    private Rigidbody2D _rb;
    private bool  _enSuelo;
    private bool  _miraDerecha  = true;

    private bool  _saltoPedido  = false;
    private bool  _enDash       = false;
    private bool  _dashListo    = true;

    // ── Control externo (lo usa TutorialManager) ─────────────
    [HideInInspector] public bool puedeMoverse = true;
    [HideInInspector] public bool puedeSaltar  = true;

    // ══════════════════════════════════════════════════════════
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        VerificarSuelo();

        // Capturar salto en Update para no perder el frame del input
        if (Input.GetButtonDown("Jump") && puedeSaltar && _enSuelo)
            _saltoPedido = true;

        // Dash
        if ((Input.GetKeyDown(KeyCode.LeftShift) ||
             Input.GetKeyDown(KeyCode.RightShift))
             && puedeMoverse && _dashListo && !_enDash)
            StartCoroutine(HacerDash());

        // Voltear sprite
        float inputH = Input.GetAxisRaw("Horizontal");
        if (inputH > 0 && !_miraDerecha) Voltear();
        if (inputH < 0 &&  _miraDerecha) Voltear();
    }

    void FixedUpdate()
    {
        if (_enDash) return; // durante el dash no se aplica movimiento normal

        // Movimiento horizontal
        if (puedeMoverse)
        {
            float inputH = Input.GetAxisRaw("Horizontal");
            _rb.linearVelocity = new Vector2(inputH * velocidad, _rb.linearVelocity.y);
        }

        // Salto
        if (_saltoPedido)
        {
            _saltoPedido = false;
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, fuerzaSalto);
        }

        // Gravedad extra al caer (salto más "pesado", menos flotante)
        if (_rb.linearVelocity.y < 0)
            _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * 2f * Time.fixedDeltaTime;
    }

    // ── Detección de suelo ────────────────────────────────────
    private void VerificarSuelo()
    {
        if (groundCheck == null) return;
        _enSuelo = Physics2D.OverlapCircle(
            groundCheck.position, groundRadius, groundLayer);
    }

    // ── Voltear personaje ─────────────────────────────────────
    private void Voltear()
    {
        _miraDerecha = !_miraDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    // ── Dash ──────────────────────────────────────────────────
    private IEnumerator HacerDash()
    {
        _enDash  = true;
        _dashListo = false;

        float dir = _miraDerecha ? 1f : -1f;
        float gravOriginal = _rb.gravityScale;

        _rb.gravityScale   = 0f;
        _rb.linearVelocity = new Vector2(fuerzaDash * dir, 0f);

        yield return new WaitForSeconds(duracionDash);

        _rb.gravityScale = gravOriginal;
        _enDash = false;

        yield return new WaitForSeconds(cooldownDash);
        _dashListo = true;
    }

    // ── Propiedades públicas (para saber el estado desde otros scripts) ──
    public bool EstaEnSuelo  => _enSuelo;
    public bool EstaEnDash   => _enDash;
    public float VelocidadH  => _rb.linearVelocity.x;
    public float VelocidadV  => _rb.linearVelocity.y;

    // ── Gizmo para ver el groundCheck en Scene view ───────────
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}
