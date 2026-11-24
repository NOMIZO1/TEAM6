
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("追従対象")]
    public Transform target;

    [Header("回転用オブジェクト")]
    public Transform cameraRoot;

    [Header("カメラ設定")]
    public Vector3 offset = new Vector3(0, 5, -5);
    public float followSpeed = 5f;

    [Header("初期角度設定")]
    public float initialAngleX = 20f;
    public float initialAngleY = 0f;

    [Header("回転設定")]
    public float mouseSensitivity = 200f;
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 60f;

    [Header("視点リセット")]
    public KeyCode resetViewKey = KeyCode.LeftAlt;

    private float rotationX;
    private float rotationY;

    private float initialRotationX;
    private float initialRotationY;

    void Start()
    {
        if (cameraRoot == null || target == null) return;

        rotationX = initialAngleX;
        rotationY = initialAngleY;

        initialRotationX = initialAngleX;
        initialRotationY = initialAngleY;

        // カメラの初期角度
        cameraRoot.rotation = Quaternion.Euler(
            rotationX,
            rotationY,
            0f
        );

        // プレイヤーの初期左右角度も合わせる
        target.rotation = Quaternion.Euler(
            0f,
            rotationY,
            0f
        );

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null || cameraRoot == null) return;

        // ALTキーで視点をリセット
        if (Input.GetKeyDown(resetViewKey))
        {
            rotationX = initialRotationX;
            rotationY = initialRotationY;
        }

        // マウス入力
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotationY += mouseX * mouseSensitivity * Time.deltaTime;
        rotationX -= mouseY * mouseSensitivity * Time.deltaTime;

        // 上下の回転制限
        rotationX = Mathf.Clamp(
            rotationX,
            minVerticalAngle,
            maxVerticalAngle
        );

        // カメラの回転
        cameraRoot.rotation = Quaternion.Euler(
            rotationX,
            rotationY,
            0f
        );

        // プレイヤーもカメラの左右角度に合わせる
        target.rotation = Quaternion.Euler(
            0f,
            rotationY,
            0f
        );

        // カメラ位置
        Vector3 desiredPosition =
            target.position + cameraRoot.rotation * offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            Time.deltaTime * followSpeed
        );

        // プレイヤーの少し上を見る
        transform.LookAt(
            target.position + Vector3.up * 1.5f
        );
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
