using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float gravity = -20f;

    [Header("ジャンプ設定")]
    [SerializeField] private float jumpPower = 8f;

    [Header("吹っ飛ばし設定")]
    [SerializeField] private float knockbackDamping = 5f;

    private CharacterController controller;
    private Camera playerCamera;

    private float verticalVelocity;
    private Vector3 knockbackVelocity;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        if (playerCamera == null) return;

        // キーボード入力
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // カメラの向きを基準にする
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection =
            cameraForward * vertical +
            cameraRight * horizontal;

        moveDirection = Vector3.ClampMagnitude(
            moveDirection, 1f
        );

        // プレイヤーの向きを変更
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // 重力
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        // ジャンプ
        if (controller.isGrounded &&
            Input.GetKeyDown(KeyCode.Space))
        {
            verticalVelocity = jumpPower;
        }

        // 吹っ飛ばしの減衰
        knockbackVelocity = Vector3.Lerp(
            knockbackVelocity,
            Vector3.zero,
            knockbackDamping * Time.deltaTime
        );

        // 通常移動 + 吹っ飛ばし + 重力
        Vector3 velocity =
            moveDirection * moveSpeed +
            knockbackVelocity +
            Vector3.up * verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }

    // 爆発などによる吹っ飛ばし
    public void ApplyKnockback(
        Vector3 explosionPosition,
        float force,
        float upwardForce)
    {
        Vector3 direction =
            transform.position - explosionPosition;

        direction.y = 0;

        if (direction.sqrMagnitude < 0.001f)
        {
            direction = -transform.forward;
        }

        direction.Normalize();

        knockbackVelocity =
            direction * force +
            Vector3.up * upwardForce;

        // 上方向の速度も反映
        verticalVelocity = 0f;

        Debug.Log("プレイヤーが吹っ飛ばされました！");
    }
}
