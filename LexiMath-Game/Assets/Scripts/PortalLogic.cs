using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;
    private Animator animator;

    [Header("Configuración")]
    public float distanciaApertura = 5f;
    
    private bool estaAbierto = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (jugador == null)
        {
            GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
            if (objJugador != null) jugador = objJugador.transform;
        }
    }

    void Update()
    {
        if (jugador == null || estaAbierto) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (distancia <= distanciaApertura)
        {
            estaAbierto = true;
            animator.SetTrigger("Open");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && estaAbierto)
        {
            collision.gameObject.SetActive(false);
            animator.SetTrigger("Close");
        }
    }
}