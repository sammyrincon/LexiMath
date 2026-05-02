using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class SentenceUIManager : MonoBehaviour
{
    public static SentenceUIManager Instance;

    [Header("UI References")]
    public GameObject sentencePanel;
    public TextMeshProUGUI sentenceText;
    public Button[] optionButtons;
    public TextMeshProUGUI feedbackText;

    [Header("Feedback")]
    public float feedbackDuration = 1.2f;
    public Color correctColor = new Color(0.2f, 0.85f, 0.3f);
    public Color wrongColor = new Color(0.9f, 0.2f, 0.2f);

    private SentenceData currentSentence;
    private Action<bool> onAnswerCallback;
    private Color defaultButtonColor;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (sentencePanel != null)
            sentencePanel.SetActive(false);

        if (optionButtons != null && optionButtons.Length > 0 && optionButtons[0] != null)
            defaultButtonColor = optionButtons[0].image.color;

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
    }

    public void ShowSentence(SentenceData sentence, Action<bool> callback)
    {
        if (sentencePanel == null || sentenceText == null) return;

        currentSentence = sentence;
        onAnswerCallback = callback;

        sentenceText.text = sentence.sentenceWithBlank;

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;
            optionButtons[i].image.color = defaultButtonColor;
            optionButtons[i].interactable = true;

            TextMeshProUGUI btnText = optionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null) btnText.text = sentence.options[i];

            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }

        sentencePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void OnAnswerSelected(int index)
    {
        bool isCorrect = currentSentence.options[index] == currentSentence.correctWord;

        foreach (var btn in optionButtons)
            btn.interactable = false;

        // Highlight correct answer green; wrong selection red
        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (currentSentence.options[i] == currentSentence.correctWord)
                optionButtons[i].image.color = correctColor;
            else if (i == index)
                optionButtons[i].image.color = wrongColor;
        }

        if (feedbackText != null)
        {
            feedbackText.text = isCorrect ? "¡Correcto! ★" : "Esa no es... ¡Mira la correcta!";
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

        sentencePanel.SetActive(false);
        Time.timeScale = 1f;

        onAnswerCallback?.Invoke(isCorrect);
    }
}