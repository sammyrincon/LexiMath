using UnityEngine;

public class EnemigoAzulIA : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra al jugador aquí desde la jerarquía. Si está vacío, lo buscará por el Tag 'Player'.")]
    public Transform jugador;
    private Animator animator;

    [Header("Estadísticas de Combate")]
    public float velocidad = 3f;
    public float distanciaPersecucion = 8f;
    public float distanciaAtaque = 1.5f;
    public float tiempoEntreAtaques = 2f;

    // Variables internas
    private float tiempoUltimoAtaque = 0f;
    private bool estaMuerto = false;
    private bool mirandoDerecha = true; // Asume que tu sprite base mira hacia la derecha

    void Start()
    {
        // Caché del componente para optimizar rendimiento
        animator = GetComponent<Animator>();

        // Auto-búsqueda del jugador si olvidaste asignarlo en el Inspector
        if (jugador == null)
        {
            GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
            if (objJugador != null)
            {
                jugador = objJugador.transform;
            }
            else
            {
                Debug.LogWarning("¡No se encontró al jugador! Asegúrate de que tenga el Tag 'Player'.");
            }
        }
    }

    void Update()
    {
        // 1. Cláusula de guardia: Si el enemigo murió o no hay jugador, detener la lógica.
        if (estaMuerto || jugador == null) return;

        // 2. Calcular la distancia entre el enemigo y el jugador
        float distanciaAlJugador = Vector2.Distance(transform.position, jugador.position);

        // 3. Evaluar de qué lado está el jugador para voltear el sprite
        MirarAlJugador();

        // 4. Lógica de Toma de Decisiones (Máquina de Estados Simple)
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
            // Fuera de rango: detenerse
            animator.SetBool("isWalking", false);
        }
    }

    private void MirarAlJugador()
    {
        // Si el jugador está a la derecha de mí, y estoy mirando a la izquierda...
        if (jugador.position.x > transform.position.x && !mirandoDerecha)
        {
            Girar();
        }
        // Si el jugador está a la izquierda de mí, y estoy mirando a la derecha...
        else if (jugador.position.x < transform.position.x && mirandoDerecha)
        {
            Girar();
        }
    }

    private void Girar()
    {
        // Cambia el estado de la variable booleana
        mirandoDerecha = !mirandoDerecha;

        // Multiplicar la escala X por -1 voltea el objeto completo como un espejo
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void Perseguir()
    {
        animator.SetBool("isWalking", true);

        // Mueve al enemigo solo en el eje X hacia el jugador (ideal para plataformas 2D)
        // Mantiene la posición 'Y' original para que no empiece a volar.
        Vector2 objetivo = new Vector2(jugador.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, objetivo, velocidad * Time.deltaTime);
    }

    private void Atacar()
    {
        // Se detiene para hacer la animación de ataque
        animator.SetBool("isWalking", false);

        // Control de enfriamiento (Cooldown) usando Time.time
        if (Time.time >= tiempoUltimoAtaque + tiempoEntreAtaques)
        {
            animator.SetTrigger("Attack");
            tiempoUltimoAtaque = Time.time; // Reiniciar el temporizador
        }
    }

    // Llama a esta función desde tu script de Salud o Daño cuando la vida llegue a 0
    public void Morir()
    {
        estaMuerto = true;
        animator.SetTrigger("Die");
        
        // Optimización: Desactiva el colisionador para que el jugador no choque con un cadáver
        Collider2D collider = GetComponent<Collider2D>();
        if(collider != null) collider.enabled = false;
    }
}