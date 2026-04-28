// Assets/Scripts/World/EyeOrb.cs
using UnityEngine;

public class EyeOrb : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode interactKey = KeyCode.E;
    public GameObject interactPrompt;
    
    [Header("Visual Feedback")]
    public SpriteRenderer orbRenderer;       // arrastrar el SpriteRenderer del ojo
    public Color consumedColor = new Color(0.3f, 0.3f, 0.3f, 0.6f); // gris apagado
    public bool destroyOnConsume = false;     // si true, desaparece; si false, queda apagado
    
    private bool playerInRange = false;
    private bool consumed = false;
    
    private void Awake()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (orbRenderer == null) orbRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Update()
    {
        if (playerInRange && !consumed && Input.GetKeyDown(interactKey))
        {
            TriggerSentence();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !consumed)
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
            consumed = true;
            GameProgressManager.Instance.RegisterCollected();
            if (interactPrompt != null) interactPrompt.SetActive(false);
            
            // Feedback visual: el ojo se "apaga"
            if (destroyOnConsume)
                gameObject.SetActive(false);
            else if (orbRenderer != null)
                orbRenderer.color = consumedColor;
        }
        // Si falla, el ojo sigue activo, puede reintentar
    }
}