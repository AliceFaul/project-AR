using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;
    public Image healthBarUI; // UI thanh máu
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI(); // Cập nhật UI khi bắt đầu game
    }

    public void TakeDamage(float damage) {
        currentHealth -= (int)damage;
        UpdateHealthUI(); // Cập nhật UI sau khi bị thương
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if(currentHealth <= 0) {
            Die();
        }
    }

    private void UpdateHealthUI() { 
        healthBarUI.fillAmount = (float)currentHealth / maxHealth;
    }

    void Die()
    {
        Debug.Log("Player chết!");
        // TODO: Game Over
        GameManager.Instance.GameOver();
    }
}
