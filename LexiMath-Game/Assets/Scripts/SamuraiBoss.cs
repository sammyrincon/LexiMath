using UnityEngine;

public class SamuraiBoss : MonoBehaviour
{
    [Header("Estadísticas de Vida")]
    public int vidaMax = 5;
    private int vidaActual;
    private bool estaHuyendo = false;

    [Header("Configuración de IA")]
    public float velocidadSeguimiento = 2.5f;
    public float velocidadHuida = 6f;
    public float distanciaDeteccion = 10f;
    public float distanciaAtaque = 1.5f;
    public float cooldownAtaque = 2f;
    private float tiempoUltimoAtaque;

    [Header("Referencias")]
    public Transform jugador;
    public string tagAtaqueJugador = "AtaqueJugador"; 

    private Rigidbody2D rb;
    private Animator anim;

    void Start()
    {
        vidaActual = vidaMax;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
        // Busca al player automáticamente 
        if (jugador == null)
        {
            GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
            if (objJugador != null) jugador = objJugador.transform;
        }
    }

    void Update()
    {
        // Si ya perdió, solo ejecuta la huida
        if (estaHuyendo)
        {
            HuirFueraDePantalla();
            return; 
        }

        // 2. Si no hay jugador en la escena, no hace nada
        if (jugador == null) return;

        // 3. Lógica principal de la Máquina de Estados
        float distanciaAlJugador = Vector2.Distance(transform.position, jugador.position);

        if (distanciaAlJugador <= distanciaAtaque)
        {
            IntentarAtacar();
        }
        else if (distanciaAlJugador <= distanciaDeteccion)
        {
            PerseguirJugador();
        }
        else
        {
            Detenerse();
        }
    }



    void PerseguirJugador()
    {
        anim.SetBool("IsRunning", true);
        
        // Decide hacia dónde caminar
        float direccion = jugador.position.x > transform.position.x ? 1 : -1;
        
        // linearVelocity es la forma correcta y optimizada en Unity 6
        rb.linearVelocity = new Vector2(direccion * velocidadSeguimiento, rb.linearVelocity.y);
        
        // Voltea el sprite usando rotación (la mejor práctica para los colliders)
        if (direccion > 0)
            transform.rotation = Quaternion.Euler(0, 0, 0); // Derecha
        else
            transform.rotation = Quaternion.Euler(0, 180, 0); // Izquierda
    }

    void IntentarAtacar()
    {
        Detenerse(); // Frena para no patinar mientras golpea
        
        if (Time.time >= tiempoUltimoAtaque + cooldownAtaque)
        {
            // Lanza un número aleatorio entre 1 y 3 para variar el ataque
            int numAtaque = Random.Range(1, 4); 
            anim.SetTrigger("Attack" + numAtaque);
            
            tiempoUltimoAtaque = Time.time;
        }
    }

    void Detenerse()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        anim.SetBool("IsRunning", false);
    }

    // --- SISTEMA DE DAÑO Y DERROTA ---

    private void OnTriggerEnter2D(Collider2D colision)
    {
        // Detecta el golpe del jugador basándose en el Tag
        if (colision.CompareTag(tagAtaqueJugador) && !estaHuyendo)
        {
            RecibirDano(1);
            Destroy(colision.gameObject); // Destruye el proyectil tras el impacto
        }
    }

    public void RecibirDano(int dano)
    {
        vidaActual -= dano;

        if (vidaActual > 0)
        {
            anim.SetTrigger("Hurt");
            tiempoUltimoAtaque = Time.time + 0.5f; 
        }
        else
        {
   
            estaHuyendo = true;
            anim.SetTrigger("Flee");
            

            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
            
            Destroy(gameObject, 6f); 
        }
    }

    void HuirFueraDePantalla()
    {
        // Obliga al Samurái a mirar hacia la derecha y correr a máxima velocidad
        transform.rotation = Quaternion.Euler(0, 0, 0);
        rb.linearVelocity = new Vector2(velocidadHuida, rb.linearVelocity.y);
    }
}