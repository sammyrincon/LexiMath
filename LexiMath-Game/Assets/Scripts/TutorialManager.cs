using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private enum TutorialStep
    {
        Move = 0,
        Jump = 1,
        Attack = 2,
        KillEnemy = 3,
        EnterPortal = 4,
        Completed = 5
    }

    [Header("Gameplay")]
    [SerializeField] private GameObject portalObject;

    [Header("Move UI")]
    [SerializeField] private GameObject keysRow;
    [SerializeField] private GameObject actionTextMover;

    [Header("Jump UI")]
    [SerializeField] private GameObject keySlot3;
    [SerializeField] private GameObject actionTextSaltar;

    [Header("Attack UI")]
    [SerializeField] private GameObject keySlot4;
    [SerializeField] private GameObject actionTextAtacar;

    [Header("Optional UI")]
    [SerializeField] private GameObject actionTextEnemy;
    [SerializeField] private GameObject actionTextPortal;
    [SerializeField] private GameObject actionTextCompletado;

    private TutorialStep currentStep = TutorialStep.Move;

    private bool hasMoved;
    private bool hasJumped;
    private bool hasAttacked;
    private bool enemyKilled;
    private bool portalEntered;

    private void Start()
    {
        if (portalObject != null)
        {
            portalObject.SetActive(false);
        }

        ShowCurrentStep();
    }

    public void OnPlayerMove()
    {
        hasMoved = true;
        TryAdvanceStep();
    }

    public void OnPlayerJump()
    {
        hasJumped = true;
        TryAdvanceStep();
    }

    public void OnPlayerAttack()
    {
        hasAttacked = true;
        TryAdvanceStep();
    }

    public void OnEnemyKilled()
    {
        hasAttacked = true;
        enemyKilled = true;

        if (currentStep < TutorialStep.KillEnemy)
        {
            currentStep = TutorialStep.KillEnemy;
        }

        if (portalObject != null)
        {
            portalObject.SetActive(true);
        }

        TryAdvanceStep();
    }

    public void OnPortalEntered()
    {
        portalEntered = true;
        TryAdvanceStep();
    }

    private void TryAdvanceStep()
    {
        while (CanCompleteCurrentStep())
        {
            currentStep++;
        }

        ShowCurrentStep();
    }

    private bool CanCompleteCurrentStep()
    {
        switch (currentStep)
        {
            case TutorialStep.Move:
                return hasMoved;

            case TutorialStep.Jump:
                return hasJumped;

            case TutorialStep.Attack:
                return hasAttacked;

            case TutorialStep.KillEnemy:
                return enemyKilled;

            case TutorialStep.EnterPortal:
                return portalEntered;

            default:
                return false;
        }
    }

    private void ShowCurrentStep()
    {
        HideAllPrompts();

        switch (currentStep)
        {
            case TutorialStep.Move:
                SetActive(keysRow, true);
                SetActive(actionTextMover, true);
                break;

            case TutorialStep.Jump:
                SetActive(keySlot3, true);
                SetActive(actionTextSaltar, true);
                break;

            case TutorialStep.Attack:
                SetActive(keySlot4, true);
                SetActive(actionTextAtacar, true);
                break;

            case TutorialStep.KillEnemy:
                SetActive(actionTextEnemy, true);
                break;

            case TutorialStep.EnterPortal:
                SetActive(actionTextPortal, true);
                break;

            case TutorialStep.Completed:
                SetActive(actionTextCompletado, true);
                break;
        }
    }

    private void HideAllPrompts()
    {
        SetActive(keysRow, false);
        SetActive(actionTextMover, false);

        SetActive(keySlot3, false);
        SetActive(actionTextSaltar, false);

        SetActive(keySlot4, false);
        SetActive(actionTextAtacar, false);

        SetActive(actionTextEnemy, false);
        SetActive(actionTextPortal, false);
        SetActive(actionTextCompletado, false);
    }

    private void SetActive(GameObject target, bool value)
    {
        if (target != null)
        {
            target.SetActive(value);
        }
    }
}
