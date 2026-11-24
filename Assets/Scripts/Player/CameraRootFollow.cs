using UnityEngine;

public class CameraRootFollow : MonoBehaviour
{
    public Transform target; // Player

    void LateUpdate()
    {
        if (target == null) return;

        // 🔥 位置だけ追従（回転は触らない！）
        transform.position = target.position;
    }
}