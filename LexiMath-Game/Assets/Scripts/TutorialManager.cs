using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI[] checklistLabels;

    [Header("References")]
    public PlayerController playerController;
    public PlayerMelee playerMelee;
    public EnemyBasic enemy;

    private int currentStep = 0;
    private bool[] completed = new bool[4];
    private string[] stepNames = { "MOVER", "SALTAR", "ATACAR", "DERROTAR" };
    private string[] instructions = {
        "← → Mueve al caballero izquierda y derecha",
        "↑ o ESPACIO para saltar",
        "X para atacar con la espada",
        "¡Acércate al enemigo y derrótalo!"
    };

    private bool hasMoved = false;
    private bool hasJumped = false;
    private bool hasAttacked = false;

    void Start()
    {
        for (int i = 0; i < checklistLabels.Length; i++)
        {
            checklistLabels[i].text = stepNames[i];
            checklistLabels[i].color = Color.white;
        }
        ShowStep(0);
    }

    void Update()
    {
        if (currentStep == 0 && !hasMoved)
        {
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f)
            {
                hasMoved = true;
                CompleteStep(0);
                ShowStep(1);
            }
        }
        else if (currentStep == 1 && !hasJumped)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                hasJumped = true;
                CompleteStep(1);
                ShowStep(2);
            }
        }
        else if (currentStep == 2 && !hasAttacked)
        {
            if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Z))
            {
                hasAttacked = true;
                CompleteStep(2);
                ShowStep(3);
            }
        }

        if (currentStep == 3 && enemy == null)
        {
            CompleteStep(3);
            dialogueText.text = "¡Perfecto! Has derrotado al enemigo";
            Invoke(nameof(FinishTutorial), 3f);
        }
    }

    void ShowStep(int step)
    {
        currentStep = step;
        dialogueText.text = instructions[step];
    }

    void CompleteStep(int index)
    {
        completed[index] = true;
        checklistLabels[index].text = "✓ " + stepNames[index];
        checklistLabels[index].color = Color.green;
    }

    void FinishTutorial()
    {
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        dialogueText.text = "¡Tutorial completado!";
    }
}