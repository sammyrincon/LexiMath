using UnityEngine;

public class CamaraMetalSlug : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;
    
    [Header("Configuración")]
    public float offsetZ = -10f; // Profundidad de la cámara
    
    private float maxPosicionX; // Guarda lo más lejos que hemos llegado

    void Start()
    {
        // Guardamos la posición inicial de la cámara
        maxPosicionX = transform.position.x;
    }

    // Usamos LateUpdate para que la cámara se mueva DESPUÉS del jugador y evitar temblores
    void LateUpdate()
    {
        if (jugador == null) return;

        // Si Mael avanzó más a la derecha, actualizamos nuestro límite
        if (jugador.position.x > maxPosicionX)
        {
            maxPosicionX = jugador.position.x;
        }

        // La cámara siempre se queda en la posición máxima alcanzada
        transform.position = new Vector3(maxPosicionX, transform.position.y, offsetZ);
    }
}