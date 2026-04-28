using UnityEngine;

public class EnemigoSeguidor : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 2f;
    public float distanciaDeAtaque = 1.5f;

    [Header("Configuración de Ataque")]
    public int danoDeAtaque = 20;
    
    // NUEVO: Tiempo que debe esperar el enemigo entre cada golpe
    public float tiempoEntreAtaques = 1.5f; 
    private float temporizadorAtaque = 0f;

    [Header("Configuración de Obstáculos")]
    public LayerMask wallLayer;

    private Animator animator;
    private Transform jugador;

    void Start()
    {
        GameObject objetoJugador = GameObject.FindGameObjectWithTag("Player");
        if (objetoJugador != null)
        {
            jugador = objetoJugador.transform;
        }

        animator = GetComponent<Animator>(); 
    }

    void Update()
    {
        if (jugador != null)
        {
            temporizadorAtaque += Time.deltaTime;

            Vector2 direccion = jugador.position - transform.position;
            float distancia = direccion.magnitude;

            if (distancia > distanciaDeAtaque)
            {
                direccion.Normalize();
                if (Physics2D.Raycast(transform.position, direccion, velocidad * Time.deltaTime + 0.1f, wallLayer).collider == null)
                {
                    transform.position = (Vector2)transform.position + (direccion * velocidad * Time.deltaTime);
                    animator.SetFloat("Speed", 1f);
                }
                else
                {
                    animator.SetFloat("Speed", 0f);
                }
                
                if (direccion.x < 0)
                {
                    transform.eulerAngles = new Vector3(0, 180, 0);
                }
                else if (direccion.x > 0)
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                }
            }
            else
            {
                animator.SetFloat("Speed", 0f); 

                if (temporizadorAtaque >= tiempoEntreAtaques)
                {
                    animator.SetTrigger("Attack");
                    temporizadorAtaque = 0f; 
                }
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Sistema_Salud_RC saludJugador = collision.gameObject.GetComponent<Sistema_Salud_RC>();

            if (saludJugador != null)
            {
                saludJugador.RecibirDano(danoDeAtaque);
            }
        }
    }
}