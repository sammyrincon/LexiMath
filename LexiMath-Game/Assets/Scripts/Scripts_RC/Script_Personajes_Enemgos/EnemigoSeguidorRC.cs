using UnityEngine;

public class EnemigoSeguidor : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 2f;
    public float distanciaDeAtaque = 1.5f;

    [Header("Configuración de Ataque")]
    public float tiempoEntreAtaques = 1.5f; 
    private float temporizadorAtaque = 0f;

    private Animator animator;
    private Transform jugador;
    private Sistema_Salud_RC salud;

    void Start()
    {
        GameObject objetoJugador = GameObject.FindGameObjectWithTag("Player");
        if (objetoJugador != null)
        {
            jugador = objetoJugador.transform;
        }

        animator = GetComponent<Animator>(); 
        salud = GetComponent<Sistema_Salud_RC>();
    }

    void Update()
    {
        if (salud != null && salud.saludActual <= 0)
        {
            return;
        }

        if (jugador != null)
        {
            temporizadorAtaque += Time.deltaTime;

            Vector2 direccion = jugador.position - transform.position;
            float distancia = direccion.magnitude;

            if (distancia > distanciaDeAtaque)
            {
                direccion.Normalize(); 
                transform.position = (Vector2)transform.position + (direccion * velocidad * Time.deltaTime);
                
                animator.SetFloat("Speed", 1f); 

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
}