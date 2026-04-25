using UnityEngine;

public class PandaBossIA : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;
    private Animator animator;
    private Collider2D colisionador;

    [Header("Estadísticas de Combate")]
    public int vidaMaxima = 100;
    private int vidaActual;
    public float velocidad = 3.5f;
    public float velocidadEscape = 6f; 
    public float distanciaPersecucion = 10f;
    public float distanciaAtaque = 2f;
    public float tiempoEntreAtaques = 1.5f;

    private float tiempoUltimoAtaque = 0f;
    private bool mirandoDerecha = true;
    private bool estaHuyendo = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        colisionador = GetComponent<Collider2D>();
        vidaActual = vidaMaxima;

        if (jugador == null)
        {
            GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
            if (objJugador != null) jugador = objJugador.transform;
        }
    }

    void Update()
    {
        if (jugador == null) return;

        if (estaHuyendo)
        {
            EjecutarEscape();
            return; 
        }

        float distanciaAlJugador = Vector2.Distance(transform.position, jugador.position);

        MirarAlObjetivo(jugador.position.x);

        if (distanciaAlJugador <= distanciaAtaque)
        {
            Atacar();
        }
        else if (distanciaAlJugador <= distanciaPersecucion)
        {
            Perseguir();
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }

    private void MirarAlObjetivo(float posicionXObjetivo)
    {
        if (posicionXObjetivo > transform.position.x && !mirandoDerecha)
        {
            Girar();
        }
        else if (posicionXObjetivo < transform.position.x && mirandoDerecha)
        {
            Girar();
        }
    }

    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void Perseguir()
    {
        animator.SetBool("isRunning", true);
        Vector2 objetivo = new Vector2(jugador.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, objetivo, velocidad * Time.deltaTime);
    }

    private void Atacar()
    {
        animator.SetBool("isRunning", false);

        if (Time.time >= tiempoUltimoAtaque + tiempoEntreAtaques)
        {
            int ataqueAleatorio = Random.Range(1, 4); 
            animator.SetTrigger("Attack_" + ataqueAleatorio);
            
            tiempoUltimoAtaque = Time.time;
        }
    }

    public void RecibirDano(int cantidad)
    {
        if (estaHuyendo) return; 

        vidaActual -= cantidad;

        if (vidaActual <= 0)
        {
            IniciarEscape();
        }
        else
        {
            animator.SetTrigger("Hurt");
        }
    }

    private void IniciarEscape()
    {
        estaHuyendo = true;
        animator.SetTrigger("Escape");
        
        if (colisionador != null) colisionador.enabled = false;

        // Calcula la posición contraria al jugador para voltearse y correr hacia allá
        float direccionContraria = transform.position.x - (jugador.position.x - transform.position.x);
        MirarAlObjetivo(direccionContraria);
    }

    private void EjecutarEscape()
    {
        int direccion = mirandoDerecha ? 1 : -1;
        transform.Translate(Vector2.right * direccion * velocidadEscape * Time.deltaTime);
        
        Destroy(gameObject,7f);
    }
}