using UnityEngine;

public class BossSamuraiIA_RC : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float velocidad = 3f;
    [SerializeField] private float distanciaAtaque = 2f;

    [Header("Configuración de Combate")]
    [SerializeField] private float cooldownAtaque = 1.5f;
    
    [Header("Configuración de Escape (Derrota)")]
    [SerializeField] private float velocidadEscape = 6f;
    [SerializeField] private float tiempoParaDesaparecer = 4f;

    private float temporizadorAtaque = 0f;
    private bool estaEscapando = false;
    private Vector2 direccionEscape;

    private Animator animator;
    private Transform jugador;
    private Sistema_Salud_RC miSalud;
    private ArmaEnemigoRC hitboxAtaque;

    void Start()
    {
        GameObject objetoJugador = GameObject.FindGameObjectWithTag("Player");
        if (objetoJugador != null)
        {
            jugador = objetoJugador.transform;
        }

        animator = GetComponent<Animator>();
        miSalud = GetComponent<Sistema_Salud_RC>();

        ArmaEnemigoRC[] armas = GetComponentsInChildren<ArmaEnemigoRC>();
        foreach (ArmaEnemigoRC arma in armas)
        {
            if (arma.gameObject.name == "Hitbox_Attack") 
            {
                hitboxAtaque = arma;
            }
        }
    }

    void Update()
    {
        if (jugador == null || miSalud == null) return;

        if (miSalud.saludActual <= 0 && !estaEscapando)
        {
            EmpezarEscape();
        }

        if (estaEscapando)
        {
            transform.position = (Vector2)transform.position + (direccionEscape * velocidadEscape * Time.deltaTime);
            return; 
        }

        temporizadorAtaque += Time.deltaTime;
        float distancia = Vector2.Distance(transform.position, jugador.position);
        Vector2 direccion = jugador.position - transform.position;

        if (direccion.x < 0) transform.eulerAngles = new Vector3(0, 180, 0);
        else if (direccion.x > 0) transform.eulerAngles = new Vector3(0, 0, 0);

        if (distancia <= distanciaAtaque)
        {
            animator.SetFloat("Speed", 0f); 

            if (temporizadorAtaque >= cooldownAtaque)
            {
                animator.SetTrigger("Attack");
                temporizadorAtaque = 0f; 
            }
        }
        else
        {
            direccion.Normalize();
            transform.position = (Vector2)transform.position + (direccion * velocidad * Time.deltaTime);
            animator.SetFloat("Speed", 1f); 
        }
    }

    private void EmpezarEscape()
    {
        estaEscapando = true;
        animator.SetTrigger("Death"); 

        float dirX = (transform.position.x > jugador.position.x) ? 1f : -1f;
        direccionEscape = new Vector2(dirX, 0).normalized;

        if (dirX > 0) transform.eulerAngles = new Vector3(0, 0, 0);
        else transform.eulerAngles = new Vector3(0, 180, 0);

        if (hitboxAtaque != null) hitboxAtaque.gameObject.SetActive(false);

        Collider2D miCollider = GetComponent<Collider2D>();
        if (miCollider != null) miCollider.enabled = false;

        Destroy(gameObject, tiempoParaDesaparecer);
    }

    public void PrenderAtaque()
    {
        if (hitboxAtaque != null) hitboxAtaque.ActivarHitbox();
    }

    public void ApagarAtaque()
    {
        if (hitboxAtaque != null) hitboxAtaque.ApagarHitbox();
    }
}