
using UnityEngine;

public class ArrowStick : MonoBehaviour
{
    [Header("刺さる対象のタグ")]
    public string targetTag = "Enemy";

    [Header("矢の刺さる位置")]
    public Transform stickPoint;

    [Header("刺さったときの設定")]
    public bool destroyRigidbody = false;

    private Rigidbody rb;
    private bool isStuck = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // すでに刺さっている場合は何もしない
        if (isStuck) return;

        // 指定したタグ以外なら何もしない
        if (!collision.gameObject.CompareTag(targetTag)) return;

        // 衝突位置を取得
        ContactPoint contact = collision.contacts[0];

        // 矢の角度を保存
        Quaternion fixedRotation = transform.rotation;

        // 矢を停止
        isStuck = true;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // 衝突したオブジェクトの子にする
        transform.SetParent(collision.transform, true);

        // 矢の先端を衝突位置に合わせる
        if (stickPoint != null)
        {
            Vector3 offset = stickPoint.position - transform.position;
            transform.position = contact.point - offset;
        }
        else
        {
            transform.position = contact.point;
        }

        // 発射時の角度を維持する
        transform.rotation = fixedRotation;

        // Rigidbodyを削除したい場合
        if (destroyRigidbody && rb != null)
        {
            Destroy(rb);
        }
    }
}
