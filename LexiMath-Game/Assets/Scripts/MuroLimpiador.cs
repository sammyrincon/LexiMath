using UnityEngine;

public class LimpiadorDeZona : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D colision)
    {
        if (!colision.CompareTag("Player"))
        {
            colision.gameObject.SetActive(false);
        }
    }
}