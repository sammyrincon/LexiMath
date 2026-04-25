using UnityEngine;
using UnityEngine.Events;

public class QuestionManager : MonoBehaviour
{
    [Header("Referencia al PlayerHealth (sin Singleton)")]
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Daño cuando falla una respuesta")]
    [SerializeField] private int damageOnWrong = 1;

    [Header("Eventos (opcional)")]
    public UnityEvent onCorrect;
    public UnityEvent onWrong;

    private void Awake()
    {
#if UNITY_2022_2_OR_NEWER
        if (playerHealth == null) playerHealth = FindFirstObjectByType<PlayerHealth>();
#else
        if (playerHealth == null) playerHealth = FindObjectOfType<PlayerHealth>();
#endif
    }

    // ✅ Métodos comunes que suelen usar AnswerBlock/NPCTrigger
    public void StartQuestions()
    {
        // Aquí puedes abrir UI de preguntas, pausar juego, etc.
        Debug.Log("Iniciando preguntas...");
    }

    public void AnswerCorrect()
    {
        onCorrect?.Invoke();
        Debug.Log("Respuesta correcta ✅");
    }

    public void AnswerWrong()
    {
        onWrong?.Invoke();
        Debug.Log("Respuesta incorrecta ❌");

        if (playerHealth != null)
        {
            // Dirección fake para knockback (si lo usas)
            Vector2 hitPoint = transform.position;
            Vector2 hitDir = Vector2.left;

            playerHealth.TakeDamage(damageOnWrong, hitPoint, hitDir);
        }
    }

    public void SubmitAnswer(bool isCorrect)
    {
        if (isCorrect) AnswerCorrect();
        else AnswerWrong();
    }

    public void SubmitAnswer(int chosenIndex, int correctIndex)
    {
        if (chosenIndex == correctIndex) AnswerCorrect();
        else AnswerWrong();
    }
}
