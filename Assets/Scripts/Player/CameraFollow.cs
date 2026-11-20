
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
        if (cameraRoot == null) return;

        // 初期角度を保存
        Vector3 angles = cameraRoot.eulerAngles;

        rotationY = angles.y;
        rotationX = angles.x;

        initialRotationX = rotationX;
        initialRotationY = rotationY;

        // マウスカーソルを固定
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null || cameraRoot == null) return;

        // ALTキーで視点を初期位置に戻す
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

        // 回転を反映
        cameraRoot.rotation = Quaternion.Euler(
            rotationX,
            rotationY,
            0f
        );

        // カメラ位置を計算
        Vector3 desiredPosition =
            target.position + cameraRoot.rotation * offset;

        // なめらかに追従
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
}
