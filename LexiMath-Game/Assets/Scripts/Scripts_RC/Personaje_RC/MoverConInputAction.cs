using UnityEngine;
using UnityEngine.InputSystem;

public class MoverConInputAction : MonoBehaviour
{
    [SerializeField] private InputAction accionMover;
    [SerializeField] private InputAction accionSalto;
    [SerializeField] private float velocidadX = 5f;
    [SerializeField] private float velocidadY = 10f;

    private Rigidbody2D rb;
    private EstadoPersonaje estadoPersonaje;

    void Start()
    {
        accionMover.Enable();
        accionSalto.Enable();
        rb = GetComponent<Rigidbody2D>();
        estadoPersonaje = GetComponentInChildren<EstadoPersonaje>();
    }

    void OnEnable()
    {
        accionSalto.performed += saltar;
    }

    void OnDisable()
    {
        accionSalto.performed -= saltar;
    }

    public void saltar(InputAction.CallbackContext context)
    {
        if (estadoPersonaje.estaEnPiso == true)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, velocidadY);
        }
    }

    void Update()
    {
        Vector2 movimiento = accionMover.ReadValue<Vector2>();
        transform.position = (Vector2)transform.position + movimiento * velocidadX * Time.deltaTime;
    }
}