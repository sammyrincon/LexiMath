using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class KnightController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float velocidadX = 6f;
    [SerializeField] private float velocidadY = 14f;

    [Header("Control en aire (para escaleras/plataformas)")]
    [Range(0f, 1f)]
    [SerializeField] private float controlEnAire = 1f; // 1 = control completo en el aire

    [Header("Input Actions")]
    [SerializeField] private InputAction accionMover;
    [SerializeField] private InputAction accionSalto;
    [SerializeField] private InputAction accionAtacar;

    [Header("Detección de Suelo")]
    [SerializeField] private LayerMask capaSuelo;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;

    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        accionMover.Enable();
        accionSalto.Enable();
        accionAtacar.Enable();

        accionSalto.performed += Saltar;
        accionAtacar.performed += Atacar;
    }

    void OnDisable()
    {
        accionSalto.performed -= Saltar;
        accionAtacar.performed -= Atacar;

        accionMover.Disable();
        accionSalto.Disable();
        accionAtacar.Disable();
    }

    void Update()
    {
        Vector2 movimiento = accionMover.ReadValue<Vector2>();

        // ✅ Movimiento horizontal SIEMPRE (suelo + aire)
        float factor = isGrounded ? 1f : controlEnAire;
        rb.linearVelocity = new Vector2(movimiento.x * velocidadX * factor, rb.linearVelocity.y);

        // ✅ Flip SIEMPRE (para corregir en el aire)
        if (movimiento.x > 0) sprite.flipX = false;
        else if (movimiento.x < 0) sprite.flipX = true;

        // ✅ Animator: actualiza ambos SIEMPRE
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("Speed", Mathf.Abs(movimiento.x)); // <- clave: no lo apagues en el aire
    }

    private void Saltar(InputAction.CallbackContext context)
    {
        if (!isGrounded) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, velocidadY);
        anim.SetTrigger("Jump");
        isGrounded = false;
    }

    private void Atacar(InputAction.CallbackContext context)
    {
        if (isGrounded)
            anim.SetTrigger("Attack");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & capaSuelo) != 0)
            isGrounded = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & capaSuelo) != 0)
            isGrounded = false;
    }
}
