using UnityEngine;
using UnityEngine.InputSystem;

public class ControladorJugadorRC : MonoBehaviour
{
    [Header("Controles")]
    [SerializeField] private InputAction accionMover;
    [SerializeField] private InputAction accionSalto;
    [SerializeField] private InputAction accionAtaque;
    [SerializeField] private InputAction accionDisparar;

    [Header("Configuracion de Movimiento y Combate")]
    public float velocidad = 5f;
    public float fuerzaDeSalto = 10f;
    [SerializeField] private GameObject prefabProyectil;
    [SerializeField] private Vector3 ajusteDisparo = new Vector3(0f, 1.0f, 0f);

    [Header("Configuracion de Cooldown")]
    public float tiempoEntreDisparos = 0.8f;
    private float tiempoProximoDisparo = 0f;

    [Header("Efectos de Sonido")]
    [SerializeField] private AudioClip sonidoSalto;
    [SerializeField] private AudioClip sonidoAtaque;
    [SerializeField] private AudioClip sonidoPasos;

    private Rigidbody2D cuerpoFisico;
    private Animator animador;
    private SpriteRenderer spriteRenderer;
    private EstadoPersonaje detectorSuelo;
    private Vector2 movimiento;
    private AudioSource reproductorSonido;

    void Awake()
    {
        cuerpoFisico = GetComponent<Rigidbody2D>();
        animador = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        detectorSuelo = GetComponentInChildren<EstadoPersonaje>();
        reproductorSonido = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        accionMover.Enable();
        accionSalto.Enable();
        accionAtaque.Enable();
        accionDisparar.Enable();

        accionSalto.performed += Saltar;
        accionAtaque.performed += Atacar;
        accionDisparar.performed += Disparar;
    }

    private void OnDisable()
    {
        accionMover.Disable();
        accionSalto.Disable();
        accionAtaque.Disable();
        accionDisparar.Disable();

        accionSalto.performed -= Saltar;
        accionAtaque.performed -= Atacar;
        accionDisparar.performed -= Disparar;
    }

    private void Disparar(InputAction.CallbackContext context)
    {
        if (Time.time >= tiempoProximoDisparo)
        {
            if (prefabProyectil != null)
            {
                GameObject nuevoProyectil = Instantiate(prefabProyectil);
                nuevoProyectil.transform.position = transform.position + ajusteDisparo;

                if (spriteRenderer.flipX == true)
                {
                    Proyectil_RC scriptProyectil = nuevoProyectil.GetComponent<Proyectil_RC>();
                    if (scriptProyectil != null)
                    {
                        scriptProyectil.velocidadX = -Mathf.Abs(scriptProyectil.velocidadX);
                    }
                }

                tiempoProximoDisparo = Time.time + tiempoEntreDisparos;
            }
        }
    }

    private void Saltar(InputAction.CallbackContext context)
    {
        if (detectorSuelo != null && detectorSuelo.estaEnPiso)
        {
            cuerpoFisico.linearVelocityY = fuerzaDeSalto;
            if (animador != null) animador.SetTrigger("Jump");

            if (reproductorSonido != null && sonidoSalto != null)
            {
                reproductorSonido.PlayOneShot(sonidoSalto);
            }
        }
    }

    private void Atacar(InputAction.CallbackContext context)
    {
        if (animador != null) animador.SetTrigger("Attack");

        if (reproductorSonido != null && sonidoAtaque != null)
        {
            reproductorSonido.PlayOneShot(sonidoAtaque);
        }
    }

    void Update()
    {
        movimiento = accionMover.ReadValue<Vector2>();

        if (animador != null)
        {
            animador.SetFloat("Speed", Mathf.Abs(movimiento.x));
            if (detectorSuelo != null) animador.SetBool("isGrounded", detectorSuelo.estaEnPiso);
        }

        if (movimiento.x < 0) spriteRenderer.flipX = true;
        else if (movimiento.x > 0) spriteRenderer.flipX = false;

        ManejarSonidoPasos();
    }

    private void ManejarSonidoPasos()
    {
        if (reproductorSonido == null || sonidoPasos == null) return;

        if (Mathf.Abs(movimiento.x) > 0.1f && detectorSuelo != null && detectorSuelo.estaEnPiso)
        {
            if (reproductorSonido.clip != sonidoPasos || !reproductorSonido.isPlaying)
            {
                reproductorSonido.clip = sonidoPasos;
                reproductorSonido.loop = true;
                reproductorSonido.Play();
            }
        }
        else
        {
            if (reproductorSonido.clip == sonidoPasos && reproductorSonido.isPlaying)
            {
                reproductorSonido.Stop();
            }
        }
    }

    void FixedUpdate()
    {
        cuerpoFisico.linearVelocityX = movimiento.x * velocidad;
    }
}  