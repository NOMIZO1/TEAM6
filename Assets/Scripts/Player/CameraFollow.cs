
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

    private float rotationX;
    private float rotationY;

    // 前回のプレイヤー角度
    private float previousPlayerAngleY;

    void Start()
    {
        if (cameraRoot == null || target == null) return;

        rotationX = initialAngleX;
        rotationY = initialAngleY;

        target.rotation = Quaternion.Euler(
            0f,
            rotationY,
            0f
        );

        previousPlayerAngleY = target.eulerAngles.y;

        cameraRoot.rotation = Quaternion.Euler(
            rotationX,
            rotationY,
            0f
        );

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null || cameraRoot == null) return;

        // プレイヤーの現在のY軸角度を取得
        rotationY = target.eulerAngles.y;

        // マウス操作で上下の角度だけ変更
        float mouseY = Input.GetAxis("Mouse Y");

        rotationX -= mouseY * mouseSensitivity * Time.deltaTime;

        // 上下の回転制限
        rotationX = Mathf.Clamp(
            rotationX,
            minVerticalAngle,
            maxVerticalAngle
        );

        // カメラをプレイヤーの向きに合わせる
        cameraRoot.rotation = Quaternion.Euler(
            rotationX,
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
    // PlayerControllerAから呼び出す
    public void ResetCameraToPlayerAngle(float playerAngleY)
    {
        rotationY = playerAngleY;
        rotationX = initialAngleX;

        previousPlayerAngleY = playerAngleY;

        cameraRoot.rotation = Quaternion.Euler(
            rotationX,
            rotationY,
            0f
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
