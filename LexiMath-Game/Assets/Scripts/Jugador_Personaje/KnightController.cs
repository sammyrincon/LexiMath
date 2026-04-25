using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class KnightController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidadX = 6f;
    [SerializeField] private float velocidadY = 6f;

    [Header("Control en aire")]
    [Range(0f, 1f)]
    [SerializeField] private float controlEnAire = 1f;

    [Header("Input Actions")]
    [SerializeField] private InputAction accionMover;
    [SerializeField] private InputAction accionSalto;
    [SerializeField] private InputAction accionAtacar;

    [Header("Suelo")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float radioSuelo = 0.25f;

    [Header("Ataque")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.8f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 0.35f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Tutorial")]
    [SerializeField] private TutorialManager tutorialManager;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private bool isGrounded;
    private float nextAttackTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        if (tutorialManager == null)
        {
            tutorialManager = FindObjectOfType<TutorialManager>();
        }
    }

    private void OnEnable()
    {
        accionMover.Enable();
        accionSalto.Enable();
        accionAtacar.Enable();

        accionSalto.performed += Saltar;
        accionAtacar.performed += Atacar;
    }

    private void OnDisable()
    {
        accionSalto.performed -= Saltar;
        accionAtacar.performed -= Atacar;

        accionMover.Disable();
        accionSalto.Disable();
        accionAtacar.Disable();
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, radioSuelo) != null;

        Vector2 movimiento = accionMover.ReadValue<Vector2>();
        float factorMovimiento = isGrounded ? 1f : controlEnAire;

        rb.linearVelocity = new Vector2(movimiento.x * velocidadX * factorMovimiento, rb.linearVelocity.y);

        if (Mathf.Abs(movimiento.x) > 0.01f)
        {
            tutorialManager?.OnPlayerMove();
        }

        if (movimiento.x > 0f)
        {
            sprite.flipX = false;
        }
        else if (movimiento.x < 0f)
        {
            sprite.flipX = true;
        }

        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("Speed", Mathf.Abs(movimiento.x));
    }

    private void Saltar(InputAction.CallbackContext context)
    {
        if (!isGrounded)
        {
            return;
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, velocidadY);
        anim.SetTrigger("Jump");
        tutorialManager?.OnPlayerJump();
    }

    private void Atacar(InputAction.CallbackContext context)
    {
        if (!isGrounded)
        {
            return;
        }

        if (Time.time < nextAttackTime)
        {
            return;
        }

        anim.SetTrigger("Attack");
        PerformAttack();
        nextAttackTime = Time.time + attackCooldown;
        tutorialManager?.OnPlayerAttack();
    }

    private void PerformAttack()
    {
        Vector3 origin = attackPoint != null ? attackPoint.position : transform.position;
        Collider2D[] hits;

        if (enemyLayer.value == 0)
        {
            hits = Physics2D.OverlapCircleAll(origin, attackRange);
        }
        else
        {
            hits = Physics2D.OverlapCircleAll(origin, attackRange, enemyLayer);
        }

        foreach (Collider2D hit in hits)
        {
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                continue;
            }

            IDamageable damageable =
                hit.GetComponent<IDamageable>() ??
                hit.GetComponentInParent<IDamageable>() ??
                hit.GetComponentInChildren<IDamageable>();

            if (damageable == null)
            {
                continue;
            }

            Vector2 hitDirection = (hit.transform.position - transform.position).normalized;
            damageable.TakeDamage(attackDamage, origin, hitDirection);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, radioSuelo);
        }

        Gizmos.color = Color.red;
        Vector3 origin = attackPoint != null ? attackPoint.position : transform.position;
        Gizmos.DrawWireSphere(origin, attackRange);
    }
}
