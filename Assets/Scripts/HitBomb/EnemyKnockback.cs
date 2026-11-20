using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyKnockback : MonoBehaviour
{
    [Header("吹っ飛ばし設定")]
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float upwardForce = 2f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // 攻撃を受けた位置から吹っ飛ばす
    public void ApplyKnockback(Vector3 attackPosition)
    {
        if (rb == null) return;

        // 吹っ飛ぶ方向を計算
        Vector3 direction = transform.position - attackPosition;

        // 水平方向だけにする
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
        {
            direction = transform.forward;
        }

        direction.Normalize();

        // 現在の速度をリセット
        rb.velocity = Vector3.zero;

        // 吹っ飛ばし処理
        Vector3 force =
            direction * knockbackForce
            + Vector3.up * upwardForce;

        rb.AddForce(force, ForceMode.Impulse);

        Debug.Log("Enemyが吹っ飛びました！");
    }
}
