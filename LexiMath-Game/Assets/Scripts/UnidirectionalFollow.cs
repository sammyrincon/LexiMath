using UnityEngine;

public class UnidirectionalFollow : MonoBehaviour
{
    public Transform player; 
    private float maxPosX = -Mathf.Infinity;

    void LateUpdate()
    {
        if (player == null) return;

        // Si la posición actual del jugador es mayor a la máxima registrada
        if (player.position.x > maxPosX)
        {
            maxPosX = player.position.x;
        }

        // El ancla solo se mueve a la posición máxima alcanzada
        transform.position = new Vector3(maxPosX, transform.position.y, transform.position.z);
    }
}