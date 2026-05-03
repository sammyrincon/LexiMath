using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NPCTrigger : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El QuestionManager de la escena que se va a activar")]
    public QuestionManager questionManager;

    [Header("Diálogo inicial (opcional)")]
    [Tooltip("Texto que se muestra cuando el jugador se acerca por primera vez")]
    [TextArea]
    public string dialogoInicial = "¡Hola! ¿Me ayudas con esta pregunta?";

    [Tooltip("Segundos que se muestra el saludo antes de lanzar la pregunta")]
    public float delaySaludo = 1.5f;

    [Header("NPC Dialog")]
    [Tooltip("Opcional: NPCDialog para mostrar el saludo. Si está vacío, se busca en el padre.")]
    public NPCDialog npcDialog;

    private bool _yaActivado = false;

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        if (!col.isTrigger)
        {
            Debug.LogWarning($"[NPCTrigger] El collider de {name} debe tener 'Is Trigger' = true");
            col.isTrigger = true;
        }

        if (npcDialog == null)
            npcDialog = GetComponentInParent<NPCDialog>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_yaActivado) return;
        if (!other.CompareTag("Player")) return;

        _yaActivado = true;
        StartCoroutine(ActivarPreguntas());
    }

    private System.Collections.IEnumerator ActivarPreguntas()
    {
        if (npcDialog != null && !string.IsNullOrEmpty(dialogoInicial))
        {
            npcDialog.MostrarDialogo(dialogoInicial);
            yield return new WaitForSeconds(delaySaludo);
        }

        if (questionManager != null)
            questionManager.IniciarPreguntas();
        else
            Debug.LogError("[NPCTrigger] No se asignó QuestionManager!");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = _yaActivado ? Color.gray : new Color(1f, 0.8f, 0f, 0.3f);

        var col = GetComponent<Collider2D>();
        if (col is CircleCollider2D circle)
        {
            Gizmos.DrawWireSphere(transform.position + (Vector3)circle.offset, circle.radius);
        }
        else if (col is BoxCollider2D box)
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)box.offset, box.size);
        }
    }
}
