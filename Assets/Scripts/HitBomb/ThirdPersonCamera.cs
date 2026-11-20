using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("追従対象")]
    [SerializeField] private Transform target;

    [Header("カメラ設定")]
    [SerializeField] private float distance = 5f;
    [SerializeField] private float height = 1.5f;
    [SerializeField] private float offset = 1.5f;
    [SerializeField] private float mouseSensitivity = 3f;

    [Header("視点制限")]
    [SerializeField] private float minPitch = -20f;
    [SerializeField] private float maxPitch = 60f;

    private float yaw;
    private float pitch = 15f;
    // Start is called before the first frame update
    void Start()
    {
        if (target == null)
        {
            Debug.LogError("追従対象が設定されていません！");
            enabled = false;
            return;
        }

        yaw = target.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        // マウス入力
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // カメラの回転
        Quaternion rotation =
            Quaternion.Euler(pitch, yaw, 0);

        // 注視位置
        Vector3 targetPosition =
            target.position + Vector3.up * height;

        // カメラの配置
        Vector3 cameraPosition =
            targetPosition -
            rotation * Vector3.forward * distance
            + rotation * Vector3.right * offset;

        transform.position = cameraPosition;
        transform.rotation = rotation;
    }
}
