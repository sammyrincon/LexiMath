using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private enum TutorialStep
    {
        Move        = 0,
        Jump        = 1,
        Attack      = 2,
        KillEnemy   = 3,
        EnterPortal = 4,
        Completed   = 5
    }

    [Header("Portal (se activa al matar al enemigo)")]
    [SerializeField] private GameObject portalObject;

    [Header("HUD del tutorial")]
    [SerializeField] private TutorialHUD tutorialHUD;

    private TutorialStep _currentStep = TutorialStep.Move;
    private bool _hasMoved;
    private bool _hasJumped;
    private bool _hasAttacked;
    private bool _enemyKilled;
    private bool _portalEntered;

    private void Start()
    {
        SetActive(portalObject, false);

        if (tutorialHUD == null)
            tutorialHUD = Object.FindFirstObjectByType<TutorialHUD>();

        tutorialHUD?.SetStep(0);

        Debug.Log("[TutorialManager] Tutorial iniciado — Paso: Mover");
    }

    public void OnPlayerMove()
    {
        if (_hasMoved) return;
        _hasMoved = true;
        Debug.Log("[TutorialManager] Mover completado");
        TryAdvanceStep();
    }

    public void OnPlayerJump()
    {
        if (_hasJumped) return;
        _hasJumped = true;
        Debug.Log("[TutorialManager] Saltar completado");
        TryAdvanceStep();
    }

    public void OnPlayerAttack()
    {
        if (_hasAttacked) return;
        _hasAttacked = true;
        Debug.Log("[TutorialManager] Atacar completado");
        TryAdvanceStep();
    }

    public void OnEnemyKilled()
    {
        _hasAttacked = true;
        _enemyKilled = true;

        if (_currentStep < TutorialStep.KillEnemy)
            _currentStep = TutorialStep.KillEnemy;

        SetActive(portalObject, true);

        tutorialHUD?.MostrarMensajePortal();

        Debug.Log("[TutorialManager] Enemigo eliminado — Portal activado");
        TryAdvanceStep();
    }

    public void OnPortalEntered()
    {
        _portalEntered = true;
        Debug.Log("[TutorialManager] Portal entrado");
        TryAdvanceStep();
    }

    private void TryAdvanceStep()
    {
        while (CanCompleteCurrentStep())
            _currentStep++;

        int hudStep = Mathf.Clamp((int)_currentStep, 0, 3);
        tutorialHUD?.SetStep(hudStep);

        if (_currentStep >= TutorialStep.EnterPortal)
            tutorialHUD?.MostrarPanelFinal();

        Debug.Log($"[TutorialManager] Paso actual: {_currentStep}");
    }

    private bool CanCompleteCurrentStep()
    {
        switch (_currentStep)
        {
            case TutorialStep.Move:        return _hasMoved;
            case TutorialStep.Jump:        return _hasJumped;
            case TutorialStep.Attack:      return _hasAttacked;
            case TutorialStep.KillEnemy:   return _enemyKilled;
            case TutorialStep.EnterPortal: return _portalEntered;
            default:                       return false;
        }
    }

    private void SetActive(GameObject target, bool value)
    {
        if (target != null)
            target.SetActive(value);
    }
}
