using System;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialHUD : MonoBehaviour
{
    [Header("Preview")]
    [SerializeField] private bool enableDebugKeys = true;

    private readonly string[] stepTitles =
    {
        "PASO 1 | MOVERSE",
        "PASO 2 | RESPONDER",
        "PASO 3 | PODERES",
        "PASO 4 | JEFE FINAL"
    };

    private UIDocument document;
    private VisualElement[] stepPanels;
    private VisualElement[] stepDots;
    private VisualElement[] progressLines;
    private VisualElement[] movementChecks;
    private VisualElement[] powerCards;
    private Button[] powerButtons;
    private Action[] powerCallbacks;

    private Label stepLabel;
    private Label counterLabel;
    private Label hintLabel;
    private Label resultLabel;
    private Label questionLabel;
    private Label bossTitleLabel;
    private Label bossChargeLabel;
    private VisualElement bossHealthFill;
    private Button backButton;
    private Button nextButton;
    private Button answerButton0;
    private Button answerButton1;
    private Button answerButton2;

    private int currentStep;
    private int selectedPowerIndex = 3;

    private void OnEnable()
    {
        document = GetComponent<UIDocument>();

        if (document == null)
        {
            Debug.LogWarning("[TutorialHUD] No se encontro UIDocument en este GameObject.");
            return;
        }

        VisualElement root = document.rootVisualElement;
        if (root == null)
        {
            Debug.LogWarning("[TutorialHUD] El rootVisualElement aun no esta listo.");
            return;
        }

        CacheElements(root);
        RegisterEvents();
        ResetState();
    }

    private void OnDisable()
    {
        UnregisterEvents();
    }

    private void Update()
    {
        if (!enableDebugKeys || document == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) SetStep(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetStep(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetStep(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetStep(3);
        if (Input.GetKeyDown(KeyCode.LeftBracket)) PreviousStep();
        if (Input.GetKeyDown(KeyCode.RightBracket)) NextStep();

        if (Input.GetKeyDown(KeyCode.F1)) SetMovementCheckCompleted(0, true);
        if (Input.GetKeyDown(KeyCode.F2)) SetMovementCheckCompleted(1, true);
        if (Input.GetKeyDown(KeyCode.F3)) SetMovementCheckCompleted(2, true);
        if (Input.GetKeyDown(KeyCode.F4)) ShowHint("Buen ritmo. Ya puedes avanzar.", true);
        if (Input.GetKeyDown(KeyCode.F5)) ShowHint("Todavia falta dominar este paso.", false);
    }

    public void SetStep(int stepIndex)
    {
        if (stepPanels == null || stepPanels.Length == 0)
        {
            return;
        }

        currentStep = Mathf.Clamp(stepIndex, 0, stepPanels.Length - 1);

        for (int i = 0; i < stepPanels.Length; i++)
        {
            SetVisible(stepPanels[i], i == currentStep);
        }

        for (int i = 0; i < stepDots.Length; i++)
        {
            if (stepDots[i] == null)
            {
                continue;
            }

            stepDots[i].EnableInClassList("step-done", i < currentStep);
            stepDots[i].EnableInClassList("step-active", i == currentStep);
            stepDots[i].EnableInClassList("step-pending", i > currentStep);
        }

        for (int i = 0; i < progressLines.Length; i++)
        {
            if (progressLines[i] == null)
            {
                continue;
            }

            progressLines[i].EnableInClassList("line-done", i < currentStep);
            progressLines[i].EnableInClassList("line-active", i == currentStep);
            progressLines[i].EnableInClassList("line-pending", i > currentStep);
        }

        if (stepLabel != null)
        {
            stepLabel.text = stepTitles[currentStep];
        }

        if (counterLabel != null)
        {
            counterLabel.text = string.Format("{0} / {1}", currentStep + 1, stepPanels.Length);
        }

        if (backButton != null)
        {
            backButton.SetEnabled(currentStep > 0);
        }

        if (nextButton != null)
        {
            nextButton.text = currentStep == stepPanels.Length - 1 ? "FINALIZAR" : "SIGUIENTE";
        }
    }

    public void NextStep()
    {
        if (currentStep >= stepPanels.Length - 1)
        {
            ShowFinalPanel();
            return;
        }

        SetStep(currentStep + 1);
    }

    public void PreviousStep()
    {
        SetStep(currentStep - 1);
    }

    public void SetMovementCheckCompleted(int index, bool completed)
    {
        if (movementChecks == null || index < 0 || index >= movementChecks.Length)
        {
            return;
        }

        VisualElement checkElement = movementChecks[index];
        if (checkElement == null)
        {
            return;
        }

        checkElement.EnableInClassList("check-completo", completed);
        checkElement.EnableInClassList("check-pendiente", !completed);
    }

    public void ResetMovementChecks()
    {
        if (movementChecks == null)
        {
            return;
        }

        for (int i = 0; i < movementChecks.Length; i++)
        {
            SetMovementCheckCompleted(i, false);
        }
    }

    public void SetQuestion(string question, string answerA, string answerB, string answerC)
    {
        if (questionLabel != null) questionLabel.text = question;
        if (answerButton0 != null) answerButton0.text = answerA;
        if (answerButton1 != null) answerButton1.text = answerB;
        if (answerButton2 != null) answerButton2.text = answerC;
    }

    public void ShowQuestionResult(bool isCorrect, string message)
    {
        if (resultLabel == null)
        {
            return;
        }

        resultLabel.text = message;
        resultLabel.EnableInClassList("resultado-correcto", isCorrect);
        resultLabel.EnableInClassList("resultado-incorrecto", !isCorrect);

        VisualElement resultPanel = resultLabel.parent;
        if (resultPanel != null)
        {
            SetVisible(resultPanel, true);
        }
    }

    public void HideQuestionResult()
    {
        if (resultLabel != null && resultLabel.parent != null)
        {
            SetVisible(resultLabel.parent, false);
        }
    }

    public void SelectPower(int powerIndex)
    {
        if (powerCards == null || powerIndex < 0 || powerIndex >= powerCards.Length)
        {
            return;
        }

        selectedPowerIndex = powerIndex;

        for (int i = 0; i < powerCards.Length; i++)
        {
            if (powerCards[i] == null)
            {
                continue;
            }

            powerCards[i].EnableInClassList("poder-seleccionado", i == selectedPowerIndex);
        }
    }

    public void SetBossState(string titleText, string chargeText, float healthPercent)
    {
        if (bossTitleLabel != null) bossTitleLabel.text = titleText;
        if (bossChargeLabel != null) bossChargeLabel.text = chargeText;

        if (bossHealthFill != null)
        {
            float safePercent = Mathf.Clamp01(healthPercent);
            bossHealthFill.style.width = Length.Percent(safePercent * 100f);
        }
    }

    public void ShowHint(string message, bool positive)
    {
        if (hintLabel == null)
        {
            return;
        }

        hintLabel.text = message;
        hintLabel.EnableInClassList("hint-positive", positive);
        SetVisible(hintLabel, !string.IsNullOrWhiteSpace(message));
    }

    public void HideHint()
    {
        ShowHint(string.Empty, false);
    }

    private void ShowFinalPanel()
    {
        VisualElement finalPanel = document.rootVisualElement.Q<VisualElement>("panel-final");
        if (finalPanel == null)
        {
            return;
        }

        for (int i = 0; i < stepPanels.Length; i++)
        {
            SetVisible(stepPanels[i], false);
        }

        SetVisible(finalPanel, true);
        HideHint();
    }

    private void ResetState()
    {
        ResetMovementChecks();
        HideQuestionResult();
        HideHint();
        SelectPower(selectedPowerIndex);
        SetBossState("JEFE FINAL", "PODER LISTO 100%", 1f);
        SetQuestion("4 x 2 = ?", "6", "8", "9");

        VisualElement finalPanel = document.rootVisualElement.Q<VisualElement>("panel-final");
        if (finalPanel != null)
        {
            SetVisible(finalPanel, false);
        }

        SetStep(0);
    }

    private void CacheElements(VisualElement root)
    {
        stepPanels = new[]
        {
            root.Q<VisualElement>("panel-movimiento"),
            root.Q<VisualElement>("panel-preguntas"),
            root.Q<VisualElement>("panel-poderes"),
            root.Q<VisualElement>("panel-jefe")
        };

        stepDots = new[]
        {
            root.Q<VisualElement>("step-dot-1"),
            root.Q<VisualElement>("step-dot-2"),
            root.Q<VisualElement>("step-dot-3"),
            root.Q<VisualElement>("step-dot-4")
        };

        progressLines = new[]
        {
            root.Q<VisualElement>("progress-line-1"),
            root.Q<VisualElement>("progress-line-2"),
            root.Q<VisualElement>("progress-line-3")
        };

        movementChecks = new[]
        {
            root.Q<VisualElement>("check-mov-0"),
            root.Q<VisualElement>("check-mov-1"),
            root.Q<VisualElement>("check-mov-2")
        };

        powerCards = new[]
        {
            root.Q<VisualElement>("card-poder-0"),
            root.Q<VisualElement>("card-poder-1"),
            root.Q<VisualElement>("card-poder-2"),
            root.Q<VisualElement>("card-poder-3")
        };

        powerButtons = new[]
        {
            root.Q<Button>("btn-poder-0"),
            root.Q<Button>("btn-poder-1"),
            root.Q<Button>("btn-poder-2"),
            root.Q<Button>("btn-poder-3")
        };

        powerCallbacks = new Action[powerButtons.Length];

        stepLabel = root.Q<Label>("label-paso-actual");
        counterLabel = root.Q<Label>("label-contador");
        hintLabel = root.Q<Label>("label-hint");
        resultLabel = root.Q<Label>("label-resultado");
        questionLabel = root.Q<Label>("label-pregunta");
        bossTitleLabel = root.Q<Label>("label-poder-activo");
        bossChargeLabel = root.Q<Label>("label-poder-carga");
        bossHealthFill = root.Q<VisualElement>("boss-health-bar-fill");
        backButton = root.Q<Button>("btn-atras");
        nextButton = root.Q<Button>("btn-siguiente");
        answerButton0 = root.Q<Button>("btn-respuesta-0");
        answerButton1 = root.Q<Button>("btn-respuesta-1");
        answerButton2 = root.Q<Button>("btn-respuesta-2");
    }

    private void RegisterEvents()
    {
        if (backButton != null) backButton.clicked += PreviousStep;
        if (nextButton != null) nextButton.clicked += NextStep;

        for (int i = 0; i < powerButtons.Length; i++)
        {
            if (powerButtons[i] == null)
            {
                continue;
            }

            int capturedIndex = i;
            powerCallbacks[i] = () => SelectPower(capturedIndex);
            powerButtons[i].clicked += powerCallbacks[i];
        }
    }

    private void UnregisterEvents()
    {
        if (backButton != null) backButton.clicked -= PreviousStep;
        if (nextButton != null) nextButton.clicked -= NextStep;

        if (powerButtons == null || powerCallbacks == null)
        {
            return;
        }

        for (int i = 0; i < powerButtons.Length; i++)
        {
            if (powerButtons[i] != null && powerCallbacks[i] != null)
            {
                powerButtons[i].clicked -= powerCallbacks[i];
            }
        }
    }

    private static void SetVisible(VisualElement element, bool isVisible)
    {
        if (element == null)
        {
            return;
        }

        element.EnableInClassList("panel-oculto", !isVisible);
    }
}
