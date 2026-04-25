using UnityEngine;

public class EfectoPortalSimple : MonoBehaviour
{
    public float velocidadGiro = 100f;
    public bool pulsar = false;
    
    void Update()
    {
        transform.Rotate(0, 0, velocidadGiro * Time.deltaTime);

        if (pulsar)
        {
            float escala = 1f + Mathf.Sin(Time.time * 3f) * 0.1f;
            transform.localScale = new Vector3(escala, escala, 1f);
        }
    }
}