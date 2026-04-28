// Assets/Scripts/World/FinalDoor.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalDoor : MonoBehaviour
{
    [Header("Settings")]
    public string nextSceneName = ""; // si vacío, solo muestra mensaje de victoria
    public KeyCode interactKey = KeyCode.E;
    public GameObject interactPrompt;
    public GameObject lockedPrompt; // texto "Necesitas 3 pergaminos"
    
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
            }
            else
            {
                if (lockedPrompt != null) lockedPrompt.SetActive(true);
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
        }
    }
    
    private void Win()
    {
        Debug.Log("¡VICTORIA! Has completado el calabozo.");
        if (GameUIManager.Instance != null)
            GameUIManager.Instance.ShowFinalVictory();
        else

        {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
            // Mostrar UI de victoria, pausar, etc.
            Time.timeScale = 0f;
        }
    }
}