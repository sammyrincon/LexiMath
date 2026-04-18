using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 2.5f;
    public float distanciaDeteccion = 8f;
    public float distanciaAtaque = 1.5f;

    [Header("Referencias")]
    private Transform jugador;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // Busca al jugador por el Tag "Player" (Asegúrate que Mael tenga este Tag)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) jugador = playerObj.transform;
    }

    void Update()
    {
        if (jugador == null) return;

        float distanciaAlJugador = Vector2.Distance(transform.position, jugador.position);

        // 1. LÓGICA DE ESTADOS
        if (distanciaAlJugador <= distanciaAtaque)
        {
            Atacar();
        }
        else if (distanciaAlJugador <= distanciaDeteccion)
        {
            Perseguir();
        }
        else
        {
            QuedarseQuieto();
        }
    }

    void Perseguir()
    {
        // Activar animación de caminar (Asegúrate de tener un Float "Speed" en el Animator)
        anim.SetFloat("Speed", 1f);

        // Calcular dirección
        Vector2 direccion = (jugador.position - transform.position).normalized;
        
        // Mover usando Rigidbody para que respete colisiones
        rb.linearVelocity = new Vector2(direccion.x * velocidad, rb.linearVelocity.y);

        // 2. VOLTEAR (FLIP) - Mirar siempre al jugador
        if (direccion.x > 0)
            spriteRenderer.flipX = false; // Derecha
        else if (direccion.x < 0)
            spriteRenderer.flipX = true;  // Izquierda
    }

    void Atacar()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Frenar para atacar
        anim.SetFloat("Speed", 0f);
        anim.SetTrigger("Attack"); // Dispara el trigger de ataque que pusimos en el autómata
    }

    void QuedarseQuieto()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        anim.SetFloat("Speed", 0f);
    }
}