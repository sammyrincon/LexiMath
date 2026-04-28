using UnityEngine;

public class Sistema_Salud_RC : MonoBehaviour
{
    public int saludMaxima = 100;
    public int saludActual { get; private set; } 
    private Animator animator;

    // En este metodo se inicializa la salud actual al valor de salud maxima al inicio del juego
    void Start()
    {
        saludActual = saludMaxima;
    }

    // Metodo para recbir daño 
    // La variable cantidad representa la cantidad de daño que se va a recibir

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
    // En este metodo se destruye el objeto del juego cuando la salud llega a cero o menos
    private void Morir()
    {
        animator.SetTrigger("Death");
        Destroy(gameObject);
        
    }
    public void Curar(int cantidad)
{
    saludActual += cantidad;
    Debug.Log("¡El personaje se curó! Salud actual: " + saludActual);
}
}