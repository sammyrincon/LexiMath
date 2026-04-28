// Assets/Scripts/Player/PlayerLives.cs
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerLives : MonoBehaviour
{
    public static PlayerLives Instance;
    
    [Header("Lives")]
    public int maxLives = 3;
    public int currentLives;
    public TextMeshProUGUI livesText;
    
    [Header("Respawn")]
    public Transform respawnPoint; // punto inicial del nivel
    public float invulnerabilityTime = 1.5f;
    
    private bool isInvulnerable = false;
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        currentLives = maxLives;
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateUI();
    }
    
    public void LoseLife()
    {
        if (isInvulnerable) return;
        
        currentLives--;
        UpdateUI();
        Debug.Log($"¡Vida perdida! Vidas restantes: {currentLives}");
        
        if (currentLives <= 0)
        {
            GameOver();
        }
        else
        {
            Respawn();
        }
    }
    
    private void Respawn()
    {
        if (respawnPoint != null)
            transform.position = respawnPoint.position;
        
        StartCoroutine(InvulnerabilityRoutine());
    }
    
    private System.Collections.IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;
        
        // Parpadeo visual
        float elapsed = 0f;
        while (elapsed < invulnerabilityTime)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.4f);
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.2f;
        }
        
        spriteRenderer.color = Color.white;
        isInvulnerable = false;
    }
    
    private void GameOver()
    {
        Debug.Log("Game Over");
        if (GameUIManager.Instance != null)
            GameUIManager.Instance.ShowGameOver();
        else
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    
    private void UpdateUI()
    {
        if (livesText != null)
            livesText.text = $"Vidas: {currentLives}";
    }
}