// Assets/Scripts/World/NPCInteractable.cs
using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode interactKey = KeyCode.E;
    public GameObject interactPrompt;
    public string npcName = "Sabio";
    
    private bool playerInRange = false;
    private bool questAnswered = false;
    
    // ← FALTABAN ESTAS DOS LÍNEAS
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
            TriggerSentence();
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
    
    private void TriggerSentence()
    {
        Debug.Log($"{npcName}: Completa esta oración para que te deje pasar...");
        SentenceData sentence = SentencePool.GetRandomSentence();
        SentenceUIManager.Instance.ShowSentence(sentence, OnAnswered);
    }
    
    private void OnAnswered(bool correct)
    {
        if (correct)
        {
            questAnswered = true;
            GameProgressManager.Instance.RegisterCollected();
            if (interactPrompt != null) interactPrompt.SetActive(false);
            Debug.Log($"{npcName}: ¡Bien hecho, joven aprendiz!");
        }
        else
        {
            Debug.Log($"{npcName}: Esa no es la respuesta. Inténtalo de nuevo.");
        }
    }
}