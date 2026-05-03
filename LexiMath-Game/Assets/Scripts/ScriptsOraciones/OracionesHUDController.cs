using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIDocument))]
public class OracionesHUDController : MonoBehaviour
{
    public static OracionesHUDController Instance { get; private set; }
    public VisualElement Root { get; private set; }

    private Label livesLabel;
    private Label scrollsLabel;
    private Label interactPrompt;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void OnEnable()
    {
        Root = GetComponent<UIDocument>().rootVisualElement;

        livesLabel     = Root.Q<Label>("lives-label");
        scrollsLabel   = Root.Q<Label>("scrolls-label");
        interactPrompt = Root.Q<Label>("interact-prompt");

        var btnPausa = Root.Q<Button>("Button"); // nombre exacto del UXML
        if (btnPausa != null)
            btnPausa.clicked += () => BookPauseController.Instance?.IntentarCambiarPausa();
        else
            Debug.LogWarning("btn pausa no encontrado — revisa el nombre en el UXML");
    }

    void Start()
    {
        if (PlayerLives.Instance != null)
            SetLives(PlayerLives.Instance.currentLives);
        if (GameProgressManager.Instance != null)
            SetScrolls(GameProgressManager.Instance.collected, GameProgressManager.Instance.totalCollectibles);
    }

    public void SetLives(int current)
    {
        if (livesLabel != null)
            livesLabel.text = $" {current}";
    }

    public void SetScrolls(int collected, int total)
    {
        if (scrollsLabel != null)
            scrollsLabel.text = $" {collected} / {total}";
    }

    public void ShowPrompt(string text = "Presiona  [ E ]")
    {
        if (interactPrompt == null) return;
        interactPrompt.text = text;
        interactPrompt.style.display = DisplayStyle.Flex;
    }

    public void HidePrompt()
    {
        if (interactPrompt != null)
            interactPrompt.style.display = DisplayStyle.None;
    }
}
