using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string sceneToLoad = "LoginScene";
    [SerializeField] private TutorialManager tutorialManager;

    private void Awake()
    {
        if (tutorialManager == null)
        {
            tutorialManager = FindObjectOfType<TutorialManager>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        tutorialManager?.OnPortalEntered();
        SceneManager.LoadScene(sceneToLoad);
    }
}
