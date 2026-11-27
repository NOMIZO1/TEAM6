
using UnityEngine;

public class ArrowStick : MonoBehaviour
{
    [Header("刺さる対象のタグ")]
    public string targetTag = "Enemy";

    [Header("矢の刺さる位置")]
    public Transform stickPoint;

    [Header("ダメージ設定")]
    public float arrowDamage = 20f;

    [Header("刺さったときの設定")]
    public bool destroyRigidbody = false;

    [Header("属性エフェクト")]
    [SerializeField] private GameObject fireEffect;
    [SerializeField] private GameObject iceEffect;

    private Element arrowElement = Element.None;

    private Rigidbody rb;
    private bool isStuck = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Init(float damage, Element element)
    {
        arrowDamage = damage;
        SetElement(element);
    }

    public void SetElement(Element element)
    {
        arrowElement = element;
        if (fireEffect != null) fireEffect.SetActive(element == Element.Fire);
        if (iceEffect != null) iceEffect.SetActive(element == Element.Ice);
    }

    void OnCollisionEnter(Collision collision)
    {
        // すでに刺さっている場合は何もしない
        if (isStuck) return;

        // 指定したタグ以外なら何もしない
        if (!collision.gameObject.CompareTag(targetTag)) return;

        // 敵のEnemyControllerを取得
        EnemyController enemy =
            collision.gameObject.GetComponentInParent<EnemyController>();

        // 敵にダメージを与える
        if (enemy != null)
        {
            enemy.TakeDamage(arrowDamage);
            enemy.ApplyElement(arrowElement, arrowDamage);
        }

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

        // 衝突した敵の子オブジェクトにする
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

        // 発射時の角度を維持
        transform.rotation = fixedRotation;

        // Rigidbodyを削除したい場合
        if (destroyRigidbody && rb != null)
        {
            Destroy(rb);
        }

        // 矢を削除
        Destroy(gameObject);
    }

}
