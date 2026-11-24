
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController1 : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 3f;
    public float dashSpeed = 7f;
    public float gravity = -9.8f;

    [Header("ダッシュ設定")]
    public float dashDelay = 1f;

    private float moveHoldTimer = 0f;
    private bool isDashing = false;

    [Header("視点リセット設定")]
    public KeyCode resetViewKey = KeyCode.LeftAlt;
    private Quaternion initialPlayerRotation;
    private Quaternion initialCameraRotation;

    [Header("ジャンプ設定")]
    public float jumpHeight = 2f;
    public string jumpAnimation = "Jump";

    private bool isJumping = false;

    [Header("カメラ設定")]
    public Transform cameraRoot;
    public float mouseSensitivity = 2f;
    public float minCameraAngle = -60f;
    public float maxCameraAngle = 60f;

    [Header("アニメーション")]
    public Animator animator;

    [Header("待機アニメーション")]
    public string[] idleAnimations;
    public float idleWaitTime = 5f;

    private CharacterController controller;

    private float cameraRotationX = 0f;
    private float verticalVelocity = 0f;
    private float idleTimer = 0f;
    private int currentIdleIndex = -1;

    private Vector2 movementInput;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        // ゲーム開始時のプレイヤーとカメラの角度を保存
        initialPlayerRotation = transform.rotation;

        if (cameraRoot != null)
        {
            initialCameraRotation = cameraRoot.localRotation;
        }

    }

    void Update()
    {
        CameraRotation();

        PlayerMovement();

        Jump();

        IdleAnimation();

        if (Input.GetKeyDown(resetViewKey))
        {
            ResetView();
        }

    }

    void ResetView()
    {
        // プレイヤーの向きを初期角度に戻す
        transform.rotation = initialPlayerRotation;

        // カメラの角度を初期角度に戻す
        if (cameraRoot != null)
        {
            cameraRoot.localRotation = initialCameraRotation;
        }
    }

    // カメラとプレイヤーの回転
    void CameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        cameraRotationX -= mouseY;

        cameraRotationX = Mathf.Clamp(
            cameraRotationX,
            minCameraAngle,
            maxCameraAngle
        );

        if (cameraRoot != null)
        {
            cameraRoot.localRotation =
                Quaternion.Euler(cameraRotationX, 0f, 0f);
        }
    }

    // 移動とダッシュ
    void PlayerMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        movementInput = new Vector2(horizontal, vertical);

        bool isMoving = movementInput.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            moveHoldTimer += Time.deltaTime;
        }
        else
        {
            moveHoldTimer = 0f;
        }

        isDashing = isMoving && moveHoldTimer >= dashDelay;

        float currentSpeed = isDashing ? dashSpeed : moveSpeed;

        Vector3 move =
            transform.right * horizontal +
            transform.forward * vertical;

        move = Vector3.ClampMagnitude(move, 1f);

        // 重力
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * currentSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);

        // ジャンプ中は移動アニメーションを更新しない
        if (animator != null && !isJumping)
        {
            animator.SetFloat("Horizontal", horizontal);
            animator.SetFloat("Vertical", vertical);

            animator.SetFloat(
                "Speed",
                isMoving ? currentSpeed : 0f
            );

            animator.SetBool("IsMoving", isMoving);
            animator.SetBool("IsDashing", isDashing);
        }
    }

    // ジャンプ
    void Jump()
    {
        if (controller.isGrounded)
        {
            // 着地したとき
            if (isJumping)
            {
                isJumping = false;

                if (animator != null)
                {
                    animator.SetBool("IsJumping", false);

                    // 着地後、移動状態を再設定
                    UpdateMovementAnimation();
                }
            }

            // SPACEキーでジャンプ
            if (Input.GetKeyDown(KeyCode.Space))
            {
                verticalVelocity = Mathf.Sqrt(
                    jumpHeight * -2f * gravity
                );

                isJumping = true;
                idleTimer = 0f;

                if (animator != null)
                {
                    animator.SetBool("IsJumping", true);

                    // 移動アニメーションからジャンプへ切り替え
                    animator.CrossFade(
                        jumpAnimation,
                        0.1f
                    );
                }
            }
        }
    }

    // 着地後の移動アニメーションを更新
    void UpdateMovementAnimation()
    {
        float horizontal = movementInput.x;
        float vertical = movementInput.y;

        bool isMoving = movementInput.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            moveHoldTimer = 0f;
        }

        isDashing = isMoving && moveHoldTimer >= dashDelay;

        float currentSpeed = isDashing ? dashSpeed : moveSpeed;

        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);

        animator.SetFloat(
            "Speed",
            isMoving ? currentSpeed : 0f
        );

        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsDashing", isDashing);
    }

    // 待機アニメーション
    void IdleAnimation()
    {
        if (animator == null ||
            idleAnimations == null ||
            idleAnimations.Length == 0)
        {
            return;
        }

        // ジャンプ中は待機アニメーションを停止
        if (isJumping)
        {
            idleTimer = 0f;
            return;
        }

        bool isMoving = movementInput.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            idleTimer = 0f;
            return;
        }

        idleTimer += Time.deltaTime;

        if (idleTimer >= idleWaitTime)
        {
            idleTimer = 0f;

            int randomIndex = Random.Range(
                0,
                idleAnimations.Length
            );

            if (idleAnimations.Length > 1 &&
                randomIndex == currentIdleIndex)
            {
                randomIndex =
                    (randomIndex + 1) % idleAnimations.Length;
            }

            currentIdleIndex = randomIndex;

            animator.CrossFade(
                idleAnimations[randomIndex],
                0.25f
            );
        }
    }
}
