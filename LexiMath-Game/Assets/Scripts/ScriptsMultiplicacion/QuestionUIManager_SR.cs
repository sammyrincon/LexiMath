using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class QuestionUIManager : MonoBehaviour
{
    public static QuestionUIManager Instance;
    
    [Header("UI References")]
    public GameObject questionPanel;
    public TextMeshProUGUI questionText;
    public Button[] optionButtons;
    
    private QuestionData currentQuestion;
    private Action<bool> onAnswerCallback;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        if (questionPanel != null)
            questionPanel.SetActive(false);
    }
    
    public void ShowQuestion(QuestionData question, Action<bool> callback)
    {
        currentQuestion = question;
        onAnswerCallback = callback;
        
        questionText.text = question.questionText;
        
        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;
            TextMeshProUGUI btnText = optionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = question.options[i].ToString();
            
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }
        
        questionPanel.SetActive(true);
        Time.timeScale = 0f;
    }
    
    private void OnAnswerSelected(int index)
    {
        bool isCorrect = currentQuestion.options[index] == currentQuestion.correctAnswer;
        
        questionPanel.SetActive(false);
        Time.timeScale = 1f;
        
        onAnswerCallback?.Invoke(isCorrect);
    }
}