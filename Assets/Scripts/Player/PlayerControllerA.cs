
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerA : MonoBehaviour
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
    [Header("視点リセット設定")]
    public KeyCode resetViewKey = KeyCode.LeftAlt;

    // Inspectorで指定するプレイヤーの初期角度
    public Vector3 initialPlayerAngle = new Vector3(0f, 0f, 0f);

    [Header("ジャンプ設定")]
    public float jumpHeight = 2f;
    public string jumpAnimation = "Jump";

    private bool isJumping = false;

    [Header("カメラ設定")]
    public Transform cameraRoot;
    public CameraFollow cameraFollow; [Header("アニメーション")]
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

        // プレイヤーの初期角度を保存
        // Inspectorで指定した初期角度を適用
        // プレイヤーの初期角度を適用
        transform.rotation = Quaternion.Euler(initialPlayerAngle);

        if (cameraFollow != null)
        {
            cameraFollow.ResetCameraToPlayerAngle(initialPlayerAngle.y);
        }

        // カーソルを固定
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        // ALTキーを押している間は移動を無効化
        bool isAltHolding = Input.GetKey(resetViewKey);

        // ALTキーを押した瞬間に初期角度へ戻す
        if (Input.GetKeyDown(resetViewKey))
        {
            ResetPlayerRotation();
        }

        // ALTキーを押している間は移動・ジャンプを停止
        if (isAltHolding)
        {
            movementInput = Vector2.zero;
            moveHoldTimer = 0f;
            isDashing = false;
            verticalVelocity = 0f;

            if (animator != null)
            {
                animator.SetFloat("Horizontal", 0f);
                animator.SetFloat("Vertical", 0f);
                animator.SetFloat("Speed", 0f);
                animator.SetBool("IsMoving", false);
                animator.SetBool("IsDashing", false);
            }

            return;
        }

        PlayerMovement();
        Jump();
        IdleAnimation();
    }
    void ResetPlayerRotation()
    {
        transform.rotation = Quaternion.Euler(initialPlayerAngle);
    }
    // カメラとプレイヤーの回転
    // カメラとプレイヤーの回転
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
