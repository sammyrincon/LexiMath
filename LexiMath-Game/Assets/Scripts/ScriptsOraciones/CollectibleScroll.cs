// Assets/Scripts/World/CollectibleScroll.cs
using UnityEngine;

public class CollectibleScroll : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode interactKey = KeyCode.E;
    public GameObject interactPrompt;
    
    private bool playerInRange = false;
    private bool collected = false;
    
    private void Awake()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }
    
    private void Update()
    {
        if (playerInRange && !collected && Input.GetKeyDown(interactKey))
        {
            TriggerSentence();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !collected)
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
        SentenceData sentence = SentencePool.GetRandomSentence();
        SentenceUIManager.Instance.ShowSentence(sentence, OnAnswered);
    }
    
    private void OnAnswered(bool correct)
    {
        if (correct)
        {
            collected = true;
            GameProgressManager.Instance.RegisterCollected();
            if (interactPrompt != null) interactPrompt.SetActive(false);
            
            // Efecto visual: el pergamino desaparece o cambia
            gameObject.SetActive(false);
            // Opcional: dejar visible pero "consumido" con cambio de sprite
        }
        // Si falla, no se "abre" — puede reintentar
    }
}