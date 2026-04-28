// Assets/Scripts/Questions/SentenceUIManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SentenceUIManager : MonoBehaviour
{
    public static SentenceUIManager Instance;
    
    [Header("UI References")]
    public GameObject sentencePanel;
    public TextMeshProUGUI sentenceText;
    public Button[] optionButtons;
    public TextMeshProUGUI feedbackText; // opcional, "¡Correcto!" o "Inténtalo de nuevo"
    
    private SentenceData currentSentence;
    private Action<bool> onAnswerCallback;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        if (sentencePanel != null)
            sentencePanel.SetActive(false);
    }
    
    public void ShowSentence(SentenceData sentence, Action<bool> callback)
    {
        currentSentence = sentence;
        onAnswerCallback = callback;
        
        sentenceText.text = sentence.sentenceWithBlank;
        if (feedbackText != null) feedbackText.text = "";
        
        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;
            TextMeshProUGUI btnText = optionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = sentence.options[i];
            
            optionButtons[i].interactable = true;
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }
        
        sentencePanel.SetActive(true);
        Time.timeScale = 0f;
    }
    
    private void OnAnswerSelected(int index)
    {
        bool isCorrect = currentSentence.options[index] == currentSentence.correctWord;
        
        if (feedbackText != null)
            feedbackText.text = isCorrect ? "¡Correcto!" : "Inténtalo de nuevo";
        
        // Pequeño delay antes de cerrar para que vean el feedback
        StartCoroutine(CloseAfterDelay(isCorrect));
    }
    
    private System.Collections.IEnumerator CloseAfterDelay(bool isCorrect)
    {
        // Deshabilitar botones para que no se haga doble click
        foreach (var btn in optionButtons) btn.interactable = false;
        
        yield return new WaitForSecondsRealtime(1f);
        
        sentencePanel.SetActive(false);
        Time.timeScale = 1f;
        
        onAnswerCallback?.Invoke(isCorrect);
    }
}