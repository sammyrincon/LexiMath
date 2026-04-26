using UnityEngine;

public class BossOceanIA : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 2.5f;
    
    [Tooltip("Distancia para dar el golpe cuerpo a cuerpo")]
    public float distanciaAtaqueNormal = 2f;
    
    [Tooltip("Distancia máxima para lanzar magia")]
    public float distanciaMagia = 6f;

    [Header("Tiempos de Recarga (Cooldowns)")]
    public float cooldownAtaque = 2f;
    public float cooldownMagia = 4f;
    public float cooldownCura = 15f;

    private float temporizadorAtaque = 0f;
    private float temporizadorMagia = 0f;
    private float temporizadorCura = 0f;

    [Header("Configuración Especial")]
    public int danoPorTocar = 15;
    public int cantidadACurar = 50;
    public int limiteSaludParaCurar = 100; 

    [Header("Configuración de Audio")]
    public AudioSource audioSource;
    public AudioClip sonidoCaminar;
    public AudioClip sonidoAtaqueNormal;
    public AudioClip sonidoMagia;
    public AudioClip sonidoCura;
    public AudioClip sonidoDolor;
    public AudioClip sonidoMuerte;

    // Referencias a los componentes (Automáticas)
    private Animator animator;
    private Transform jugador;
    private SpriteRenderer spriteRenderer;
    private Sistema_Salud_RC miSalud; 
    
    // Variables privadas para los Hitboxes
    private ArmaEnemigoRC hitboxAtaqueNormal;
    private ArmaEnemigoRC hitboxMagia;

    void Start()
    {
        GameObject objetoJugador = GameObject.FindGameObjectWithTag("Player");
        if (objetoJugador != null)
        {
            jugador = objetoJugador.transform;
        }

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        miSalud = GetComponent<Sistema_Salud_RC>(); 
        
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        ArmaEnemigoRC[] todasLasArmas = GetComponentsInChildren<ArmaEnemigoRC>();
        
        foreach (ArmaEnemigoRC arma in todasLasArmas)
        {
            if (arma.gameObject.name == "Hitbox_Attack")
            {
                hitboxAtaqueNormal = arma;
            }
            else if (arma.gameObject.name == "Hitbox_Magic")
            {
                hitboxMagia = arma;
            }
        }
    }

    void Update()
    {
        if (jugador == null || miSalud == null || miSalud.saludActual <= 0) return;

        temporizadorAtaque += Time.deltaTime;
        temporizadorMagia += Time.deltaTime;
        temporizadorCura += Time.deltaTime;

        float distancia = Vector2.Distance(transform.position, jugador.position);
        Vector2 direccion = jugador.position - transform.position;

        if (direccion.x < 0) spriteRenderer.flipX = true;
        else if (direccion.x > 0) spriteRenderer.flipX = false;

        if (miSalud.saludActual <= limiteSaludParaCurar && temporizadorCura >= cooldownCura)
        {
            DetenerMovimiento();
            animator.SetTrigger("Heal"); 
            miSalud.Curar(cantidadACurar); 
            temporizadorCura = 0f;
        }
        else if (distancia <= distanciaAtaqueNormal && temporizadorAtaque >= cooldownAtaque)
        {
            DetenerMovimiento();
            animator.SetTrigger("Attack"); 
            temporizadorAtaque = 0f;
        }
        else if (distancia <= distanciaMagia && distancia > distanciaAtaqueNormal && temporizadorMagia >= cooldownMagia)
        {
            DetenerMovimiento();
            animator.SetTrigger("Magic"); 
            temporizadorMagia = 0f;
        }
        else if (distancia > distanciaAtaqueNormal)
        {
            direccion.Normalize();
            transform.position = (Vector2)transform.position + (direccion * velocidad * Time.deltaTime);
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            DetenerMovimiento(); 
        }
    }

    private void DetenerMovimiento()
    {
        animator.SetFloat("Speed", 0f);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Sistema_Salud_RC saludJugador = collision.gameObject.GetComponent<Sistema_Salud_RC>();
            if (saludJugador != null)
            {
                saludJugador.RecibirDano(danoPorTocar);
            }
        }
    }

    // --- EVENTOS PARA ACTIVAR/DESACTIVAR HITBOXES (Ya no tienen audio) ---

    public void PrenderAtaqueNormal()
    {
        if (hitboxAtaqueNormal != null) hitboxAtaqueNormal.ActivarHitbox();
    }

    public void ApagarAtaqueNormal()
    {
        if (hitboxAtaqueNormal != null) hitboxAtaqueNormal.ApagarHitbox();
    }

    public void PrenderAtaqueMagico()
    {
        if (hitboxMagia != null) hitboxMagia.ActivarHitbox();
    }

    public void ApagarAtaqueMagico()
    {
        if (hitboxMagia != null) hitboxMagia.ApagarHitbox();
    }

    public void ReproducirSonidoAtaqueNormal() 
    {
        ReproducirSonido(sonidoAtaqueNormal);
    }

    public void ReproducirSonidoAtaqueMagico() 
    {
        ReproducirSonido(sonidoMagia);
    }

    public void ReproducirPasoCaminar()
    {
        ReproducirSonido(sonidoCaminar);
    }

    public void ReproducirSonidoCura() 
    {
        ReproducirSonido(sonidoCura);
    }

    public void ReproducirSonidoDolor()
    {
        ReproducirSonido(sonidoDolor);
    }

    public void ReproducirSonidoMuerte()
    {
        ReproducirSonido(sonidoMuerte);
    }

    private void ReproducirSonido(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}