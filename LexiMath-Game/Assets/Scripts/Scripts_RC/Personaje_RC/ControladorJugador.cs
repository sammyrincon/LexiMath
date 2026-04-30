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
    private EstadoPersonaje detectorSuelo;
    private Vector2 movimiento;

    void Awake()
    {
        cuerpoFisico = GetComponent<Rigidbody2D>();
        animador = GetComponent<Animator>();
        detectorSuelo = GetComponentInChildren<EstadoPersonaje>();
    }

    private void OnEnable()
    {
        accionMover.Enable();
        accionSalto.Enable();
        accionAtaque.Enable();
    }

    private void OnDisable()
    {
        accionMover.Disable();
        accionSalto.Disable();
        accionAtaque.Disable();
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

        if (movimiento.x < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (movimiento.x > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

        if (accionSalto.WasPressedThisFrame())
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

        if (accionAtaque.WasPressedThisFrame())
        {
            if (animador != null)
            {
                animador.SetTrigger("Attack");
            }
        }
    }

    void FixedUpdate()
    {
        cuerpoFisico.linearVelocityX = movimiento.x * velocidad;
    }
}  