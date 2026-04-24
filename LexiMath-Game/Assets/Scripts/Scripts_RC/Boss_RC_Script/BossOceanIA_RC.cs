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
        // 1. Buscar al Jugador
        GameObject objetoJugador = GameObject.FindGameObjectWithTag("Player");
        if (objetoJugador != null)
        {
            jugador = objetoJugador.transform;
        }

        // 2. Enlazar componentes del cuerpo
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        miSalud = GetComponent<Sistema_Salud_RC>(); 

        // 3. AUTOMATIZACIÓN DE ARMAS (Con tus nombres exactos)
        ArmaEnemigoRC[] todasLasArmas = GetComponentsInChildren<ArmaEnemigoRC>();
        
        foreach (ArmaEnemigoRC arma in todasLasArmas)
        {
            // Nombres actualizados según tu jerarquía
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

        // ==========================================
        // ÁRBOL DE DECISIONES DE LA IA
        // ==========================================

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

    // ==========================================
    // DAÑO POR CONTACTO FÍSICO
    // ==========================================
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

    // ==========================================
    // PUENTE PARA LOS EVENTOS DE ANIMACIÓN
    // ==========================================
    
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
}