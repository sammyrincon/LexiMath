using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalDoor : MonoBehaviour
{
    [Header("Settings")]
    public string nextSceneName = "";
    public KeyCode interactKey = KeyCode.E;
    public GameObject interactPrompt;
    public GameObject lockedPrompt;

    private bool playerInRange = false;

    private void Awake()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (lockedPrompt != null) lockedPrompt.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            if (GameProgressManager.Instance.IsComplete())
            {
                Win();
            }
            else
            {
                Debug.Log("La puerta está sellada. Necesitas los 3 pergaminos.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (GameProgressManager.Instance.IsComplete())
            {
                if (interactPrompt != null) interactPrompt.SetActive(true);
                OracionesHUDController.Instance?.ShowPrompt("Presiona  [ E ]  para entrar");
            }
            else
            {
                if (lockedPrompt != null) lockedPrompt.SetActive(true);
                OracionesHUDController.Instance?.ShowPrompt("Necesitas los 3 pergaminos");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactPrompt != null) interactPrompt.SetActive(false);
            if (lockedPrompt != null) lockedPrompt.SetActive(false);
            OracionesHUDController.Instance?.HidePrompt();
        }
    }

    private void Win()
    {
        Debug.Log("¡VICTORIA! Has completado el calabozo.");
        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.ShowFinalVictory();
            return;
        }

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
        else
            Time.timeScale = 0f;
    }
}
