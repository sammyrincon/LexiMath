using UnityEngine;

public class Sistema_Salud_RC : MonoBehaviour
{
    public int saludMaxima = 100;
    public int saludActual { get; private set; }
    private Animator animator;

    void Start()
    {
        saludActual = saludMaxima;
    }

    public void RecibirDano(int cantidad)
    {
        saludActual -= cantidad;
        Debug.Log(gameObject.name + " recibió daño. Salud restante: " + saludActual);
        animator.SetTrigger("Hurt");

        if (saludActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        animator.SetTrigger("Death");
        Destroy(gameObject);
    }

    public void Curar(int cantidad)
    {
        saludActual += cantidad;
        if (saludActual > saludMaxima)
        {
            saludActual = saludMaxima;
        }
        Debug.Log("¡El personaje se curó! Salud actual: " + saludActual);
    }
}
