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
        
        if (jugador == null)
        {
            GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
            if (objJugador != null) jugador = objJugador.transform;
        }
    }

    void Update()
    {
        if (estaHuyendo)
        {
            HuirFueraDePantalla();
            return; 
        }

        if (jugador == null) return;

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

    void MirarHaciaJugador()
    {
        float direccion = jugador.position.x > transform.position.x ? 1 : -1;
        
        if (direccion > 0)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    void PerseguirJugador()
    {
        anim.SetBool("IsRunning", true);
        
        float direccion = jugador.position.x > transform.position.x ? 1 : -1;
        rb.linearVelocity = new Vector2(direccion * velocidadSeguimiento, rb.linearVelocity.y);
        
        MirarHaciaJugador(); 
    }

    void IntentarAtacar()
    {
        Detenerse(); 
        MirarHaciaJugador(); 
        
        if (Time.time >= tiempoUltimoAtaque + cooldownAtaque)
        {
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

    private void OnTriggerEnter2D(Collider2D colision)
    {
        if (colision.CompareTag(tagAtaqueJugador) && !estaHuyendo)
        {
            RecibirDano(1);
            Destroy(colision.gameObject);
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
        transform.rotation = Quaternion.Euler(0, 0, 0);
        rb.linearVelocity = new Vector2(velocidadHuida, rb.linearVelocity.y);
    }
}