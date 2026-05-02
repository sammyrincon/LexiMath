using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class QuestionUIManager : MonoBehaviour
{
    public static QuestionUIManager Instance;

    [Header("UI References")]
    public GameObject questionPanel;
    public TextMeshProUGUI questionText;
    public Button[] optionButtons;
    public TextMeshProUGUI feedbackText;

    [Header("Feedback")]
    public float feedbackDuration = 1.2f;
    public Color correctColor = new Color(0.2f, 0.85f, 0.3f);
    public Color wrongColor = new Color(0.9f, 0.2f, 0.2f);

    private QuestionData_SR currentQuestion;
    private Action<bool> onAnswerCallback;
    private Color defaultButtonColor;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (questionPanel != null)
            questionPanel.SetActive(false);

        if (optionButtons != null && optionButtons.Length > 0 && optionButtons[0] != null)
            defaultButtonColor = optionButtons[0].image.color;

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
    }

    public void ShowQuestion(QuestionData_SR question, Action<bool> callback)
    {
        if (questionPanel == null || questionText == null) return;

        currentQuestion = question;
        onAnswerCallback = callback;

        questionText.text = question.questionText;

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;
            optionButtons[i].image.color = defaultButtonColor;
            optionButtons[i].interactable = true;

            TextMeshProUGUI btnText = optionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null) btnText.text = question.options[i].ToString();

            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }

        questionPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void OnAnswerSelected(int index)
    {
        bool isCorrect = currentQuestion.options[index] == currentQuestion.correctAnswer;

        foreach (var btn in optionButtons)
            btn.interactable = false;

        // Highlight: green on correct answer, red on the wrong selection
        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (currentQuestion.options[i] == currentQuestion.correctAnswer)
                optionButtons[i].image.color = correctColor;
            else if (i == index)
                optionButtons[i].image.color = wrongColor;
        }

        if (feedbackText != null)
        {
            feedbackText.text = isCorrect ? "¡Correcto! ★" : "Esa no es... ¡Sigue intentando!";
            feedbackText.color = isCorrect ? correctColor : wrongColor;
            feedbackText.gameObject.SetActive(true);
        }

        StartCoroutine(CloseFeedback(isCorrect));
    }

    private IEnumerator CloseFeedback(bool isCorrect)
    {
        yield return new WaitForSecondsRealtime(feedbackDuration);

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        questionPanel.SetActive(false);
        Time.timeScale = 1f;

        onAnswerCallback?.Invoke(isCorrect);
    }
}
