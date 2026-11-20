using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FireArrowProjectile : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f;

    private Rigidbody rb;
    private bool hasLaunched = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Log("FireArrowProjectile起動：" + gameObject.name);
    }

    public void Launch(Vector3 direction, float speed)
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbodyが取得できません！");
            return;
        }

        // Rigidbodyの設定を初期化
        rb.isKinematic = false;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.None;
        rb.detectCollisions = true;

        // 発射方向と速度を設定
        direction.Normalize();

        rb.velocity = direction * speed;

        hasLaunched = true;

        Debug.Log("矢の発射処理を実行！");
        Debug.Log("発射方向：" + direction);
        Debug.Log("発射速度：" + rb.velocity);

        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        if (!hasLaunched) return;

        Debug.Log(
            "矢の現在位置：" + transform.position
        );
    }

    void OnTriggerEnter(Collider other)
    {
        // プレイヤーには当たらない
        if (other.CompareTag("Player"))
        {
            return;
        }

        // 木の根に当たった場合
        BurnableRoot root =
            other.GetComponentInParent<BurnableRoot>();

        if (root != null)
        {
            Debug.Log("木の根に着弾！");

            root.Ignite();

            Destroy(gameObject);
            return;
        }

        // その他のオブジェクトに当たった場合
        if (!other.isTrigger &&
            !other.CompareTag("Player"))
        {
            Debug.Log("矢が接触：" + other.name);
            Destroy(gameObject);
        }
    }
}
