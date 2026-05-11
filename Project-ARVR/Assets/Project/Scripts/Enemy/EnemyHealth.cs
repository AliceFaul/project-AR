using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamageable {
    public int maxHealth = 50;
    private int currentHealth;
    public Image healthBarUI; // UI thanh máu

    void Start() {
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

    void Die() {
        Debug.Log("Enemy chết!");
        GameManager.Instance.AdjustScore(10); // Tăng điểm khi kẻ địch chết
        GameManager.Instance.AdjustEnemyLeft(-1); // Giảm số lượng kẻ địch còn lại
        Destroy(gameObject); // Hủy đối tượng kẻ địch
        GameManager.Instance.RemoveEnemies(); // Gọi hàm để xóa kẻ địch khỏi danh sách quản lý trong GameManager
    }
}
