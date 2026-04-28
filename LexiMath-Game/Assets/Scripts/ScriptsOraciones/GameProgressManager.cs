// Assets/Scripts/Game/GameProgressManager.cs
using UnityEngine;
using TMPro;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;
    
    [Header("Progress")]
    public int totalCollectibles = 3;
    public int collected = 0;
    
    [Header("UI")]
    public TextMeshProUGUI progressText; // muestra "Pergaminos: 1/3"
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        UpdateUI();
    }
    
    public void RegisterCollected()
    {
        collected++;
        UpdateUI();
        Debug.Log($"Progreso: {collected}/{totalCollectibles}");
        
        if (collected >= totalCollectibles)
        {
            Debug.Log("¡Todos los pergaminos recolectados! La puerta final está abierta.");
            if (GameUIManager.Instance != null)
                GameUIManager.Instance.ShowVictoryHint();
        }
    }
    
    public bool IsComplete()
    {
        return collected >= totalCollectibles;
    }
    
    private void UpdateUI()
    {
        if (progressText != null)
            progressText.text = $"Ojos: {collected}/{totalCollectibles}";
    }
}