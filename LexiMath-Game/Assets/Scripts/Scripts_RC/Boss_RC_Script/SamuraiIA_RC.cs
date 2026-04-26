using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossSamuraiIA_RC : MonoBehaviour
{
    [Header("Configuracion")]
    [SerializeField] private float velocidad = 3f;
    [SerializeField] private float distanciaAtaque = 2.5f;
    [SerializeField] private float cooldownAtaque = 1.2f;
    [SerializeField] private float ventanaCombo = 1.5f;
    [SerializeField] private float velocidadEscape = 6f;
    [SerializeField] private float tiempoParaDesaparecer = 4f;

    [Header("Configuración de Audio")]
    public AudioSource audioSource;
    public AudioClip sonidoCaminar;
    public AudioClip sonidoAtaque;
    public AudioClip sonidoDolor;
    public AudioClip sonidoEscape;

    private int pasoCombo = 0;
    private float temporizadorAtaque = 0f;
    private float tiempoUltimoGolpe = 0f;
    private bool estaEscapando = false;
    private Vector2 direccionEscape;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform jugador;
    private Sistema_Salud_RC miSalud;
    private ArmaEnemigoRC hitboxAtaque;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        miSalud = GetComponent<Sistema_Salud_RC>();

        rb.freezeRotation = true;

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
        if (objJugador != null) jugador = objJugador.transform;

        ArmaEnemigoRC[] armas = GetComponentsInChildren<ArmaEnemigoRC>();
        foreach (ArmaEnemigoRC arma in armas)
        {
            if (arma.gameObject.name == "Hitbox_Attack") hitboxAtaque = arma;
        }
    }

    void Update()
    {
        if (jugador == null || miSalud == null) return;

        if (miSalud.saludActual <= 0 && !estaEscapando)
        {
            EmpezarEscape();
            return;
        }

        if (estaEscapando)
        {
            rb.linearVelocity = new Vector2(direccionEscape.x * velocidadEscape, rb.linearVelocity.y);
            return;
        }

        temporizadorAtaque += Time.deltaTime;

        if (pasoCombo > 0 && Time.time - tiempoUltimoGolpe > ventanaCombo)
        {
            pasoCombo = 0;
            animator.SetInteger("ComboStep", 0);
        }

        float distancia = Vector2.Distance(transform.position, jugador.position);
        float dirX = jugador.position.x - transform.position.x;

        if (dirX < 0) spriteRenderer.flipX = true;
        else if (dirX > 0) spriteRenderer.flipX = false;

        if (distancia <= distanciaAtaque)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetFloat("Speed", 0f);

            if (temporizadorAtaque >= cooldownAtaque)
            {
                ManejarCombo();
            }
        }
        else
        {
            float movimientoX = (dirX > 0 ? 1 : -1) * velocidad;
            rb.linearVelocity = new Vector2(movimientoX, rb.linearVelocity.y);
            animator.SetFloat("Speed", 1f);
        }
    }

    private void ManejarCombo()
    {
        pasoCombo++;
        if (pasoCombo > 3) pasoCombo = 1;

        tiempoUltimoGolpe = Time.time;
        temporizadorAtaque = 0f;

        animator.SetInteger("ComboStep", pasoCombo);
        animator.SetTrigger("Attack");
    }

    private void EmpezarEscape()
    {
        estaEscapando = true;
        animator.SetTrigger("Death");
        
        ReproducirSonidoEscape(); 

        float ladoEscape = (transform.position.x > jugador.position.x) ? 1f : -1f;
        direccionEscape = new Vector2(ladoEscape, 0);
        spriteRenderer.flipX = (ladoEscape < 0);

        if (hitboxAtaque != null) hitboxAtaque.gameObject.SetActive(false);

        Destroy(gameObject, tiempoParaDesaparecer);
    }

    public void PrenderAtaque()
    {
        if (hitboxAtaque != null) hitboxAtaque.ActivarHitbox();
    }

    public void ApagarAtaque()
    {
        if (hitboxAtaque != null) hitboxAtaque.ApagarHitbox();
    }

    public void ReproducirSonidoAtaque() 
    {
        ReproducirSonido(sonidoAtaque);
    }

    public void ReproducirPasoCaminar()
    {
        ReproducirSonido(sonidoCaminar);
    }

    public void ReproducirSonidoDolor()
    {
        ReproducirSonido(sonidoDolor);
    }

    public void ReproducirSonidoEscape()
    {
        ReproducirSonido(sonidoEscape);
    }

    private void ReproducirSonido(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}