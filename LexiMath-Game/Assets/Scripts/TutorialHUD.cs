using UnityEngine;
using UnityEngine.UIElements;

public class TutorialHUD : MonoBehaviour
{
    private readonly string[] _stepTitles =
    {
        "PASO 1: MOVER",
        "PASO 2: SALTAR",
        "PASO 3: ATACAR",
        "PASO 4: MATAR AL ENEMIGO"
    };

    private UIDocument      _document;
    private VisualElement[] _stepDots;
    private VisualElement[] _progressLines;
    private VisualElement[] _stepPanels;
    private VisualElement   _panelPortal;
    private Label           _labelPasoActual;

    private int _currentStep = 0;

    private void OnEnable()
    {
        _document = GetComponent<UIDocument>();
        if (_document == null)
        {
            Debug.LogError("[TutorialHUD] No se encontró UIDocument.");
            return;
        }

        var root = _document.rootVisualElement;
        if (root == null) return;

        CacheElements(root);
        SetStep(0);
    }

    public void SetStep(int stepIndex)
    {
        if (_stepDots == null) return;

        _currentStep = Mathf.Clamp(stepIndex, 0, _stepDots.Length - 1);

        for (int i = 0; i < _stepDots.Length; i++)
        {
            if (_stepDots[i] == null) continue;

            _stepDots[i].EnableInClassList("step-done",    i < _currentStep);
            _stepDots[i].EnableInClassList("step-active",  i == _currentStep);
            _stepDots[i].EnableInClassList("step-pending", i > _currentStep);
        }

        for (int i = 0; i < _progressLines.Length; i++)
        {
            if (_progressLines[i] == null) continue;
            _progressLines[i].EnableInClassList("line-done", i < _currentStep);
        }

        if (_labelPasoActual != null && _currentStep < _stepTitles.Length)
            _labelPasoActual.text = _stepTitles[_currentStep];

        for (int i = 0; i < _stepPanels.Length; i++)
        {
            if (_stepPanels[i] == null) continue;
            _stepPanels[i].EnableInClassList("panel-oculto", i != _currentStep);
        }

        Debug.Log($"[TutorialHUD] Paso: {_currentStep} — {_stepTitles[_currentStep]}");
    }

    public void SetMovementCheckCompleted(int index, bool completed)
    {
        Debug.Log($"[TutorialHUD] Check {index} = {completed}");

    }

    public void ResetMovementChecks()
    {
        SetStep(0);
    }

    public void MostrarMensajePortal()
    {
        if (_panelPortal == null)
        {
            Debug.LogWarning("[TutorialHUD] panel-portal no encontrado en UXML.");
            return;
        }

        _panelPortal.EnableInClassList("panel-oculto", false);
        Debug.Log("[TutorialHUD] Mensaje portal mostrado.");
    }

    public void MostrarPanelFinal()
    {
        if (_document == null) return;

        var root = _document.rootVisualElement;
        var panelFinal = root?.Q<VisualElement>("panel-final");
        if (panelFinal == null) return;

        foreach (var panel in _stepPanels)
            panel?.EnableInClassList("panel-oculto", true);

        if (_panelPortal != null)
            _panelPortal.EnableInClassList("panel-oculto", true);

        panelFinal.EnableInClassList("panel-oculto", false);

        Debug.Log("[TutorialHUD] Panel final mostrado.");
    }

    private void CacheElements(VisualElement root)
    {
        _stepDots = new[]
        {
            root.Q<VisualElement>("step-dot-1"),
            root.Q<VisualElement>("step-dot-2"),
            root.Q<VisualElement>("step-dot-3"),
            root.Q<VisualElement>("step-dot-4")
        };

        _progressLines = new[]
        {
            root.Q<VisualElement>("progress-line-1"),
            root.Q<VisualElement>("progress-line-2"),
            root.Q<VisualElement>("progress-line-3")
        };

        _stepPanels = new[]
        {
            root.Q<VisualElement>("panel-movimiento"),
            root.Q<VisualElement>("panel-saltar"),
            root.Q<VisualElement>("panel-atacar"),
            root.Q<VisualElement>("panel-matar")
        };

        _panelPortal = root.Q<VisualElement>("panel-portal");

        _labelPasoActual = root.Q<Label>("label-paso-actual");

        for (int i = 0; i < _stepDots.Length; i++)
        {
            if (_stepDots[i] == null)
                Debug.LogError($"[TutorialHUD] step-dot-{i + 1} no encontrado en UXML.");
        }

        for (int i = 0; i < _stepPanels.Length; i++)
        {
            if (_stepPanels[i] == null)
                Debug.LogError($"[TutorialHUD] Panel paso {i} no encontrado en UXML.");
        }

        if (_panelPortal == null)
            Debug.LogWarning("[TutorialHUD] panel-portal no encontrado en UXML.");

        if (_labelPasoActual == null)
            Debug.LogWarning("[TutorialHUD] label-paso-actual no encontrado en UXML.");
    }
}
