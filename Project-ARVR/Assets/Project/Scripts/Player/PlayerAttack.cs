using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private int damage = 25;
    [SerializeField] private float attackRadius = 1.5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackCooldown = 0.5f;

    [Header("Sword Slash")]
    [SerializeField] private GameObject playerSword;
    [SerializeField] private GameObject swordSlashPrefab;
    
    public Animator animator;
    private bool isAttacking = false;

    // Hàm này được GameManager gọi khi nhấn nút Attack
    public void Attack()
    {
        if (isAttacking) return;
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        // Kích hoạt hiệu ứng Sword Slash
        var swordSlash = Instantiate(swordSlashPrefab);
        if(swordSlash != null && playerSword != null) {
            swordSlash.transform.position = playerSword.transform.position; // Đặt vị trí của hiệu ứng tại thanh kiếm
            swordSlash.transform.rotation = playerSword.transform.rotation;
            swordSlash.SetActive(true);
        }
        yield return new WaitForSeconds(0.1f); // Delay để đồng bộ với animation, điều chỉnh nếu cần
        OnAttackHit(); // Gọi hàm kiểm tra va chạm sau khi animation đã bắt đầu
        Destroy(swordSlash, 0.5f); // Hủy hiệu ứng sau khi đã hiển thị
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    private void OnAttackHit() {
        Collider[] hitEnemies =
            Physics.OverlapSphere(
                attackPoint.position,
                attackRadius,
                enemyLayer);

        foreach (Collider enemy in hitEnemies) {
            IDamageable enemyHealth = enemy.GetComponent<IDamageable>();

            if (enemyHealth != null) {
                enemyHealth.TakeDamage(damage);
                Debug.Log("Enemy take damage");
            }
        }
    }

    private void OnDrawGizmosSelected() {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRadius);
    }
}