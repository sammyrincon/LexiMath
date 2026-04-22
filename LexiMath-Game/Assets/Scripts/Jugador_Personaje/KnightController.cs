using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class KnightController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float velocidadX = 6f;
    [SerializeField] private float velocidadY = 6f;

    [Header("Control en aire")]
    [Range(0f, 1f)]
    [SerializeField] private float controlEnAire = 1f;

    [Header("Input Actions")]
    [SerializeField] private InputAction accionMover;
    [SerializeField] private InputAction accionSalto;
    [SerializeField] private InputAction accionAtacar;

    [Header("Detección de Suelo")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float radioSuelo = 0.25f;

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
        Collider2D suelo = Physics2D.OverlapCircle(groundCheck.position, radioSuelo);
        isGrounded = suelo != null;

        if (suelo != null)
        {
            Debug.Log("Detectando suelo: " + suelo.name);
        }

        Vector2 movimiento = accionMover.ReadValue<Vector2>();
        float factor = isGrounded ? 1f : controlEnAire;

        rb.linearVelocity = new Vector2(movimiento.x * velocidadX * factor, rb.linearVelocity.y);

        if (movimiento.x > 0)
            sprite.flipX = false;
        else if (movimiento.x < 0)
            sprite.flipX = true;

        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("Speed", Mathf.Abs(movimiento.x));
    }

    private void Saltar(InputAction.CallbackContext context)
    {
        Debug.Log("Intento de salto. isGrounded = " + isGrounded);

        if (!isGrounded) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, velocidadY);
        anim.SetTrigger("Jump");

        Debug.Log("SALTO aplicado");
    }

    private void Atacar(InputAction.CallbackContext context)
    {
        if (isGrounded)
            anim.SetTrigger("Attack");
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, radioSuelo);
    }
}