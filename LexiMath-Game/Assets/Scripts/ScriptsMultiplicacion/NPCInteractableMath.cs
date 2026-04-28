// Assets/Scripts/World/NPCInteractableMath.cs
using UnityEngine;

public class NPCInteractableMath : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode interactKey = KeyCode.E;
    public GameObject interactPrompt;
    public string npcName = "Sabio";
    
    [Header("Reward")]
    public int soulsReward = 50;
    
    private bool playerInRange = false;
    private bool questAnswered = false;
    private SpriteRenderer spriteRenderer;
    private Transform player;
    
    private void Awake()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);
        spriteRenderer = GetComponent<SpriteRenderer>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }
    
    private void Update()
    {
        if (playerInRange && player != null && spriteRenderer != null)
        {
            if (player.position.x < transform.position.x)
                spriteRenderer.flipX = true;
            else
                spriteRenderer.flipX = false;
        }
        
        if (playerInRange && !questAnswered && Input.GetKeyDown(interactKey))
        {
            TriggerQuestion();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !questAnswered)
        {
            playerInRange = true;
            if (interactPrompt != null) interactPrompt.SetActive(true);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }
    }
    
    private void TriggerQuestion()
    {
        Debug.Log($"{npcName}: Resuelve esta multiplicación para ganar mi recompensa...");
        QuestionData question = QuestionPool.GenerateMultiplicationQuestion();
        QuestionUIManager.Instance.ShowQuestion(question, OnAnswered);
    }
    
    private void OnAnswered(bool correct)
    {
        if (correct)
        {
            questAnswered = true;
            if (PlayerSouls.Instance != null)
                PlayerSouls.Instance.AddSouls(soulsReward);
            if (interactPrompt != null) interactPrompt.SetActive(false);
            Debug.Log($"{npcName}: ¡Excelente cálculo! Toma estas {soulsReward} almas.");
        }
        else
        {
            // Sin castigo, solo mensaje. Puede reintentar.
            Debug.Log($"{npcName}: Esa no es la respuesta. Vuelve cuando estés listo.");
        }
    }
}