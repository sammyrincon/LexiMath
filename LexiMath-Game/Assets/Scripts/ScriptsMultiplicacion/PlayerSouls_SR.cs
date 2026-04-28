using UnityEngine;
using TMPro;

public class PlayerSouls : MonoBehaviour
{
    public static PlayerSouls Instance;
    
    [Header("Souls")]
    public int currentSouls = 0;
    public TextMeshProUGUI soulsText;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        UpdateUI();
    }
    
    public void AddSouls(int amount)
    {
        currentSouls += amount;
        UpdateUI();
        Debug.Log($"+{amount} almas! Total: {currentSouls}");
    }
    
    private void UpdateUI()
    {
        if (soulsText != null)
            soulsText.text = $"Almas: {currentSouls}";
    }
}