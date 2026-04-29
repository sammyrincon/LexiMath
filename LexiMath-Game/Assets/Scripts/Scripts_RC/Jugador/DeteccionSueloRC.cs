using UnityEngine;

public class DeteccionSueloRC : MonoBehaviour
{
    [Header("Configuración de Suelo")]
    public Transform controladorSuelo;
    public LayerMask capaSuelo;
    public float radioDeteccion = 0.2f;

    [Header("Estado")]
    public bool tocandoSuelo;

    public float groundedConfirmTime = 0.08f;
    private float groundedTimer = 0f;

    void FixedUpdate()
    {
        if (controladorSuelo == null) return;

        bool detected = Physics2D.OverlapCircle(controladorSuelo.position, radioDeteccion, capaSuelo);

        if (detected)
        {
            groundedTimer = groundedConfirmTime;
            tocandoSuelo = true;
        }
        else
        {
            groundedTimer -= Time.fixedDeltaTime;
            if (groundedTimer <= 0f) tocandoSuelo = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (controladorSuelo == null) return;
        Gizmos.color = tocandoSuelo ? Color.green : Color.red;
        Gizmos.DrawWireSphere(controladorSuelo.position, radioDeteccion);
    }
}
