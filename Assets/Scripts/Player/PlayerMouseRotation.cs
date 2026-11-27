
using UnityEngine;

public class PlayerMouseRotation : MonoBehaviour
{
    [Header("回転対象")]
    public Transform player;
    public Transform mainCamera;

    [Header("マウス回転設定")]
    public float mouseSensitivity = 200f;

    [Header("回転の有効・無効")]
    public bool rotationEnabled = true;

    private float rotationY;

    void Start()
    {
        if (player == null)
        {
            player = transform;
        }

        if (mainCamera == null && Camera.main != null)
        {
            mainCamera = Camera.main.transform;
        }

        // Playerの現在の角度を取得
        rotationY = player.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!rotationEnabled) return;

        if (player == null || mainCamera == null) return;

        // マウスの左右移動量
        float mouseX = Input.GetAxis("Mouse X");

        // 回転角度を計算
        rotationY += mouseX * mouseSensitivity * Time.deltaTime;

        // Playerを左右回転
        player.rotation = Quaternion.Euler(
            0f,
            rotationY,
            0f
        );

        // メインカメラも同じ左右角度にする
        Vector3 cameraEuler = mainCamera.eulerAngles;

        mainCamera.rotation = Quaternion.Euler(
            cameraEuler.x,
            rotationY,
            cameraEuler.z
        );
    }
}
