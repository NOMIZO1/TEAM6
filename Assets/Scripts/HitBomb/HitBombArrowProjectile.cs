using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HitBombArrowProjectile : MonoBehaviour
{
    [Header("矢の設定")]
    [SerializeField] private float lifeTime = 5f;

    private Rigidbody rb;
    private bool hasLaunched = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // 矢の発射
    public void Launch(Vector3 direction, float speed)
    {
        if (rb == null) return;

        rb.isKinematic = false;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.None;
        rb.detectCollisions = true;

        direction.Normalize();
        rb.velocity = direction * speed;

        hasLaunched = true;

        Debug.Log("ヒットボムの矢を発射しました！");

        Destroy(gameObject, lifeTime);
    }

    // オブジェクトへの衝突
    private void OnCollisionEnter(Collision collision)
    {
        if (!hasLaunched) return;

        // プレイヤーには当たらない
        if (collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        Debug.Log(
            "ヒットボム命中：" + collision.gameObject.name
        );

        // 矢を削除
        Destroy(gameObject);
    }
}
