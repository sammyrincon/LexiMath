using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [Header("Panels")]
    public GameObject gameOverPanel;
    public GameObject victoryHintPanel;
    public GameObject finalVictoryPanel;

    [Header("Hint Settings")]
    public float hintDuration = 3f;

    private bool gameOverActive = false;
    private bool finalVictoryActive = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryHintPanel != null) victoryHintPanel.SetActive(false);
        if (finalVictoryPanel != null) finalVictoryPanel.SetActive(false);
    }

    private void Update()
    {
        if ((gameOverActive || finalVictoryActive) && Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
    }

    public void ShowGameOver()
    {
        gameOverActive = true;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ShowVictoryHint()
    {
        if (victoryHintPanel != null)
        {
            victoryHintPanel.SetActive(true);
            CancelInvoke(nameof(HideVictoryHint));
            Invoke(nameof(HideVictoryHint), hintDuration);
        }
    }

    private void HideVictoryHint()
    {
        if (victoryHintPanel != null) victoryHintPanel.SetActive(false);
    }

    public void ShowFinalVictory()
    {
        finalVictoryActive = true;
        if (finalVictoryPanel != null) finalVictoryPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
