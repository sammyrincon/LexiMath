using UnityEngine;
using System.Collections; 

public class Proyectil_RC : MonoBehaviour
{
    public float velocidadX = 10f;
    public int danoFuego = 20;
    
    [Header("Tiempos de Animación")]
    [Tooltip("Tiempo en segundos que tarda en formarse antes de moverse")]
    public float tiempoFormacion = 0.3f; 
    [Tooltip("Tiempo que dura la explosión al chocar")]
    public float tiempoExplosion = 0.5f; 

    private Rigidbody2D rb;
    private SpriteRenderer rendererProyectil;
    private Animator animador;
    private bool mostrando = false;
    private bool yaImpacto = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rendererProyectil = GetComponent<SpriteRenderer>();
        animador = GetComponent<Animator>();
        
        StartCoroutine(RutinaFormacion());
    }

    private IEnumerator RutinaFormacion()
    {
        yield return new WaitForSeconds(tiempoFormacion);

        if (yaImpacto == false)
        {
            rb.linearVelocity = new Vector2(velocidadX, 0f);
        }
    }

    void Update()
    {
        if (mostrando && rendererProyectil.isVisible == false && yaImpacto == false)
        {
            Destroy(gameObject);
        }
        mostrando = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (yaImpacto == true) return;

        if (collision.CompareTag("Enemy"))
        {
            yaImpacto = true;
            
            rb.linearVelocity = Vector2.zero;

            Sistema_Salud_RC saludEnemigo = collision.GetComponent<Sistema_Salud_RC>();
            if (saludEnemigo != null)
            {
                saludEnemigo.RecibirDano(danoFuego);
            }

            if (animador != null)
            {
                animador.SetTrigger("Impacto");
            }
            
            Destroy(gameObject, tiempoExplosion);
        }
    }
}