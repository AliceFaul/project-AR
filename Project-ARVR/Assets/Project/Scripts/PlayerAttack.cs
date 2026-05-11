using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator;

    // Hàm này được GameManager gọi khi nhấn nút Attack
    public void Attack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
            Debug.Log("Đánh liên tục không cooldown!");
        }
        else
        {
            Debug.LogWarning("Vui lòng kéo thả Animator vào Script PlayerAttack trên Inspector!");
        }
    }
}