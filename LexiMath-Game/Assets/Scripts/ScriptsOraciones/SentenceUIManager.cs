using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;

public class SentenceUIManager : MonoBehaviour
{
    public static SentenceUIManager Instance;

    public float feedbackDuration = 1.2f;

    private VisualElement overlay;
    private Label sentenceText;
    private Button[] optionButtons;
    private Label feedbackLabel;

    private SentenceData currentSentence;
    private Action<bool> onAnswerCallback;

    private static readonly StyleColor ColDefault = new StyleColor(new Color32(80, 50, 20, 230));
    private static readonly StyleColor ColCorrect = new StyleColor(new Color32(51, 216, 76, 255));
    private static readonly StyleColor ColWrong   = new StyleColor(new Color32(229, 51, 51, 255));

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        var root = OracionesHUDController.Instance.Root;

        overlay       = root.Q<VisualElement>("sentence-overlay");
        sentenceText  = root.Q<Label>("sentence-text");
        feedbackLabel = root.Q<Label>("feedback-label");

        optionButtons = new[]
        {
            root.Q<Button>("btn-opt-0"),
            root.Q<Button>("btn-opt-1"),
            root.Q<Button>("btn-opt-2"),
            root.Q<Button>("btn-opt-3"),
        };

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int idx = i;
            optionButtons[i].clicked += () => OnAnswerSelected(idx);
        }
    }

    public void ShowSentence(SentenceData sentence, Action<bool> callback)
    {
        if (overlay == null) return;

        currentSentence  = sentence;
        onAnswerCallback = callback;

        sentenceText.text = sentence.sentenceWithBlank;
        feedbackLabel.style.display = DisplayStyle.None;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            optionButtons[i].text = sentence.options[i];
            optionButtons[i].SetEnabled(true);
            optionButtons[i].style.backgroundColor = ColDefault;
        }

        overlay.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;
    }

    private void OnAnswerSelected(int index)
    {
        if (currentSentence == null) return;

        bool isCorrect = currentSentence.options[index] == currentSentence.correctWord;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            optionButtons[i].SetEnabled(false);
            if (currentSentence.options[i] == currentSentence.correctWord)
                optionButtons[i].style.backgroundColor = ColCorrect;
            else if (i == index)
                optionButtons[i].style.backgroundColor = ColWrong;
        }

        feedbackLabel.text = isCorrect ? "¡Correcto!" : "Esa no es... ¡Mira la correcta!";
        feedbackLabel.style.color = isCorrect ? ColCorrect : ColWrong;
        feedbackLabel.style.display = DisplayStyle.Flex;

        StartCoroutine(CloseFeedback(isCorrect));
    }

    private IEnumerator CloseFeedback(bool isCorrect)
    {
        yield return new WaitForSecondsRealtime(feedbackDuration);
        overlay.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
        onAnswerCallback?.Invoke(isCorrect);
    }
}
