using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth_SR : MonoBehaviour
{
    public static PlayerHealth_SR Instance;
    
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthBar;
    
    private PlayerControler_SR playerController;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        currentHealth = maxHealth;
        playerController = GetComponent<PlayerControler_SR>();
        UpdateUI();
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        
        if (playerController != null)
            playerController.TakeDamage();
        
        UpdateUI();
        
        if (currentHealth <= 0)
            Die();
    }
    
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateUI();
    }
    
    private void Die()
    {
        Debug.Log("Game Over");
    }
    
    private void UpdateUI()
    {
        if (healthBar != null)
            healthBar.value = (float)currentHealth / maxHealth;
    }
}