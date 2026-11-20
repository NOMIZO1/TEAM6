
using UnityEngine;

public class FirstPersonCameraController : MonoBehaviour
{
    [Header("カメラ設定")]
    [SerializeField] private Transform player;
    [SerializeField] private float mouseSensitivity = 200f;

    [Header("カメラ初期角度")]
    [SerializeField] private Vector2 initialCameraAngle = new Vector2(10f, 0f);

    [Header("カメラ位置調整")]
    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 1.6f, 0.2f);

    [Header("上下の視点制限")]
    [SerializeField] private float minAngle = -80f;
    [SerializeField] private float maxAngle = 80f;

    private float rotationX;
    private float rotationY;

    private bool isActive = false;

    void Start()
    {
        ResetCameraAngle();
        UpdateCameraPosition();
    }

    void LateUpdate()
    {
        if (player == null) return;

        UpdateCameraPosition();

        if (!isActive) return;

        // マウス操作
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 左右の回転
        rotationY += mouseX * mouseSensitivity * Time.deltaTime;

        // 上下の回転
        rotationX -= mouseY * mouseSensitivity * Time.deltaTime;

        rotationX = Mathf.Clamp(rotationX, minAngle, maxAngle);

        // Playerとカメラの回転を更新
        ApplyCameraRotation();
    }

    // 初期角度を設定
    void ResetCameraAngle()
    {
        rotationX = initialCameraAngle.x;

        rotationY = player != null
            ? player.eulerAngles.y
            : initialCameraAngle.y;

        ApplyCameraRotation();
    }

    // カメラとPlayerの回転を反映
    void ApplyCameraRotation()
    {
        // Playerは左右だけ回転
        player.rotation = Quaternion.Euler(
            0f,
            rotationY,
            0f
        );

        // カメラは上下左右に回転
        transform.rotation = Quaternion.Euler(
            rotationX,
            rotationY,
            0f
        );
    }

    // カメラ位置を更新
    void UpdateCameraPosition()
    {
        transform.position = player.position + cameraOffset;
    }

    // カメラ切り替え
    public void SetActiveCamera(bool active)
    {
        isActive = active;

        if (active)
        {
            ResetCameraAngle();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
