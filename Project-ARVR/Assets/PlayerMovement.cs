using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Input Settings")]
    public Joystick joystick;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationTurnTime = 0.1f; // Thời gian để xoay nhân vật
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;
    private Transform cameraTransform;
    private float rotationVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;

        if (joystick == null)
        {
            Debug.LogError("Chưa kéo thả Joystick vào PlayerController");
        }
    }

    void Update()
    {
        MovePlayer();
        ApplyGravity();
    }

    void MovePlayer()
    {
        // Lấy dữ liệu trực tiếp từ Joystick
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        // Kiểm tra Joystick có đang được kéo đủ mạnh không (tránh bị nhiễu)
        if (moveDirection.magnitude >= 0.1f)
        {
            // Tính toán góc xoay dựa trên hướng Joystick và hướng Camera
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

            // Xoay nhân vật
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationTurnTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Tính hướng đi thực tế sau khi đã cộng góc xoay của Camera
            Vector3 targetMoveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // Di chuyển với vận tốc tỉ lệ thuận với độ kéo của Joystick
            float currentSpeed = moveSpeed * moveDirection.magnitude;
            controller.Move(targetMoveDir * currentSpeed * Time.deltaTime);
        }
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}