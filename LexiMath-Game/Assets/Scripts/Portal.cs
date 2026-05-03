using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Header("Escena destino")]
    [SerializeField] private string sceneToLoad = "MainMenu";

    [Header("Referencias")]
    [SerializeField] private TutorialManager tutorialManager;

    private void Awake()
    {
        if (tutorialManager == null)
            tutorialManager = Object.FindFirstObjectByType<TutorialManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        tutorialManager?.OnPortalEntered();

        if (GameManager.Instance != null)
            GameManager.Instance.SetTutorialCompletado();

        SceneManager.LoadScene(sceneToLoad);
    }
}
