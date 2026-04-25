using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class UIManager2 : MonoBehaviour
{
    public static UIManager2 Instance;

    private VisualElement root;
    private VisualElement questionPanel;
    private Label questionText;
    private Button[] answerButtons = new Button[4];
    private Button helpButton;
    
    private QuestionData currentQuestion;
    private bool isAnswering = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        questionPanel = root.Q<VisualElement>("question-panel");
        questionText = root.Q<Label>("question-text");
        helpButton = root.Q<Button>("btn-help");

        for (int i = 0; i < 4; i++)
        {
            int index = i;
            answerButtons[i] = root.Q<Button>($"btn-answer-{i}");
            answerButtons[i].clicked += () => OnAnswerSelected(index);
        }

        helpButton.clicked += OnHelpClicked;
    }

    public void ShowQuestion(QuestionData qData)
    {
        currentQuestion = qData;
        isAnswering = true;
        questionText.text = qData.questionText;

        for (int i = 0; i < 4; i++)
        {
            answerButtons[i].text = qData.answers[i];
            answerButtons[i].RemoveFromClassList("btn-correct");
            answerButtons[i].RemoveFromClassList("btn-incorrect");
            answerButtons[i].SetEnabled(true);
        }
        
        helpButton.SetEnabled(true);
        questionPanel.RemoveFromClassList("panel-hidden");
        Time.timeScale = 0; 
    }

    private void OnAnswerSelected(int selectedIndex)
    {
        if (!isAnswering) return;
        isAnswering = false;

        foreach (var btn in answerButtons) btn.SetEnabled(false);
        helpButton.SetEnabled(false);

        bool isCorrect = (selectedIndex == currentQuestion.correctAnswerIndex);

        if (isCorrect)
        {
            answerButtons[selectedIndex].AddToClassList("btn-correct");
        }
        else
        {
            answerButtons[selectedIndex].AddToClassList("btn-incorrect");
            answerButtons[currentQuestion.correctAnswerIndex].AddToClassList("btn-correct");
        }

        string temaStr = currentQuestion.subject.ToString();
        AnalyticsManager.Instance.RegistrarRespuesta(temaStr, isCorrect);

        StartCoroutine(ClosePanelAfterDelay(1.5f));
    }

    private void OnHelpClicked()
    {
        questionText.text = currentQuestion.helpText;
        helpButton.SetEnabled(false);
    }

    private IEnumerator ClosePanelAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        questionPanel.AddToClassList("panel-hidden");
        Time.timeScale = 1; 
    }
}