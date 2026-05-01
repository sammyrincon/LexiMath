using UnityEngine;
using UnityEngine.InputSystem;

public class ControladorJugadorRC : MonoBehaviour
{
    [Header("Controles (Configurar en el Inspector)")]
    [SerializeField] private InputAction accionMover;
    [SerializeField] private InputAction accionSalto;
    [SerializeField] private InputAction accionAtaque;

    [Header("Configuracion de Movimiento")]
    public float velocidad = 5f;
    public float fuerzaDeSalto = 10f;

    private Rigidbody2D cuerpoFisico;
    private Animator animador;
    private SpriteRenderer spriteRenderer; 
    private EstadoPersonaje detectorSuelo;
    private Vector2 movimiento;

    void Awake()
    {
        cuerpoFisico = GetComponent<Rigidbody2D>();
        animador = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        detectorSuelo = GetComponentInChildren<EstadoPersonaje>();
    }

    private void OnEnable()
    {
        accionMover.Enable();
        accionSalto.Enable();
        accionAtaque.Enable();

        accionSalto.performed += Saltar;
        accionAtaque.performed += Atacar;
    }

    private void OnDisable()
    {
        accionMover.Disable();
        accionSalto.Disable();
        accionAtaque.Disable();

        accionSalto.performed -= Saltar;
        accionAtaque.performed -= Atacar;
    }

    private void Saltar(InputAction.CallbackContext context)
    {
        if (detectorSuelo != null)
        {
            if (detectorSuelo.estaEnPiso)
            {
                cuerpoFisico.linearVelocityY = fuerzaDeSalto;
                
                if (animador != null)
                {
                    animador.SetTrigger("Jump");
                }
            }
        }
    }

    private void Atacar(InputAction.CallbackContext context)
    {
        if (animador != null)
        {
            animador.SetTrigger("Attack");
        }
    }

    void Update()
    {
        movimiento = accionMover.ReadValue<Vector2>();

        if (animador != null)
        {
            animador.SetFloat("Speed", Mathf.Abs(movimiento.x));
            
            if (detectorSuelo != null)
            {
                animador.SetBool("isGrounded", detectorSuelo.estaEnPiso);
            }
        }

        // Cambio de dirección usando flipX
        if (movimiento.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (movimiento.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    void FixedUpdate()
    {
        cuerpoFisico.linearVelocityX = movimiento.x * velocidad;
    }
} 