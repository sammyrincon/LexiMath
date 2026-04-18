using UnityEngine;

public class PortalLogic : MonoBehaviour
{
    private Animator anim;
    public float radioDeteccion = 5f;
    public Transform jugador;

    void Start() {
        anim = GetComponent<Animator>();
    }

    void Update() {
        float distancia = Vector2.Distance(transform.position, jugador.position);
        anim.SetBool("PlayerNear", distancia < radioDeteccion);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            anim.SetTrigger("PlayerEntered");
        }
    }
}